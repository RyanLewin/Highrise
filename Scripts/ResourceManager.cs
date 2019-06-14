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

    public TimeController timeController;
    float time;
    
    void Update ()
    {
        time += Time.deltaTime;
        if (time > 30)
        {
            time = 0;
            if (happiness < 100 && happiness < maxHappiness - 1)
                happiness += 2;
            else if (happiness > 100)
                happiness = 100;
            if (happiness < 0)
                happiness = 0;
            if (maxHappiness < 0)
                maxHappiness = 0;
        }
    }
}
