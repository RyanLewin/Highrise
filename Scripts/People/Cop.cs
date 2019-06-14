using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cop : MonoBehaviour {

    public float speed = 15f;

    public float health = 300f;
    float currHealth;
    
    CameraController cameraController;
    GameObject gameController;
    Driving driving;
    public DriveUI_Controller driveUI;
    public House house;

    public float timer = 0;
    public float shootTimer = 3f;
    int gunRounds = 6;
    public Bullet bullet;
    RectTransform healthBar;
    Quaternion healthRotation;

    Vector3 lookAt;

    void Start ()
    {
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (controller.GetComponent<CameraController>())
                cameraController = controller.GetComponent<CameraController>();
            else
                gameController = controller;
        driving = gameController.GetComponent<Driving>();
        currHealth = health;
        healthRotation = Quaternion.Euler(0,180,0) * cameraController.driveCam.transform.rotation;
        healthBar = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        driveUI = driving.canvas.GetComponent<DriveUI_Controller>();
        driveUI.Cop(true);
    }

    public void Update ()
    {
        //Force gun reload
        if (Input.GetKey(KeyCode.R))
        {
            timer = shootTimer;
            gunRounds = 6;
            driveUI.Bullets = gunRounds;
        }

        //if clicked and reload timer is below 0
        timer -= Time.deltaTime;
        if (Input.GetMouseButtonUp(0))
            if (timer <= 0)
                Shoot();
        
        if (currHealth > 0)
            transform.GetChild(1).rotation = healthRotation;
    }

    void Shoot ()
    {
        //Create a vector around the aimed position and shoot at it
        float y = Random.Range(transform.eulerAngles.y - 15, transform.eulerAngles.y + 15);
        Vector3 shotAngle = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
        //Create a bullet and reduce the no of rounds in the gun
        Instantiate(bullet, transform.GetChild(0).position, Quaternion.Euler(shotAngle));
        gunRounds--;
        //reload
        if (gunRounds <= 0)
        {
            timer = shootTimer;
            gunRounds = 6;
        }
        driveUI.Bullets = gunRounds;
    }
    
    void FixedUpdate ()
    {
        #region look at mouse
        //Creates a Vector point under the mouse that cop will rotate to look at
        Vector3 v = Input.mousePosition;
        v.z = 0;
        lookAt = cameraController.driveCam.ScreenToWorldPoint(v);
        lookAt = new Vector3(lookAt.x, 2, lookAt.z);
        transform.LookAt(lookAt);
        //locks rotation in the y axis
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y - 90, 0);
        #endregion

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(-moveHorizontal, 0.0f, -moveVertical);
        GetComponent<Rigidbody>().velocity = movement * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        //If a bullet hits, reduce health
        if (currHealth > 0)
            if (other.transform.tag == "Bullet")
                Health -= 25;
    }

    public float Health
    {
        get { return currHealth; }
        set
        {
            currHealth = value;
            healthBar.localScale = new Vector3(currHealth / health, 1, 1);
            if (currHealth <= 0)
            {
                driving.cop = false;        //set the driving script to control the vehicle
                house.EndRobbery(false);    //End robbery and tell it that the criminal escaped
                Destroy(gameObject);        //Destroy cop
            }
        }
    }

    void OnDestroy ()
    {
        //Reset camera position to above the vehicle
        cameraController.driveCam.transform.localPosition = new Vector3(0, 0, 1);
        driveUI.Cop(false); //Hides the cop gun UI
    }
}
