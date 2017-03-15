using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    private static float time, tScale;  // make this private
    private int gameSpeed, day;

    [SerializeField]
    private Text dayDisplay, timeDisplay, multiplierDisplay;

    //Test Stuff
    public Light mainLight;

    private void Start()
    {
        time = day = 0;
        gameSpeed = 1;
        UpdateDay();
    }

    private void Update()
    {
        DisplayTime();

        mainLight.transform.RotateAround(mainLight.transform.position, Vector3.right, (float)0.25 * Time.deltaTime);
    }

    private void DisplayTime()
    {
        if ((time += Time.deltaTime) > (24 * 60)) // Incremenets time and then checks whether it has passed 24 hours
            UpdateDay();

        int t = Mathf.FloorToInt(time); // time is always rounded down
        timeDisplay.text = (t / 60).ToString("00") + ":" + (t % 60).ToString("00");
    }

    private void UpdateDay()
    {
        string d = "";
        day++;
        time = 0;

        switch(day % 7)
        {
            case 1:     d = "MON";  break;
            case 2:     d = "TUES"; break;
            case 3:     d = "WED";  break;
            case 4:     d = "THURS";break;
            case 5:     d = "FRI";  break;
            case 6:     d = "SAT";  break;
            case 0:
            default:    d = "SUN";  break;
        }

        dayDisplay.text = d;
    }

    public void PauseGame()
    {
        if (Time.timeScale != 0)
            tScale = Time.timeScale;

        Time.timeScale = (Time.timeScale == 0) ? tScale : 0;
        DisplayMultiplier();
    }

    public void ChangeSpeed(float speedFactor)
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale * speedFactor, 0.5f, 32);
        DisplayMultiplier();
    }

    public void ResetSpeed()
    {
        Time.timeScale = 1;
        DisplayMultiplier();
    }

    private void DisplayMultiplier()
    {
        multiplierDisplay.text = "x" + Time.timeScale;
    }
}
