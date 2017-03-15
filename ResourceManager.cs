using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

    public bool creative = false;
    public bool paused = false;
    public int population = 0;
    public int workingPopulation = 0;
    public int maxPopulation;
    public int money = 500;

    [Range(0,100)]
    public float happiness = 100;
    public float maxHappiness = 100;

    //Time
    public float minutes = 0;
    public int hour = 0;
    public int day = 0;

    Driving driving;

    void Start ()
    {
        driving = transform.GetComponent<Driving>();
    }

    void Update ()
    {
        minutes += Time.deltaTime;
        if (minutes > 60)
        {
            minutes = 0;
            hour++;
            if (hour == 24)
            {
                day++;
                hour = 0;
            }

            if (happiness < 100 && happiness < maxHappiness - 1)
                happiness += 2;
            else if (happiness > 100)
                happiness = 100;
            if (happiness < 0)
                happiness = 0;
        }


        if (!driving.driving)
        {
            if (Input.GetKeyUp(KeyCode.Alpha0))
            {
                Time.timeScale = 0.0000001f;
                paused = true;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                paused = false;
                Time.timeScale = 3f;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                paused = false;
                Time.timeScale = 20f;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                paused = false;
                Time.timeScale = 60f;
            }
        }
    }

    public void UpdateTime (int speed)
    {
        switch (speed)
        {
            case (0):
                Time.timeScale = 0.0000001f;
                paused = true;
                break;
            case (1):
                paused = false;
                Time.timeScale = 1f;
                break;
            case (2):
                paused = false;
                Time.timeScale = 5f;
                break;
            case (3):
                paused = false;
                Time.timeScale = 20f;
                break;
        }
    }
}
