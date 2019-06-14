using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Driving : MonoBehaviour {
    
    public bool driving = false;

    public Transform vehicle;
    public Transform hose;
    public Canvas canvas;

    public Transform ambulance;
    public Transform fireEngine;
    public Transform policeCar;
    
    Vector3 direction;

    CameraController cameraController;
    public GameObject copPrefab;

    public float moveSpeed = 15;
    public float rotateSpeed = 50;

    public bool cop = false;
    public bool inrange = false;
    Vector3 lookAt;

    void Start ()
    {
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (controller.GetComponent<CameraController>())
                cameraController = controller.GetComponent<CameraController>();

        direction = transform.rotation.eulerAngles;
    }
    
    void Update ()
    {
        if (driving && !cop)
        {
            if (Input.GetKeyDown(KeyCode.D))
                ChangeDirection(Direction.East);
            if (Input.GetKeyDown(KeyCode.S))
                ChangeDirection(Direction.South);
            if (Input.GetKeyDown(KeyCode.A))
                ChangeDirection(Direction.West);
            if (Input.GetKeyDown(KeyCode.W))
                ChangeDirection(Direction.North);
        }
    }

    void FixedUpdate () {
        if (driving)
        {
            if (!cop)
            {
                vehicle.rotation = Quaternion.SlerpUnclamped(vehicle.rotation, Quaternion.Euler(direction), rotateSpeed * Time.deltaTime);
                cameraController.driveCam.transform.position = new Vector3(vehicle.transform.position.x, cameraController.driveCam.transform.position.y, vehicle.transform.position.z);
                cameraController.mapCam.transform.position = new Vector3(vehicle.transform.position.x, cameraController.mapCam.transform.position.y, vehicle.transform.position.z);

                Vector3 forwards = vehicle.right;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A))
                    vehicle.GetComponent<Rigidbody>().velocity = forwards * moveSpeed;

                if (inrange)
                {
                    #region look at mouse
                    //Creates a Vector point under the mouse that cop will rotate to look at
                    Vector3 v = Input.mousePosition;
                    v.z = 0;
                    lookAt = cameraController.driveCam.ScreenToWorldPoint(v);
                    lookAt.y = hose.position.y;
                    hose.LookAt(lookAt);
                    //locks rotation in the y axis
                    hose.eulerAngles = new Vector3(-90, hose.eulerAngles.y - 90, 0);
                    #endregion
                }
            }
            else
            {
                cameraController.driveCam.transform.position = new Vector3(copPrefab.transform.position.x, cameraController.driveCam.transform.position.y, copPrefab.transform.position.z);
            }
        }
	}

    void ChangeDirection (Direction dir)
    {
        switch (dir)
        {
            case (Direction.North):
                direction = new Vector3(-90, 0, 90);
                break;
            case (Direction.East):
                direction = new Vector3(-90, 0, 180);
                break;
            case (Direction.South):
                direction = new Vector3(-90, 0, 270);
                break;
            case (Direction.West):
                direction = new Vector3(-90, 0, 0);
                break;
        }
    }

    public bool InRange
    {
        get { return inrange; }
        set
        {
            inrange = value;
            if (inrange)
                hose.GetChild(0).gameObject.SetActive(true);
            else
                hose.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void SpawnVehicle(GameObject building)
    {
        Time.timeScale = 1;
        Vector3 pos = building.transform.GetChild(2).position;
        direction = new Vector3(-90, 0, 0);
        
        Transform v = building.tag == "Hospital" ? ambulance : building.tag == "PoliceStation" ? policeCar : fireEngine;
        vehicle = Instantiate(v, pos, building.transform.GetChild(2).rotation);
        if (vehicle.tag == "Vehicle/FireEngine")
            hose = vehicle.GetChild(0);
        cameraController.ChangeCamera = true;
        canvas.enabled = true;
    }
}
