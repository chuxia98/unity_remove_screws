using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AnyThinkAds.Api;

public abstract class BaseAdOperator
{

    // public Text statusText;
    public event EventHandler<string> statusChangeEvent;
    public event EventHandler<float> retryLoadAdAttemptEvent;
    public int retryAdAttemptCount;

    abstract public void initializeAd();

    abstract public void destroyAd();

    abstract public void loadAd();

    abstract public void showAd();


    public void setStatusText(string text)
    {
        // statusText.text = text;
        statusChangeEvent?.Invoke(this, text);
    }

    public void setLoading(string status = "Loading...") 
    {
        setStatusText(status);
    }

    public void setLoadSuccess(string status = "Load succeed.")
    {
        setAdReadyStatus(true);
        retryAdAttemptCount = 0;
    }

    public void setLoadFailed(ATAdErrorEventArgs args, string status = "Load failed.")
    {
        setStatusText(status);
    }

    public void retryAdAttempt()
    {
         //ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        retryAdAttemptCount++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAdAttemptCount));
        // Debug.Log("setLoadFailed() >>> retryDelay: " + retryDelay + " retryLoadAdAttemptEvent: " + retryLoadAdAttemptEvent);
        retryLoadAdAttemptEvent?.Invoke(this, (float)retryDelay);
    }

    public void setAdReadyStatus(bool isReady)
    {
        string text = isReady ? "Ad is ready." : "Ad not ready.";
        setStatusText(text);
    }
}