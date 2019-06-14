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
    public GameObject content;
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
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.transform.childCount * 100, 0);
    }

    void FixedUpdate ()
    {
        if (toRotate != null)
            toRotate.Rotate(0, (Time.deltaTime / Time.timeScale) * 20, 0);
    }

    void Update ()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !gameController.GetComponent<Driving>().driving)
        {
            if (buildMenu.activeSelf)
                CloseMenu(buildMenu);
            gameObject.GetComponent<PauseMenu>().SwapMenus();
        }
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