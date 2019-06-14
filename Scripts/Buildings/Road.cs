using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Road: BuildingTypes{

    //cost - subject to change, just a placeholder
    public int roadCost = 2; //wood + stone + metal

    protected override void Awake()
    {
        base.Awake();
        cost = roadCost;
    }
}
