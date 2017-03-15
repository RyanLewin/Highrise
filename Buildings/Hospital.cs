using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : BuildingTypes {
    
    public int hospitalCost = 750;
    public int upkeep = 250;
    int currentDay;

    public bool justSpawned = false;

    protected override void Awake()
    {
        base.Awake();
        cost = hospitalCost;
        xMax = 10;
        zMax = 5;
        currentDay = resources.day;
    }

    void Update ()
    {
        if (resources.day != currentDay)
        {
            currentDay = resources.day;
            resources.money -= upkeep;
        }
    }

    void OnMouseUpAsButton ()
    {
        if (!resources.paused)
        {
            if (Input.GetKey(KeyCode.LeftShift) && currentCam == Camera.main)
            {
                justSpawned = true;
                driving.driving = true;
                driving.SpawnVehicle(this.gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vehicle/Ambulance" && !justSpawned)
        {
            driving.driving = false;
            cameraController.ChangeCamera = true;
            Destroy(other.gameObject);
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
