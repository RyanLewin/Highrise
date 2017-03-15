using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : MonoBehaviour {

    public Building building;
    public ResourceManager resources;
    public GameObject gameController;

    void Start ()
    {
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (controller.GetComponent<Building>())
                gameController = controller;
        building = gameController.GetComponent<Building>();
        resources = gameController.GetComponent<ResourceManager>();
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.GetComponent<BuildingTypes>() && other.GetComponent<BuildingTypes>().placed)
        {
            building.Deletion(other.transform);
            resources.happiness -= 5;
        }
    }
}
