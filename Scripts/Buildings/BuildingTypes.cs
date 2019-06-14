using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingTypes : MonoBehaviour, IPointerClickHandler {
    
    public Cells cell;
    public Direction dir;
    public bool placed = false;

    public int cost = 50;
    public int xMax = 2, zMax = 2;
    public int buildTime = 60;    // in seconds   buildtime/60 = hours, buildtime%60 = minutes
    public float buildStartY = 0;

    [Tooltip("Only for service buildings!")]
    public GameObject parkingArea;

    protected Camera currentCam;
    protected CameraController cameraController;
    //protected TimeController timeController;
    protected GameObject gameController;
    protected ResourceManager resources;
    protected Building building;
    protected Driving driving;
    protected News news;
    protected Statistics statistics;

    protected virtual void Awake()
    {
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (controller.GetComponent<CameraController>())
                cameraController = controller.GetComponent<CameraController>();
            else
                gameController = controller;

        //timeController = GameObject.FindGameObjectWithTag("TimeController").GetComponent<TimeController>();
        resources = gameController.GetComponent<ResourceManager>();
        building = gameController.GetComponent<Building>();
        driving = gameController.GetComponent<Driving>();
        news = gameController.GetComponent<News>();
        statistics = GameObject.FindGameObjectWithTag("UI").GetComponent<Statistics>();
    }

    void LateUpdate()
    {
        currentCam = cameraController.GetCurrentCam;
    }
    
    public IEnumerator Build ()
    {
        Vector3 currentPos = transform.position;
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / buildTime;
            transform.position = Vector3.Lerp(currentPos, new Vector3(transform.position.x, 0, transform.position.z), t);
            yield return null;
        }
        placed = true;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        return;
    }

    public virtual void Clicked ()
    {
        return;
    }

    public Canvas CreateInfo (Canvas info, GameObject upgrade, bool service)
    {
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Details"))
            Destroy(i);

        Canvas infoPrefab = Resources.Load("Prefab/UI/DetailsCanvas", typeof(Canvas)) as Canvas;
        info = Instantiate(infoPrefab) as Canvas;
        info.transform.position = new Vector3(transform.position.x, 75f, transform.position.z);
        info.GetComponent<Selectable>().Select();

        BuildingInfo buildingInfo = info.GetComponent<BuildingInfo>();
        buildingInfo.currentCam = currentCam;
        buildingInfo.building = building;
        buildingInfo.resources = resources;
        buildingInfo.driving = driving;
        buildingInfo.currBuilding = transform;
        if (!service)
        {
            if (upgrade != null)
                buildingInfo.Upgradable = upgrade.transform;
        }
        else
        {
            buildingInfo.upgradeButton.gameObject.SetActive(false);
            buildingInfo.vehicleButton.gameObject.SetActive(true);
        }
        return info;
    }

    protected bool justSpawned = false;
    public void Spawn()
    {
        justSpawned = true;
        driving.driving = true;
        driving.SpawnVehicle(gameObject);
        parkingArea.SetActive(true);
    }

    public int GetCost
    {
        get { return cost; }
        set {  cost *= value; }
    }

    public Cells GetCell { get { return cell; } }
    public bool GetPlaced { get { return placed; } set { placed = value; } }
}
