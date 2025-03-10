﻿using UnityEngine;
using System;
/// <summary>
/// Script which track the real world time
/// </summary>
/// ref to the tutorial for saving real time https://www.youtube.com/watch?v=Yoh6owRXCXA

public class AdsTimeTracker : MonoBehaviour
{
    public static AdsTimeTracker instance;

    private ulong lastAdsSeen; //stores the value when ad button is clicked
    [Tooltip("Its time between two consecutive ads in minute")]
    public int timeToWait = 15; //time in min to wait before the ads button can be clicked again
    [HideInInspector]
    public bool adsReady; //to keep trackis ads button is ready

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        lastAdsSeen = GameManager.instance.adsTime; //get the value saved on device

        if (!IsAdsReady())//check is ready or not
        {//if not the make button non - interactable
            UIObjects.instance.gameOverMenuUI.adsBtn.interactable = false;
            adsReady = false;
        }
        else
        {
            adsReady = true;
        }
    }

    void Update()
    {
        if (!UIObjects.instance.gameOverMenuUI.adsBtn.IsInteractable())
        {
            if (IsAdsReady())
            {
                adsReady = true;
                UIObjects.instance.gameOverMenuUI.adsBtn.interactable = true;
                return;
            }

            //Set timer
            ulong diff = (ulong)DateTime.Now.Ticks - lastAdsSeen;
            ulong milliSec = diff / TimeSpan.TicksPerMillisecond;
            //(1000 millisec = 1 second)
            //1st converted timeGap into seconds then converted milliseconds into seconds and subtracted
            float secondsLeft = (float)(timeToWait * 60) - milliSec / 1000;

            string r = "";
            ////hours (If wanted can add)
            //r += ((int)secondsLeft / 3600).ToString() + ":";
            //secondsLeft -= ((int)secondsLeft / 3600) * 3600;

            //min
            r += ((int)secondsLeft / 60).ToString("00") + ":";
            //sec
            r += (secondsLeft % 60).ToString("00");

            UIObjects.instance.gameOverMenuUI.adsTimer.text = r;
        }

    }

    public void TrackTime()
    {
        GameManager.instance.adsTime = (ulong)DateTime.Now.Ticks;
        lastAdsSeen = GameManager.instance.adsTime;
        GameManager.instance.Save();
    }

    private bool IsAdsReady()
    {
        ulong diff = (ulong)DateTime.Now.Ticks - lastAdsSeen;
        ulong milliSec = diff / TimeSpan.TicksPerMillisecond;
        //(1000 millisec = 1 second)
        //1st converted timeGap into seconds then converted milliseconds into seconds and subtracted
        float secondsLeft = (float)(timeToWait * 60) - milliSec / 1000;

        if (secondsLeft < 0)
        {
            UIObjects.instance.gameOverMenuUI.adsTimer.text = "Watch Ads to get +20";
            return true;
        }

        return false;

    }

}//class

