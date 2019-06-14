using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Statistics : MonoBehaviour {

    [SerializeField]
    Text _timePlayed;
    DateTime startTime = DateTime.MinValue;
    DateTime timePlayed;

    [SerializeField]
    Text _moneyMade;
    int moneyMade;
    
    [SerializeField]
    Text _moneyLost;
    int moneyLost;

    [SerializeField]
    Text _lastMonthProfit;
    int lastMonthProfit;

    [SerializeField]
    Text _lastYearProfit;
    int lastYearProfit;

    [SerializeField]
    Text _robberies;
    int robberies;

    [SerializeField]
    Text _robbersCaught;
    int robbersCaught;

    [SerializeField]
    Text _illPeople;
    int illPeople;

    [SerializeField]
    Text _illRecovered;
    int illRecovered;

    [SerializeField]
    Text _fires;
    int fires;

    [SerializeField]
    Text _firesPutOut;
    int firesPutOut;

    [SerializeField]
    Text _earthquakes;
    int earthquakes;

    int currentMonthlyProfit;
    int currentYearlyProfit;

    void Awake ()
    {
        startTime = DateTime.Now;
    }

    public DateTime TimePlayed
    {
        get { return timePlayed; }
        set
        {
            timePlayed = DateTime.Now;
            TimeSpan span = timePlayed - startTime;
            _timePlayed.text = string.Format("{0:00}:{1:00}:{2:00}", span.TotalHours, span.Minutes, span.Seconds);
        }
    }

    public int MoneyMade
    {
        get { return moneyMade; }
        set
        {
            moneyMade += value;
            currentMonthlyProfit += value;
            currentYearlyProfit += value;
            _moneyMade.text = moneyMade.ToString();
        }
    }

    public int MoneyLost
    {
        get { return moneyLost; }
        set
        {
            moneyLost += value;
            currentMonthlyProfit -= value;
            currentYearlyProfit -= value;
            _moneyLost.text = moneyLost.ToString();
        }
    }

    public int LastMonthProfit
    {
        get { return lastMonthProfit; }
        set
        {
            lastMonthProfit = currentMonthlyProfit;
            currentMonthlyProfit = 0;
            _lastMonthProfit.text = lastMonthProfit.ToString();
        }
    }

    public int LastYearProfit
    {
        get { return lastYearProfit; }
        set
        {
            lastYearProfit = currentYearlyProfit;
            currentYearlyProfit = 0;
            _lastYearProfit.text = lastYearProfit.ToString();
        }
    }

    public int Robberies { get { return robberies; } set { robberies = value; _robberies.text = robberies.ToString(); } }
    public int RobbersCaught { get { return robbersCaught; } set { robbersCaught = value; _robbersCaught.text = robbersCaught.ToString(); } }
    public int IllPeople { get { return illPeople; } set { illPeople = value; _illPeople.text = illPeople.ToString(); } }
    public int IllRecovered { get { return illRecovered; } set { illRecovered = value; _illRecovered.text = illRecovered.ToString(); } }
    public int Fires { get { return fires; } set { fires = value; _fires.text = fires.ToString(); } }
    public int FiresPutOut { get { return firesPutOut; } set { firesPutOut = value; _firesPutOut.text = firesPutOut.ToString(); } }
    public int Earthquakes { get { return earthquakes; } set { earthquakes = value; _earthquakes.text = earthquakes.ToString(); } }
}
