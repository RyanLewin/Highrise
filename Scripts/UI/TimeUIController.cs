using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUIController : MonoBehaviour
{
    public Text dayDisplay, timeDisplay, monthDisplay, yearDisplay, multiplierDisplay;

    public Driving driving;
    [SerializeField]
    Statistics statistics;

    private void Start()
    {
        /* Attempt to find any references that have not been set in the inspector.
         * If you encounter any issues it is likely due to one of these references being incorrect
         * Potential fixes: correct the reference in the inspector 
         * OR ensure s in transform.Find(string s) matches a component in the scene heirachy */
         
        if (!dayDisplay)
            dayDisplay = transform.Find("Day").GetComponent<Text>();

        if (!timeDisplay)
            timeDisplay = transform.Find("Time").GetComponent<Text>();

        if (!monthDisplay)
            monthDisplay = transform.Find("Month").GetComponent<Text>();

        if (!yearDisplay)
            yearDisplay = transform.Find("Year").GetComponent<Text>();

        if (!multiplierDisplay)
            multiplierDisplay = transform.Find("Time Multiplier").GetComponent<Text>();
    }

    private void Update()
    {
        DisplayTime();
        KeyUpdate(); // Update the UI based on actions via keyboard input
    }

    void KeyUpdate()
    {
        if (!driving.driving)
        {
            if (Input.GetKeyUp(KeyCode.Alpha0))
                TimeController.TogglePause();
            else if (Input.GetKeyUp(KeyCode.Alpha1))
                TimeController.ResetSpeed();
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                TimeController.ChangeSpeed(0.5f);
                DisplayMultiplier();
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                TimeController.ChangeSpeed(10);
                DisplayMultiplier();
            }
        }
    }

    private void DisplayTime()
    {
        int time = Mathf.FloorToInt(TimeController.Time); // time is always rounded down
        timeDisplay.text = (time / 60).ToString("00") + ":" + (time % 60).ToString("00");

        if(time < 60)
            DisplayDay();
    }

    private void DisplayDay()
    {
        string d = "";

        switch (TimeController.Day % 7)
        {
            case 1: d = "MON"; DisplayMonth(); break; //Update month every week - couldn't be bothered waiting any longer
            case 2: d = "TUES"; break;
            case 3: d = "WED"; break;
            case 4: d = "THURS"; break;
            case 5: d = "FRI"; break;
            case 6: d = "SAT"; break;
            case 0:
            default: d = "SUN"; break;
        }

        dayDisplay.text = d;
    }

    private void DisplayMonth()
    {
        string m = "";
        statistics.LastMonthProfit = 0;

        switch (TimeController.Month % 12)
        {
            case 1: m = "JAN"; DisplayYear(); break; //Update year every 12 months
            case 2: m = "FEB"; break;
            case 3: m = "MARCH"; break;
            case 4: m = "APRIL"; break;
            case 5: m = "MAY"; break;
            case 6: m = "JUNE"; break;
            case 7: m = "JULY"; break;
            case 8: m = "AUG"; break;
            case 9: m = "SEP"; break;
            case 10: m = "OCT"; break;
            case 11: m = "NOV"; break;
            case 0:
            default: m = "DEC"; break;
        }

        monthDisplay.text = m;
    }

    private void DisplayYear()
    {
        yearDisplay.text = TimeController.Year.ToString();
        statistics.LastYearProfit = 0;
    }

    public void PauseGame()
    {
        TimeController.TogglePause();
        DisplayMultiplier();
    }

    public void ChangeSpeed(float speedFactor)
    {
        TimeController.ChangeSpeed(speedFactor);
        DisplayMultiplier();
    }

    public void ResetSpeed()
    {
        TimeController.ResetSpeed();
        DisplayMultiplier();
    }

    private void DisplayMultiplier()
    {
        multiplierDisplay.text = "x" + ((Time.timeScale == 0.000001f) ? 0 : Time.timeScale);
    }
    
    public void ToggleTimeUIAnimation(Animator anim)
    {
        anim.SetBool("hide", !anim.GetBool("hide"));
    }
    
    public string GetTime
    {
        get
        {
            return timeDisplay.text + ", " + dayDisplay.text + ", " + monthDisplay.text + " " + yearDisplay.text;
        }
    }

    private void TryFinding<T>(out T var, string s)
    {
        var = transform.Find(s).GetComponent<T>();
    }
}
