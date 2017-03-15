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

    DriveUI_Controller driveUI;
    Canvas canvasDrive;
    Transform houseCanvas;
    Emergencies emergencies;

    public Person[] personPrefab;
    Person person;
    public Cop copPrefab;
    Cop cop;
    Transform spawnPoint;

    float illtimer = 0f;
    float robberytimer = 0f;
    const float chanceTime = 5f;

    bool ill = false;
    bool robbery = false;
    GameObject self;
    
    protected override void Awake ()
    {
        cost = houseCost;
        xMax = 2;
        zMax = 2;
        base.Awake();
        self = this.gameObject;

        foreach (GameObject amb in GameObject.FindGameObjectsWithTag("Vehicle/Ambulance"))
            if (amb.transform.GetComponent<Canvas>())
                canvasDrive = amb.transform.GetComponent<Canvas>();

        driveUI = canvasDrive.GetComponent<DriveUI_Controller>();
        houseCanvas = transform.GetChild(2);

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
                foreach (GameObject i in GameObject.FindGameObjectsWithTag("Details"))
                    Destroy(i);

                Canvas infoPrefab = Resources.Load("Prefab/UI/DetailsCanvas", typeof(Canvas)) as Canvas;
                info = Instantiate(infoPrefab) as Canvas;
                info.transform.GetChild(1).GetComponent<Text>().text = "Residents\n" + residents + " / " + maxResidents;
                info.transform.position = new Vector3(transform.position.x, 75f, transform.position.z);

                BuildingInfo buildingInfo = info.GetComponent<BuildingInfo>();
                buildingInfo.currentCam = currentCam;
                buildingInfo.building = building;
                buildingInfo.resources = resources;
                buildingInfo.currBuilding = transform;
                if (upgrade != null)
                    buildingInfo.Upgradable = upgrade.transform;
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
        StartCoroutine(RobbedEnum());
        houseCanvas.GetChild(1).gameObject.SetActive(true);
    }

    //set ill to true and add house to ill list
    void Illness()
    {
        ill = true;
        transform.GetComponent<BoxCollider>().enabled = true;
        AffectHappiness(5);
        resources.maxHappiness -= 5;
        driveUI.AddToIll(self.gameObject);
        StartCoroutine(DeathFromIllness());
        houseCanvas.GetChild(0).gameObject.SetActive(true);
    }

    void OnDestroy ()
    {
        if (placed)
        {
            resources.maxPopulation -= maxResidents;
            resources.population -= residents;
            if (ill)
            {
                driveUI.RemoveFromList(self.gameObject);
                AffectHappiness(20);
                resources.maxHappiness += 5;
            }
            if (robbery)
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
    }

    void OnTriggerExit (Collider other)
    {
        if ((ill && other.tag == "Vehicle/Ambulance") && placed)
            person.SetTarget(spawnPoint);
        //else if ((robbery && other.tag == "Vehicle/PoliceCar") && placed)
        //{
        //    EndRobbery(false);
        //    Destroy(person.gameObject, 0.5f);
        //}
        else if (other.gameObject == person && !robbery)
            person.SetTarget(spawnPoint);
    }

    void Police (Collider other)
    {
        driving.cop = true; //stop car movement
        
        //Instantiate Cop
        cop = Instantiate(copPrefab);
        cop.transform.position = new Vector3(other.transform.position.x, 4.25f, other.transform.position.z);
        cop.house = this;
        gameController.GetComponent<Driving>().copPrefab = cop.gameObject;

        //Instantiate Robber
        spawnPoint = transform.GetChild(3);
        person = Instantiate(personPrefab[2], spawnPoint.position, spawnPoint.rotation, transform);
        person.house = this;
        person.GetComponent<Robber>().vehicle = other.GetComponent<Vehicle>();
        person.SetTarget(cop.transform);
    }
    
    void Ambulance (Collider other)
    {
        spawnPoint = transform.GetChild(3);
        person = Instantiate(ill ? personPrefab[1] : personPrefab[2], spawnPoint.position, spawnPoint.rotation, transform);
        person.house = this;
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
        }
        else
        {
            Destroy(person.gameObject);
            residents--;
            resources.population--;
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
            transform.GetComponent<BoxCollider>().enabled = false;
            driveUI.RemoveFromList(self.gameObject);
            houseCanvas.GetChild(0).gameObject.SetActive(false);
            resources.maxHappiness += 5;
    }

    IEnumerator DeathFromIllness ()
    {
        yield return new WaitForSeconds(360f);

        if (ill)
        {
            ill = false;
            driveUI.RemoveFromList(self.gameObject);
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
