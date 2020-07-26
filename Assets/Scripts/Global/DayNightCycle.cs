using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public Light worldLight;
    public float timeSpeed;
    float realTimeSpeed;
    float originalTimeSpeed;
    public float currentTimeSpeed;
    public float[] speedControls;
    public int sunsetHour;
    public int sunriseHour;
    public int currentHour;
    public float currentMinute;

    public bool day;
    
    // Start is called before the first frame update
    void Start()
    {
        // Sets the inital speed of time
        originalTimeSpeed = timeSpeed;
        if (speedControls.Length > 0)
        {
            speedControls[0] = 1;
            currentTimeSpeed = speedControls[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Sets the current speed
        realTimeSpeed = timeSpeed / 100;
        currentMinute += realTimeSpeed * currentTimeSpeed;

        // Changes the hour once the minute hits 60
        if (int.Parse(currentMinute.ToString("F0")) >= 60)
        {
            currentHour++;
            currentMinute = 0;
        }

        // Rolls over onto the next day
        if (currentHour >= 24) currentHour = 0;

        // Shows the current type as an integer for the internal clock
        string hour = currentHour.ToString("F0");
        if (currentHour < 10) hour = hour.Insert(0, "0");
        string minute = currentMinute.ToString("F0");
        if (int.Parse(minute) < 10) minute = minute.Insert(0, "0");
        GetComponent<Text>().text = hour + ":" + minute;

        // Changes from day to night based on the current hour
        if (currentHour >= sunriseHour && currentHour < sunsetHour) day = true;
        else day = false;

        // Lowers or increases the brightness if it's day or night
        if (day && worldLight.intensity < 1) worldLight.intensity += (realTimeSpeed / 10) / 2;
        if (!day && worldLight.intensity > 0) worldLight.intensity -= (realTimeSpeed / 10) / 2;

        worldLight.intensity = Mathf.Clamp(worldLight.intensity, 0, 1);

        ChangeSpeed();
    }

    void ChangeSpeed()
    {
        // Changes time speed base on key input (numbers) and time multiplier in speedControls array
        int buttonPressed = 0;
        int.TryParse(Input.inputString, out buttonPressed);
        if (buttonPressed > 0 && buttonPressed <= speedControls.Length)
        {
            currentTimeSpeed = speedControls[buttonPressed - 1];
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentTimeSpeed == 0) currentTimeSpeed = 1;
            else currentTimeSpeed = 0;
        }
    }
}
