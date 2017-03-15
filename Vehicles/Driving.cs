using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driving : MonoBehaviour {
    
    public bool driving = false;

    public Transform vehicle;

    public Transform ambulance;
    public Transform fireEngine;
    public Transform policeCar;
    
    Vector3 direction;
    Vector3 targetDirection;

    CameraController cameraController;
    public GameObject copPrefab;

    public float moveSpeed = 15;
    public float rotateSpeed = 50;

    public bool cop = false;

    void Start ()
    {
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (controller.GetComponent<CameraController>())
                cameraController = controller.GetComponent<CameraController>();

        direction = transform.rotation.eulerAngles;
    }

    //void Update ()
    //{
    //    //if (Input.GetKeyUp(KeyCode.Escape) && vehicle != null)
    //    //{
    //    //    cameraController.ChangeCamera = true;
    //    //    driving = false;
    //    //    Destroy(vehicle.gameObject);
    //    //}
    //}

    void Update ()
    {
        if (driving && !cop)
        {
            if (Input.GetKeyDown(KeyCode.D))
                direction.z += 90;
            if (Input.GetKeyDown(KeyCode.A))
                direction.z -= 90;
        }
    }

    void FixedUpdate () {
        if (driving)
        {
            if (!cop)
            {
                Vector3 forwards = vehicle.right;
                if (Input.GetKey(KeyCode.W))
                    vehicle.GetComponent<Rigidbody>().AddForce(forwards * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
                if (Input.GetKey(KeyCode.S))
                    vehicle.GetComponent<Rigidbody>().AddForce(-forwards * (moveSpeed * Time.deltaTime), ForceMode.VelocityChange);

                vehicle.rotation = Quaternion.SlerpUnclamped(vehicle.rotation, Quaternion.Euler(direction), rotateSpeed * Time.deltaTime);
            }
            else
            {
                vehicle.GetChild(0).position = new Vector3(copPrefab.transform.position.x, vehicle.GetChild(0).position.y, copPrefab.transform.position.z);
                vehicle.GetChild(0).eulerAngles = new Vector3(90, 0, -90);
            }
        }
	}

    public void SpawnVehicle(GameObject building)
    {
        Time.timeScale = 1;
        Vector3 pos = building.transform.GetChild(2).position;
        direction = new Vector3(-90, 0, 0);
        
        Transform v = building.tag == "Hospital" ? ambulance : building.tag == "PoliceStation" ? policeCar : fireEngine;
        vehicle = Instantiate(v, pos, building.transform.GetChild(2).rotation);
        cameraController.ChangeCamera = true;
    }
}
