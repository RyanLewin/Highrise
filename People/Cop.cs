using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cop : MonoBehaviour {

    public float speed = 15f;

    public float health = 100f;
    CameraController cameraController;
    GameObject gameController;
    Driving driving;
    public House house;

    public float timer = 0;
    public float shootTimer = 2.5f;

    Vector3 lookAt;

    void Start ()
    {
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (controller.GetComponent<CameraController>())
                cameraController = controller.GetComponent<CameraController>();
            else
                gameController = controller;
        driving = gameController.GetComponent<Driving>();
    }

    public void Update ()
    {
        timer -= Time.deltaTime;
        if (Input.GetMouseButtonUp(0))
        {
            if (timer <= 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Quaternion.Euler(0, 90, 0) * transform.forward, out hit, 1000f))
                {
                    if (hit.transform.name == "Robber(Clone)")
                    {
                        hit.transform.GetComponent<Robber>().Health -= 25;
                        Debug.Log("Robber Shot, health left: " + hit.transform.GetComponent<Robber>().Health);
                        timer = 1f;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        #region look at mouse
        Vector3 v = Input.mousePosition;
        v.z = 0;
        lookAt = cameraController.driveCam.ScreenToWorldPoint(v);
        lookAt = new Vector3(lookAt.x, 2, lookAt.z);
        transform.LookAt(lookAt);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y - 90, 0);
        #endregion

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveVertical, 0.0f, -moveHorizontal);
        GetComponent<Rigidbody>().velocity = movement * speed;
    }

    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
            {
                driving.cop = false;
                house.EndRobbery(false);
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy ()
    {
        cameraController.driveCam.transform.localPosition = new Vector3(0, 0, 1);
        cameraController.driveCam.transform.localEulerAngles = new Vector3(180, 0, -90);
    }
}
