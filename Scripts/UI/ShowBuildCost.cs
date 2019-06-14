using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBuildCost : MonoBehaviour
{
    public Text objectName, costText, timeText;

    private void Start()
    {
        Disable();
    }

    private void OnEnable()
    {
        // check if global var are not null
    }

    private void OnDisable()
    {

    }

    private int counter = 0;
    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(mousePos.x, rt.anchorMax.y);

        Vector2 v = Camera.main.ScreenToViewportPoint(new Vector2(50, 50));
        if (rt.anchorMin.x < v.x)
            rt.anchorMin = rt.anchorMax = new Vector2(v.x, rt.anchorMax.y);
        else if (rt.anchorMax.x > 1 - v.x)  // 1 is the screen width (i.e., the rightmost point on x axis in viewport space)
            rt.anchorMin = rt.anchorMax = new Vector2(1 - v.x, rt.anchorMax.y);
    }

    public void DisplayCosts(BuildingTypes building)
    {
        if (building == null)
            Disable();

        this.gameObject.SetActive(true);
        objectName.text = building.name;
        costText.text = building.cost.ToString();
        timeText.text = TimeToString(building.buildTime);
    }

    public void Disable()
    {
        this.gameObject.SetActive(false);
    }

    private string TimeToString(int t)
    {
        int h = t / 60, m = t % 60; // hours and minutes
        return (h + "h " + m + "m");
    }
}
