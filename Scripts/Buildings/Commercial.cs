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
        currentHour = Mathf.FloorToInt(TimeController.Time) / 60;
        resources.workingPopulation += workers;
    }

    Canvas info;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (currentCam == Camera.main && placed)
        {
            info = CreateInfo(info, null, false);
            info.transform.GetChild(1).GetComponent<Text>().text = "Workers\n" + workers + " / " + maxWorkers;
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
            if (workers > 0)
            {
                if (resources.workingPopulation > resources.population)
                {
                    workers--;
                    resources.workingPopulation--;
                }
            }

            int actualHour = Mathf.FloorToInt(TimeController.Time) / 60;
            if (actualHour != currentHour)
            {
                currentHour = actualHour;
                resources.money += (tax * workers);
                statistics.MoneyMade = tax * workers;
            }
        }
    }

    void OnDestroy ()
    {
        if (placed)
            resources.workingPopulation -= workers;
    }
}
