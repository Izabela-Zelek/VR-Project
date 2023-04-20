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
    [SerializeField]
    private TextMeshProUGUI wordDayText;

    public DateTime currentTime;
    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;
    public int dayNr = 1;

    private GameObject _grass1;
    private GameObject _grass2;
    private Transform _grassParent;
    private AudioSource _birdsAmb;
    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
        dayText.text = "Day " + dayNr.ToString();
        DecideOnDay();
        _grass1 = Resources.Load("Grass_1_1") as GameObject;
        _grass2 = Resources.Load("Grass_1_2") as GameObject;
        _grassParent = GameObject.Find("TownDecor").transform.GetChild(3).transform;
        _birdsAmb = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        RotateSun();
        UpdateSettings();

        if(currentTime.Hour > sunriseHour - 1 && currentTime.Hour < sunsetHour - 1)
        {
            if(!_birdsAmb.isPlaying)
            {
                _birdsAmb.Play();
            }
        }
        else
        {
            if(_birdsAmb.isPlaying)
            {
                _birdsAmb.Stop();
            }
        }
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
        DecideOnDay();
        Spawn();
    }

    private void DecideOnDay()
    {
        string dayOfWeek = "";

        switch(dayNr % 7)
        {
            case 0:
                dayOfWeek = "SUNDay";
                break;
            case 1:
                dayOfWeek = "MONDay";
                break;
            case 2:
                dayOfWeek = "TUESDay";
                break;
            case 3:
                dayOfWeek = "WEDNESDay";
                break;
            case 4:
                dayOfWeek = "THURSDay";
                break;
            case 5:
                dayOfWeek = "FRIDay";
                break;
            case 6:
                dayOfWeek = "SaTURDay";
                break;
        }

        wordDayText.text = dayOfWeek.ToString();
    }

    public int GetDayOfWeek()
    {
        return dayNr % 7;
    }

    private void Spawn()
    {

        int randNum = UnityEngine.Random.Range(5, 9);

        for (int i = 0; i < randNum; i++)
        {
            int rand = UnityEngine.Random.Range(1, 3);
            float xValue = UnityEngine.Random.Range(-40, 56);
            float zValue = UnityEngine.Random.Range(-24, 101);
            switch (rand)
            {
                case 1:
                    Instantiate(_grass1, new Vector3(xValue, 0, zValue), Quaternion.identity, _grassParent);
                    break;
                case 2:
                    Instantiate(_grass2, new Vector3(xValue, 0, zValue), Quaternion.identity, _grassParent);
                    break;
            }
        }
    }
}
