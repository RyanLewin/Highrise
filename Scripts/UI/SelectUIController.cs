using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUIController : MonoBehaviour
{
    RaycastHit hit;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit " + hit.transform.name);
            }
        }
    }
}
