using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robber : Person {

    [Tooltip("Time it takes to shoot")]
    public float shootTimer = 3f;
    float timer = 2f;

    float health = 50;
    
    GameObject gameController;
    Driving driving;
    public Vehicle vehicle;

    void Start()
    {
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (!controller.GetComponent<CameraController>())
                gameController = controller;
        driving = gameController.GetComponent<Driving>();
    }

    // Update is called once per frame
    protected override void Update () {

        if (target != null)
        {
            if (target == vehicle.transform)
                base.Update();
            else {
                agent.SetDestination(target.position);
                if (agent.hasPath)
                    if (agent.remainingDistance <= 15f)
                        agent.speed = 0f;
                    else if (agent.remainingDistance > 15f)
                        agent.speed = 10f;

                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 50f))
                    {
                        if (hit.transform == target)
                        {
                            target.GetComponent<Cop>().Health -= 25;
                            Debug.Log("Cop Shot, health left: " + target.GetComponent<Cop>().Health);
                            timer = shootTimer;
                        }
                    }
                }
            }
        }
        else
            agent.ResetPath();
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
                house.EndRobbery(true);
                Destroy(target.gameObject);
                Destroy(transform.GetChild(0).gameObject);
                agent.speed = 10f;
                SetTarget(vehicle.transform);
            }
        }
    }
}
