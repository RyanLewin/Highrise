using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Controller : MonoBehaviour
{
    public GameObject buildMenu;
    public GameObject buildButton;
    public Transform resourcesDisplay;
    Transform toRotate;
    Quaternion baseRotation;

    int resourceInX = 250;  
    int resourceOutX = 50; 
    bool lockResources = false;

    public GameObject gameController;
    Building building;
    ResourceManager resources;

    void Awake()
    {
        building = gameController.GetComponent<Building>();
        resources = gameController.GetComponent<ResourceManager>();
    }

    void FixedUpdate ()
    {
        if (toRotate != null)
            toRotate.Rotate(0, (Time.deltaTime / Time.timeScale) * 20, 0);
    }

    void Update ()
    {
        resourcesDisplay.GetChild(0).GetChild(1).GetComponent<Text>().text = resources.creative ? "Coins: Inf" : "Coins: " + resources.money;
        resourcesDisplay.GetChild(1).GetChild(1).GetComponent<Text>().text = "Population: " + resources.population + " / " + resources.maxPopulation;
        resourcesDisplay.GetChild(2).GetChild(1).GetComponent<Text>().text = "Happiness: " + resources.happiness + "%";
    }

    public void LockResources () { lockResources = !lockResources; }

    /// <summary>Make the UI size change on hover enter and exit</summary>
    public void ToggleResourcesDisplay()
    {
        if (!lockResources)
        {
            //resourcesDisplay.localPosition = new Vector3(resourceOutX, resourcesDisplay.localPosition.y, 0);
            RectTransform rt = resourcesDisplay.GetComponent<RectTransform>();

            Vector2 newPos = rt.anchoredPosition;
            newPos.x = (newPos.x >= -50) ? -200 : -50;
            rt.anchoredPosition = newPos;
        }
    }

    public void SetGameSpeed (int speed)
    {
        resources.UpdateTime(speed);
    }

    public void Rotate (Transform menuItem)
    {
        // set toRotate transform to menuItem
        baseRotation = menuItem.rotation;
        toRotate = menuItem;
    }

    public void StopRotate ()
    {
        if (toRotate != null)
        {
            toRotate.rotation = baseRotation;
            toRotate = null;
        }
    }

    public void OpenMenu(GameObject menu)
    {
        menu.SetActive(true);
        buildButton.SetActive(false);
    }

    public void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
        buildButton.SetActive(true);

        building.obj = 0;
        building.ResetObjectToBuild(null);
    }
    
    //public void DestroyMenu(GameObject menu)
    //{
    //    Destroy(menu);
    //}

    //public void ToggleMenu() { this.gameObject.SetActive(!this.gameObject.activeSelf); }

    public void StartBuilding(BuildingTypes toBuild)
    {
        //building.obj = itemIndex;
        building.ResetObjectToBuild(toBuild);
        //CloseMenu(buildMenu);
    }

    public void MakeXRed(Text t)
    {
        t.color = Color.red;
    }

    public void MakeXWhite(Text t)
    {
        t.color = Color.white;
    }
}