using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Hospital : BuildingTypes
{
    public int hospitalCost = 750;
    public int upkeep = 250;
    int currentDay;
    int currentMonth;
    int patients = 0;
    public int maxPatients = 100;

    protected override void Awake()
    {
        base.Awake();
        cost = hospitalCost;
        xMax = 10;
        zMax = 5;
        currentDay = TimeController.Day;
        currentMonth = TimeController.Month;
    }

    Canvas info;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (currentCam == Camera.main && placed)
        {
            info = CreateInfo(info, null, true);
            info.transform.GetChild(1).GetComponent<Text>().text = "Patients\n" + patients + " / " + maxPatients;
        }
    }

    void Update ()
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
            patients -= 10;
        }
    }

    void OnMouseUpAsButton ()
    {
        if (!resources.paused)
        {
            if (placed)
            {
                if (Input.GetKey(KeyCode.LeftShift) && currentCam == Camera.main)
                {
                    if (patients < maxPatients)
                    {
                        Spawn();
                    }
                }
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vehicle/Ambulance" && !justSpawned)
        {
            driving.driving = false;
            cameraController.ChangeCamera = true;
            patients += other.GetComponent<Vehicle>().totalPassengers;
            Destroy(other.gameObject);
            parkingArea.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Vehicle/Ambulance")
        {
            justSpawned = false;
        }
    }
}
