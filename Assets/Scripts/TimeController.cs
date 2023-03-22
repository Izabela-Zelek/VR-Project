using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float TimeMultiplier; 
    [SerializeField]
    private float startHour;
    [SerializeField]
    private Light sun;
    [SerializeField]
    private Light moon;
    [SerializeField]
    private float sunriseHour;
    [SerializeField]
    private float sunsetHour;
    [SerializeField]
    private Color dayAmbientLight;
    [SerializeField]
    private Color nightAmbientLight;
    [SerializeField]
    private AnimationCurve lightChange;
    [SerializeField]
    private float maxSunIntensity;    
    [SerializeField]
    private float maxMoonIntensity;
    [SerializeField]
    private TextMeshProUGUI dayText;
    [SerializeField]
    private TextMeshProUGUI timeText;

    public DateTime currentTime;
    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;
    public int dayNr = 1;

    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
        dayText.text = "Day " + dayNr.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        RotateSun();
        UpdateSettings();
    }

    private void UpdateTime()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * TimeMultiplier);

       timeText.text =  currentTime.ToString("HH:mm");
    }
    private TimeSpan CalculateTime(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        if(difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }
        return difference;
    }
    private void RotateSun()
    {
        float sunLightRotation;
        if(currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTime(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTime(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;
            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTime(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTime(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;
            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }
        sun.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    void UpdateSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightChange.Evaluate(dotProduct));
        moon.intensity = Mathf.Lerp(maxMoonIntensity, 0, lightChange.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChange.Evaluate(dotProduct));
    }

    public void newDay()
    {
        dayNr += 1;
        dayText.text = "Day " + dayNr.ToString();
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
    }
}
