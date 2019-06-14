using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObject : MonoBehaviour
{
    public Building building;
    public GameObject selectUI;
    private int counter = 0;

    private void Start()
    {
        selectUI.SetActive(false);     // by default selectUI is not visible
    }

    private void Update()
    {
        if (building.obj != 0)  // something is being placed so cannot select an object whilst another is being placed
        {
            selectUI.SetActive(false);
            return;
        }

        if (Input.GetMouseButtonUp(0))  // the user clicked and wants to select something
        {
            Debug.Log(counter++);
            RaycastHit hit;         // store the result of the raycast we're about to do
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // prepare a ray going from camera to mouse position

            if (Physics.Raycast(ray, out hit))  // shoot the ray and store result in hit (ln 16)
            {
                selectUI.SetActive(true);
            }
        }
    }
}
