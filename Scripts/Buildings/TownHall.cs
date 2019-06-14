using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownHall : BuildingTypes {
    
    public int townHallCost = 510;

    protected override void Awake()
    {
        base.Awake();
        cost = townHallCost;
        xMax = 10;
        zMax = 5;
    }
}
