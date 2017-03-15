using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DriveUI_Controller : MonoBehaviour {
    Canvas canvas;
    Image illIconPrefab;

    public GameObject compass;
    Vector3 compassDir;
    Vector3 target;
    float angle;

    public List<GameObject> illHouses;
    public List<GameObject> robbedHouses;

    void Awake()
    {
        canvas = this.GetComponent<Canvas>();
    }

    void Update ()
    {
        if (illHouses != null)
        {
            compassDir = target - transform.position;
            compassDir.y = compass.transform.position.y;
            Quaternion qTo = Quaternion.LookRotation(compassDir, Vector3.up);
            compass.transform.rotation = Quaternion.RotateTowards(compass.transform.rotation, qTo, 90f);
            compass.transform.eulerAngles = new Vector3(0, 0, compass.transform.eulerAngles.z);
        }
        else
            compass.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void AddToIll(GameObject house)
    {
        illHouses.Add(house);
        target = illHouses[0].transform.position;
        target.y = transform.position.y;
    }

    public void RemoveFromList(GameObject house)
    {
        illHouses.Remove(house);
    }

    Vector3 ScreenClamp(Vector3 pos)
    {
        float x = pos.x, y = pos.y;
        if (x > /*drivingCanvas.transform.position.z + */(canvas.pixelRect.width / 2 - 15))
            x = canvas.pixelRect.width - 15;
        if (x < /*drivingCanvas.transform.position.z - */(canvas.pixelRect.width / 2 + 15))
            x = canvas.pixelRect.width + 15;
        if (y > /*drivingCanvas.transform.position.x + */(canvas.pixelRect.height / 2 - 15))
            y = canvas.pixelRect.height - 15;
        if (y > /*drivingCanvas.transform.position.x - */(canvas.pixelRect.height / 2 + 15))
            y = canvas.pixelRect.height + 15;
        return new Vector3(y, x, 0);
    }
}
