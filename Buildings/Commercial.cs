using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Commercial : BuildingTypes {

    public int commercialCost = 100;
    public int tax = 5;
    public int workers = 0;
    public int maxWorkers = 4;
    int currentHour;

    protected override void Awake()
    {
        cost = commercialCost;
        xMax = 3;
        zMax = 2;
        base.Awake();
        currentHour = resources.hour;
        resources.workingPopulation += workers;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (currentCam == Camera.main && placed)
        {
            foreach (GameObject i in GameObject.FindGameObjectsWithTag("Details"))
                Destroy(i);

            Canvas infoPrefab = Resources.Load("Prefab/UI/DetailsCanvas", typeof(Canvas)) as Canvas;
            Canvas info = Instantiate(infoPrefab) as Canvas;
            info.transform.GetChild(1).GetComponent<Text>().text = "Workers\n" + workers + " / " + maxWorkers;
            info.transform.position = new Vector3(transform.position.x, 75f, transform.position.z);

            info.GetComponent<BuildingInfo>().currentCam = currentCam;
            info.GetComponent<BuildingInfo>().building = building;
            info.GetComponent<BuildingInfo>().currBuilding = transform;
        }
    }

    void Update ()
    {
        if (placed)
        {
            if (workers < maxWorkers)
                if (resources.workingPopulation < resources.population)
                {
                    workers++;
                    resources.workingPopulation++;
                } 

            if (resources.workingPopulation > resources.population)
            {
                workers--;
                resources.workingPopulation--;
            }

            if (resources.hour != currentHour)
            {
                currentHour = resources.hour;
                resources.money += tax * workers;
            }
        }
    }

    void OnDestroy ()
    {
        if (placed)
            resources.workingPopulation -= workers;
    }
}
