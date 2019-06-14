using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* A static class for central in-game time control
 * All variables are static and can be accessed via methods */

public class TimeController : MonoBehaviour
{
    public static float Time { get; private set; }
    private static float prevTimeScale; // a reference to previous time scale (such as before a pause), not the actual timeScale
    public static int Day { get; private set; }
    public static int Month { get; private set; }
    public static int Year { get; private set; }

    public static int MaxSpeed { get; private set; }

    private void Awake()
    {
        if(Year == 0)
            Year = 1970;

        if (Month == 0)
            Month = 1;
    }

    private void Start()
    {
        if (MaxSpeed <= 0)  // i.e., maxSpeed must be >= 1
            MaxSpeed = 100;
    }

    private void Update()
    {
        if ((Time += UnityEngine.Time.deltaTime) > (24 * 60)) // 1440 real life seconds (24 minutes)
        {
            Time -= (24 * 60);
            if((++Day) % 7 == 0)
            {
                if((++Month) % 12 == 0)
                {
                    Year++;
                }
            }
        }
    }

    public static void TogglePause()
    {
        /* The global variable prevTimeScale here is used to store the timeScale before a pause
         * This means resuming game sets the speed to the timeScale before pausing
         * E.g. Pausing when the Time Scale is 2 sets prevTimeScale = 2. On resume Time.timeScale = prevTimeScale */

        if (UnityEngine.Time.timeScale != 0.000001f)
            prevTimeScale = UnityEngine.Time.timeScale;

        SetTimeScale( (UnityEngine.Time.timeScale == 0.000001f) ? prevTimeScale : 0.000001f );
    }

    public static void ChangeSpeed(float speedFactor)
    {
        SetTimeScale(Mathf.Clamp(UnityEngine.Time.timeScale * speedFactor, 0.5f, MaxSpeed) );
    }

    public static void ResetSpeed()
    {
        SetTimeScale(1);
    }

    private static void SetTimeScale(float scale)
    {
        UnityEngine.Time.timeScale = scale;
        UnityEngine.Time.fixedDeltaTime = 0.02f * scale;
    }
}
