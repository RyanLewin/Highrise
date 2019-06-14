using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North,
    East,
    South,
    West,
    None
}

public static class Directions
{
    public const int count = 9;

    private static Quaternion[] rotations =
    {
        Quaternion.Euler(0,0,0),
        Quaternion.Euler(0, 90f, 0),
        Quaternion.Euler(0, 180f, 0),
        Quaternion.Euler(0, 270f, 0),
        Quaternion.Euler(0,0,0)
    };

    private static Direction[] opposites =
    {
        Direction.South,
        Direction.West,
        Direction.North,
        Direction.East,
        Direction.None
    };

    //Return Euler rotation of direction
    public static Quaternion ToRotation (this Direction direction)
    {
        return rotations[(int)direction];
    }

    //Return opposite direction
    public static Direction GetOpposite (this Direction direction)
    {
        return opposites[(int)direction];
    }
}
