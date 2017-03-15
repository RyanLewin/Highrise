using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStation : BuildingTypes {
    
    public int fireStationCost = 750;
    public int upkeep = 250;
    int currentDay;

    bool justSpawned = false;

    protected override void Awake()
    {
        base.Awake();
        currentDay = resources.day;
        cost = fireStationCost;
        xMax = 7;
        zMax = 4;
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
        if (other.tag == "Vehicle/FireEngine" && !justSpawned)
        {
            driving.driving = false;
            cameraController.ChangeCamera = true;
            Destroy(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Vehicle/FireEngine")
            justSpawned = false;
    }
}
