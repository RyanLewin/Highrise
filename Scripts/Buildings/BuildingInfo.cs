using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfo : MonoBehaviour {

    public Transform currBuilding;
    public Transform buildingUpgrade;

    public Transform upgradeButton;
    public Transform vehicleButton;

    public Building building;
    public ResourceManager resources;
    public Driving driving;
    public Camera currentCam;

    public void DestroyMenu ()
    {
        Destroy(gameObject,.3f);
    }

    public void Update ()
    {
        if (currentCam == Camera.main)
            transform.LookAt(Camera.main.transform.GetChild(0));
        else
            Destroy(gameObject);

        if (!currBuilding)
            DestroyMenu();

        if (buildingUpgrade)
            if (resources.money >= buildingUpgrade.GetComponent<BuildingTypes>().cost)
                upgradeButton.GetComponent<Button>().interactable = true;
    }

    public void SpawnVehicle()
    {
        currBuilding.GetComponent<BuildingTypes>().Spawn();
        DestroyMenu();
    }

    public void UpgradeBuilding ()
    {
        BuildingTypes buildPrefab = buildingUpgrade.GetComponent<BuildingTypes>();
        BuildingTypes build = Instantiate(buildPrefab, currBuilding.position, currBuilding.rotation, currBuilding.parent) as BuildingTypes;
        build.cell = currBuilding.GetComponent<BuildingTypes>().cell;
        build.dir = currBuilding.GetComponent<BuildingTypes>().dir;
        build.GetPlaced = true;
        resources.money -= build.cost;

        if (currBuilding.GetComponent<House>())
            build.GetComponent<House>().level = currBuilding.GetComponent<House>().level++;

        Destroy(currBuilding.gameObject);
        DestroyMenu();
    }

    public void DestroyBuilding ()
    {
        DestroyMenu();
        building.Deletion(currBuilding);
    }

    public Transform Upgradable
    {
        set
        {
            buildingUpgrade = value;
            if (resources.money >= buildingUpgrade.GetComponent<BuildingTypes>().cost)
                upgradeButton.GetComponent<Button>().interactable = true;
        }
    }
}
