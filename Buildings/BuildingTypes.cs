using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingTypes : MonoBehaviour, IPointerClickHandler {
    
    public Cells cell;
    public Direction dir;
    public bool placed = false;

    public int cost = 50;
    public int xMax = 2, zMax = 2;
    public int buildTime = 60;    // in seconds   buildtime/60 = hours, buildtime%60 = minutes

    protected Camera currentCam;
    protected CameraController cameraController;
    protected GameObject gameController;
    protected ResourceManager resources;
    protected Building building;
    protected Driving driving;

    protected virtual void Awake()
    {
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (controller.GetComponent<CameraController>())
                cameraController = controller.GetComponent<CameraController>();
            else
                gameController = controller;
        resources = gameController.GetComponent<ResourceManager>();
        building = gameController.GetComponent<Building>();
        driving = gameController.GetComponent<Driving>();
    }

    void LateUpdate()
    {
        currentCam = cameraController.GetCurrentCam;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        return;
    }

    public int GetCost
    {
        get { return cost; }
        set {  cost *= value; }
    }

    public Cells GetCell { get { return cell; } }
    public bool GetPlaced { get { return placed; } set { placed = value; } }
}
