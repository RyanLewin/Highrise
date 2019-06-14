using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PoliceStation : BuildingTypes {
    
    public int policeStationCost = 750;
    public int upkeep = 250;
    int currentDay;
    int currentMonth;
    int inmates = 0;
    public int maxInmates = 50;

    protected override void Awake()
    {
        base.Awake();
        cost = policeStationCost;
        xMax = 4;
        zMax = 3;
        currentDay = TimeController.Day;
        currentMonth = TimeController.Month;
    }

    Canvas info;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (currentCam == Camera.main && placed)
        {
            info = CreateInfo(info, null, true);
            info.transform.GetChild(1).GetComponent<Text>().text = "Inmates\n" + inmates + " / " + maxInmates;
        }
    }

    void Update()
    {
        if (TimeController.Day != currentDay)
        {
            currentDay = TimeController.Day;
            resources.money -= upkeep;
            statistics.MoneyLost = upkeep;
        }

        if (TimeController.Month != currentMonth)
        {
            currentMonth = TimeController.Month;
            inmates -= 10;
        }
    }

    void OnMouseUpAsButton()
    {
        if (!resources.paused)
        {
            if (placed)
            {
                if (Input.GetKey(KeyCode.LeftShift) && currentCam == Camera.main)
                {
                    if (inmates < maxInmates)
                    {
                        Spawn();
                    }
                }
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vehicle/PoliceCar" && !justSpawned)
        {
            driving.driving = false;
            cameraController.ChangeCamera = true;
            inmates += other.GetComponent<Vehicle>().totalPassengers;
            Destroy(other.gameObject);
            parkingArea.SetActive(false);
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.tag == "Vehicle/PoliceCar")
            justSpawned = false;
    }
}
