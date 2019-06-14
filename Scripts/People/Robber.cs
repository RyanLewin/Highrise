using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robber : Person {


    [Tooltip("Time it takes to shoot")] public float shootTimer = 3f;    
    float timer = 0.5f;

    [Tooltip("Number of bullets in the gun before reload")]
    int gunRounds = 6;
    public Bullet bullet;
    RectTransform healthBar;
    Quaternion healthRotation;

    
    public float health = 150;
    float currHealth;

    CameraController cameraController;
    GameObject gameController;
    Driving driving;
    public Vehicle vehicle;

    protected override void Awake()
    {
        base.Awake();
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (controller.GetComponent<CameraController>())
                cameraController = controller.GetComponent<CameraController>();
            else
                gameController = controller;
        driving = gameController.GetComponent<Driving>();
        currHealth = health;
        healthRotation = cameraController.driveCam.transform.rotation;
        healthBar = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    protected override void Update () {
        //Make sure theres a target
        if (target != null)
        {
            //Do the Person.cs update if not aiming at a cop, e.g. getting into the cop's car
            if (target == vehicle.transform)
                base.Update();
            //else move within a certain distance of the cop and shoot at him.
            else {
                agent.SetDestination(target.position);
                if (agent.hasPath)
                    if (agent.remainingDistance <= 15f)
                    {
                        agent.speed = 0f;
                        transform.LookAt(target);
                    }
                    else if (agent.remainingDistance > 15f)
                        agent.speed = 10f;

                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 75f))
                        if (hit.transform == target)
                            StartCoroutine(Shoot());
                }
            }
        }
        else
            agent.ResetPath();

        if (currHealth > 0)
            transform.GetChild(1).rotation = healthRotation;
    }

    IEnumerator Shoot()
    {
        //Create a vector around the aimed position and shoot at it
        float y = Random.Range(transform.eulerAngles.y - 15, transform.eulerAngles.y + 15);
        Vector3 shotAngle = new Vector3(transform.eulerAngles.x, y - 90, transform.eulerAngles.z);
        //Create a bullet and reduce the no of rounds in the gun
        Instantiate(bullet, transform.GetChild(0).position, Quaternion.Euler(shotAngle));
        gunRounds--;
        //reload
        if (gunRounds <= 0)
        {
            timer = shootTimer;
            gunRounds = 6;
        }
        yield return new WaitForSeconds(0.1f);
    }

    void OnTriggerEnter (Collider other)
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
            healthBar.localScale = new Vector3(currHealth / health, 1, 1); //Health bar
            if (currHealth <= 0)
            {
                transform.GetChild(1).gameObject.SetActive(false);      //Hide the health bar
                vehicle.totalPassengers++;
                gameObject.tag = "Untagged";                            //detag so that the next part works

                //If there are no other robbers, set the control to be the player
                if (!GameObject.FindGameObjectWithTag("Person/Robber"))
                {
                    driving.cop = false;                                //set the driving script to control the vehicle
                    Destroy(target.gameObject);                         //Destroy cop
                }

                house.EndRobbery(true);                                 //End robbery and tell it that the criminal was captured
                Destroy(transform.GetChild(0).gameObject);              //Destroy gun

                SetTarget(vehicle.transform);                           //Set target to vehicle
                agent.speed = 10f;                                      //Make sure the robber's speed isn't 0;
                if (agent.remainingDistance > 20f)                      //If the distance to the vehicle is too far, destroy the robber
                    Destroy(gameObject);
            }
        }
    }
}
