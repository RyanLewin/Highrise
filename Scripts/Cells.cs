using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Cells {

    public int x, z;
    
    // Initialiser
    public Cells (int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static Cells operator + (Cells a, Cells b)
    {
        a.x += b.x;
        a.z += b.z;
        return a;
    }
}
