using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceStation : BuildingTypes {
    
    public int policeStationCost = 750;
    public int upkeep = 250;
    int currentDay;

    bool justSpawned = false;

    protected override void Awake()
    {
        base.Awake();
        cost = policeStationCost;
        xMax = 4;
        zMax = 3;
        currentDay = resources.day;
    }

    void Update()
    {
        if (resources.day != currentDay)
        {
            currentDay = resources.day;
            resources.money -= upkeep;
        }
    }

    void OnMouseUpAsButton()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentCam == Camera.main)
        {
            justSpawned = true;
            driving.driving = true;
            driving.SpawnVehicle(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vehicle/PoliceCar" && !justSpawned)
        {
            driving.driving = false;
            cameraController.ChangeCamera = true;
            Destroy(other.gameObject);
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.tag == "Vehicle/PoliceCar")
            justSpawned = false;
    }
}
