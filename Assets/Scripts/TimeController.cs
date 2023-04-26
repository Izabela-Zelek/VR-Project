using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
/// <summary>
/// Handles the passage of time, changing day to night and night to day, and random grass growing each day
/// </summary>
public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float _timeMultiplier; 
    [SerializeField]
    private float _startHour;
    [SerializeField]
    private Light _sun;
    [SerializeField]
    private Light _moon;
    [SerializeField]
    private float _sunriseHour;
    [SerializeField]
    private float _sunsetHour;
    [SerializeField]
    private Color _dayAmbientLight;
    [SerializeField]
    private Color _nightAmbientLight;
    [SerializeField]
    private AnimationCurve _lightChange;
    [SerializeField]
    private float _maxSunIntensity;    
    [SerializeField]
    private float _maxMoonIntensity;
    [SerializeField]
    private TextMeshProUGUI _dayText;
    [SerializeField]
    private TextMeshProUGUI _timeText;
    [SerializeField]
    private TextMeshProUGUI _wordDayText;

    public DateTime CurrentTime;
    public int DayNr = 1;

    private TimeSpan _sunriseTime;
    private TimeSpan _sunsetTime;
    private GameObject _grass1;
    private GameObject _grass2;
    private Transform _grassParent;
    private AudioSource _birdsAmb;

    private void Start()
    {
        CurrentTime = DateTime.Now.Date + TimeSpan.FromHours(_startHour);
        _sunriseTime = TimeSpan.FromHours(_sunriseHour);
        _sunsetTime = TimeSpan.FromHours(_sunsetHour);
        _dayText.text = "Day " + DayNr.ToString();
        DecideOnDay();
        _grass1 = Resources.Load("Grass_1_1") as GameObject;
        _grass2 = Resources.Load("Grass_1_2") as GameObject;
        _grassParent = GameObject.Find("TownDecor").transform.GetChild(3).transform;
        _birdsAmb = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Updates the current time, moves sun across the sky and plays ambient sounds of birds singing
    /// </summary>
    private void Update()
    {
        UpdateTime();
        RotateSun();
        UpdateSettings();

        if(CurrentTime.Hour > _sunriseHour - 1 && CurrentTime.Hour < _sunsetHour - 1)
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

    /// <summary>
    /// Updates the time based on real time passing multiplied by a speed multiplier
    /// Updates the text on the watch
    /// </summary>
    private void UpdateTime()
    {
        CurrentTime = CurrentTime.AddSeconds(Time.deltaTime * _timeMultiplier);

       _timeText.text =  CurrentTime.ToString("HH:mm");
    }
    /// <summary>
    /// Calculates the time between two given times
    /// </summary>
    /// <param name="fromTime"></param>
    /// <param name="toTime"></param>
    /// <returns></returns>
    private TimeSpan CalculateTime(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        if(difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }
        return difference;
    }
    /// <summary>
    /// Moves the sun across the sky and changes the light level of the world based on the position of the sun
    /// </summary>
    private void RotateSun()
    {
        float sunLightRotation;
        if(CurrentTime.TimeOfDay > _sunriseTime && CurrentTime.TimeOfDay < _sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTime(_sunriseTime, _sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTime(_sunriseTime, CurrentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;
            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTime(_sunsetTime, _sunriseTime);
            TimeSpan timeSinceSunset = CalculateTime(_sunsetTime, CurrentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;
            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }
        _sun.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    /// <summary>
    /// Changes the light intensity of the sun and moon based on the sun position
    /// </summary>
    private void UpdateSettings()
    {
        float dotProduct = Vector3.Dot(_sun.transform.forward, Vector3.down);
        _sun.intensity = Mathf.Lerp(0, _maxSunIntensity, _lightChange.Evaluate(dotProduct));
        _moon.intensity = Mathf.Lerp(_maxMoonIntensity, 0, _lightChange.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(_nightAmbientLight, _dayAmbientLight, _lightChange.Evaluate(dotProduct));
    }

    /// <summary>
    /// Updates the daynr to a new day
    /// </summary>
    public void NewDay()
    {
        DayNr += 1;
        _dayText.text = "Day " + DayNr.ToString();
        CurrentTime = DateTime.Now.Date + TimeSpan.FromHours(_startHour);
        DecideOnDay();
        Spawn();
    }

    /// <summary>
    /// Decides on which day of the week it is based on the day nr, updates the watch
    /// </summary>
    private void DecideOnDay()
    {
        string dayOfWeek = "";

        switch(DayNr % 7)
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

        _wordDayText.text = dayOfWeek.ToString();
    }

    public int GetDayOfWeek()
    {
        return DayNr % 7;
    }

    /// <summary>
    /// Spawns a random amount of grass within game bounds
    /// </summary>
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
