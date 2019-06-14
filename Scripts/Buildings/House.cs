using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class House : BuildingTypes {
    
    int residents = 0;
    public int maxResidents = 4;
    public int level = 1;
    public GameObject upgrade = null;

    public int houseCost = 50;
    
    [SerializeField] Transform houseCanvas;
    Emergencies emergencies;

    public Person[] personPrefab;
    Person person;
    public Cop copPrefab;
    Cop cop;
    [SerializeField] Transform spawnPoint;

    ParticleSystem fireCurr;
    public ParticleSystem firePrefab;
    public ParticleSystem smoke;

    float illtimer = 0f;
    float robberytimer = 0f;
    float firetimer = 0f;
    const float chanceTime = 5f;

    public bool ill = false;
    public bool robbery = false;
    public bool fire = false;
    
    protected override void Awake ()
    {
        cost = houseCost;
        xMax = 2;
        zMax = 2;
        base.Awake();
        
        if(!houseCanvas)
            houseCanvas = transform.GetChild(2);
        if(!spawnPoint)
            spawnPoint = transform.GetChild(3);
        
        emergencies = gameController.GetComponent<Emergencies>();

        resources.maxPopulation += maxResidents;
        if (resources.happiness > 70)
        {
            residents = Random.Range(1, 3);
            resources.population += residents;
        }
    }

    Canvas info = null;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!info)
        {
            if (currentCam == Camera.main && placed)
            {
                info = CreateInfo(info, upgrade, false);
                info.transform.GetChild(1).GetComponent<Text>().text = "Residents\n" + residents + " / " + maxResidents;
            }
        }
    }

    void Update()
    {
        if (placed)
        {
            //1 in 10,000 chance someone else moves in
            if (resources.happiness > 70 && residents < maxResidents)
            {
                int randPop = Random.Range(0, 10000);
                if (randPop < 1)
                {
                    residents++;
                    resources.population++;
                }
            }
            
            if (info)
                info.transform.GetChild(1).GetComponent<Text>().text = "Residents\n" + residents + " / " + maxResidents;

            if (resources.happiness < 40 && residents > 0)
            {
                int randPop = Random.Range(0, 500);
                if (randPop < 1)
                {
                    residents--;
                    resources.population--;
                    if (residents == 0)
                    {
                        ill = false;
                        StopCoroutine(DeathFromIllness());
                        houseCanvas.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }

            #region emergencies
            //Chance of a house contracting illness
            if (!ill && residents > 0)
            {
                illtimer -= Time.deltaTime;
                if (illtimer < 0f)
                {
                    int rand = Random.Range(0, 10000);
                    if (rand < emergencies.chanceOfIllness * residents)
                        Illness();
                    illtimer = chanceTime;
                }
            }

            if (!robbery && residents > 0)
            {
                robberytimer -= Time.deltaTime;
                if (robberytimer < 0f)
                {
                    int rand = Random.Range(0, 10000);
                    if (rand < emergencies.chanceOfRobbery * (level * 2))
                        Robbery();
                    robberytimer = chanceTime;
                }
            }

            if (!fire)
            {
                firetimer -= Time.deltaTime;
                if (firetimer < 0f)
                {
                    int rand = Random.Range(0, 10000);
                    if (rand < emergencies.chanceOfFire * residents)
                        Fire();
                    firetimer = chanceTime;
                }
            }

            if (robbery || ill)
            {
                //make illness icon look at main camera if you're not driving
                if (currentCam == Camera.main)
                    houseCanvas.LookAt(Camera.main.transform.GetChild(0));
                else
                    houseCanvas.eulerAngles = new Vector3(-90, 0, 0);
            }
            #endregion
        }
    }

    //set robbery to true and lower happiness
    void Robbery()
    {
        robbery = true;
        transform.GetComponent<BoxCollider>().enabled = true;
        AffectHappiness(5);
        resources.maxHappiness -= 5;
        statistics.Robberies++;
        StartCoroutine(RobbedEnum());
        houseCanvas.GetChild(1).gameObject.SetActive(true);

        string newsInfo = "Armed Robbery at [" + cell.x + ", " + cell.z + "]!"; 
        news.AddToNews(newsInfo);
    }

    //set ill to true and add house to ill list
    void Illness()
    {
        ill = true;
        transform.GetComponent<BoxCollider>().enabled = true;
        AffectHappiness(5);
        resources.maxHappiness -= 5;
        statistics.IllPeople++;
        StartCoroutine(DeathFromIllness());
        houseCanvas.GetChild(0).gameObject.SetActive(true);
    }

    //set ill to true and add house to ill list
    void Fire()
    {
        fire = true;
        fireCurr = Instantiate(firePrefab, transform.position, firePrefab.transform.rotation, transform);
        transform.GetComponent<BoxCollider>().enabled = true;
        AffectHappiness(5);
        resources.maxHappiness -= 5;
        statistics.Fires++;
        StartCoroutine(BurnDown());

        string newsInfo = "Fire Breaks out at [" + cell.x + ", " + cell.z + "]!";
        news.AddToNews(newsInfo);
    }

    void OnDestroy ()
    {
        if (placed)
        {
            resources.maxPopulation -= maxResidents;
            resources.population -= residents;
            if (ill || robbery || fire)
            {
                AffectHappiness(20);
                resources.maxHappiness += 5;
            }
        }
    }
    
    void OnTriggerEnter (Collider other)
    {
        if ((ill && other.tag == "Vehicle/Ambulance") && placed)
            if (other.GetComponent<Vehicle>().totalPassengers < other.GetComponent<Vehicle>().maxPassengers)
                Ambulance(other);
       if ((robbery && other.tag == "Vehicle/PoliceCar") && placed)
            if (other.GetComponent<Vehicle>().totalPassengers < other.GetComponent<Vehicle>().maxPassengers && !cop)
                Police(other);
        if ((fire && other.tag == "Vehicle/FireEngine") && placed)
            if (other.GetComponent<Vehicle>().totalPassengers < other.GetComponent<Vehicle>().maxPassengers)
                FireEngine(other);
    }

    void OnTriggerExit (Collider other)
    {
        if (person)
        {
            if ((ill && other.tag == "Vehicle/Ambulance") && placed)
                person.SetTarget(spawnPoint);
            else if (other.gameObject == person && !robbery)
                person.SetTarget(spawnPoint);
        }
        else if ((fire && other.tag == "Vehicle/FireEngine") && placed)
            driving.InRange = false;
    }

    int waterHit;
    void OnParticleCollision(GameObject other)
    {
        if (fire)
        {
            waterHit++;
            if (waterHit >= 300)
                EndFire();
        }
    }

    void FireEngine (Collider other)
    {
        driving.InRange = true;
    }

    void Police (Collider other)
    {
        driving.cop = true; //stop car movement

        if (!GameObject.FindGameObjectWithTag("Person/Cop"))
        {
            //Instantiate Cop
            cop = Instantiate(copPrefab);
            cop.transform.position = new Vector3(other.transform.position.x, 4.25f, other.transform.position.z);
            cop.house = this;
            gameController.GetComponent<Driving>().copPrefab = cop.gameObject;
        }
        else
            cop = GameObject.FindGameObjectWithTag("Person/Cop").GetComponent<Cop>();

        //Instantiate Robber
        person = Instantiate(personPrefab[2], spawnPoint.position, spawnPoint.rotation, transform);
        person.house = this;
        person.GetComponent<Robber>().vehicle = other.GetComponent<Vehicle>();
        person.SetTarget(cop.transform);
    }
    
    void Ambulance (Collider other)
    {
        if (!person)
        {
            person = Instantiate(ill ? personPrefab[1] : personPrefab[2], spawnPoint.position, spawnPoint.rotation, transform);
            person.house = this;
            person.SetTarget(other.transform);
        }
        else
            person.SetTarget(other.transform);
    }

    public void EndRobbery (bool arrested)
    {
        resources.maxHappiness += 5;
        houseCanvas.GetChild(1).gameObject.SetActive(false);
        transform.GetComponent<BoxCollider>().enabled = false;
        StopCoroutine(RobbedEnum());
        robbery = false;
        if (arrested)
        {
            resources.happiness += 3;
            statistics.RobbersCaught++;
        }
        else
        {
            if (person)
            {
                Instantiate(smoke, person.transform.position, smoke.transform.rotation);
                Destroy(person.gameObject, 1f);
            }
            if (residents > 0)
            {
                residents--;
                resources.population--;
            }
            AffectHappiness(20);
        }
    }

    IEnumerator RobbedEnum ()
    {
        yield return new WaitForSeconds(360f);

        if (robbery)
            EndRobbery(false);
    }

    public void Collected ()
    {
        ill = false;
        StopCoroutine(DeathFromIllness());
        statistics.IllRecovered++;
        transform.GetComponent<BoxCollider>().enabled = false;
        houseCanvas.GetChild(0).gameObject.SetActive(false);
        resources.maxHappiness += 5;
    }

    IEnumerator DeathFromIllness ()
    {
        yield return new WaitForSeconds(360f);

        if (ill)
        {
            ill = false;
            houseCanvas.GetChild(0).gameObject.SetActive(false);
            residents--;
            resources.population--;
            AffectHappiness(20);
            resources.maxHappiness += 5;
        }
    }

    public void EndFire()
    {
        StopCoroutine(BurnDown());
        fire = false;
        statistics.FiresPutOut++;
        waterHit = 0;
        resources.maxHappiness += 5;
        Destroy(fireCurr.gameObject);
        transform.GetComponent<BoxCollider>().enabled = false;
        driving.InRange = false;
    }

    IEnumerator BurnDown()
    {
        yield return new WaitForSeconds(360f);

        if (fire)
        {
            building.Deletion(transform);
            houseCanvas.GetChild(0).gameObject.SetActive(false);
            residents--;
            resources.population--;
            AffectHappiness(20);
            resources.maxHappiness += 5;
        }
    }

    void AffectHappiness (int x)
    {
        if (resources.happiness >= x)
            resources.happiness -= x;
        else if (resources.happiness < x && resources.happiness > 0)
            resources.happiness = 0;
    }
}
