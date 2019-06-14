using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FireStation : BuildingTypes {
    
    public int fireStationCost = 750;
    public int upkeep = 250;
    int currentDay;

    protected override void Awake()
    {
        base.Awake();
        currentDay = TimeController.Day;
        cost = fireStationCost;
        xMax = 7;
        zMax = 4;
    }

    Canvas info;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (currentCam == Camera.main && placed)
        {
            info = CreateInfo(info, null, true);
            info.transform.GetChild(1).GetComponent<Text>().text = "";
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
    }

    void OnMouseUpAsButton()
    {
        if (!resources.paused)
        {
            if (placed)
            {
                if (Input.GetKey(KeyCode.LeftShift) && currentCam == Camera.main)
                {
                    Spawn();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vehicle/FireEngine" && !justSpawned)
        {
            driving.driving = false;
            cameraController.ChangeCamera = true;
            Destroy(other.gameObject);
            parkingArea.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Vehicle/FireEngine")
            justSpawned = false;
    }
}
