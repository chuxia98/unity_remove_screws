using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AnyThinkAds.Api;
using UnityEngine.UI;
using System;

using AnyThinkAds.ThirdParty.LitJson;


public class InterstitialAdOperator : BaseAdOperator
{
    private static readonly InterstitialAdOperator instance = new InterstitialAdOperator();
    // public Button autoLoadButton;
    // public Button autoShowButton;
    // public Button removeAutoButton;
    // public Button isAutoReadyButton;

#if UNITY_ANDROID
    static string mPlacementId_interstitial_all = "n6944c0b3b6d44";
    static string showingScenario = "f5e71c49060ab3";

#elif UNITY_IOS || UNITY_IPHONE
    static string mPlacementId_interstitial_all = "b5bacad26a752a";
    static string showingScenario = "f5e549727efc49";

#endif

    private InterstitialAdOperator() 
	{
        
	}

	public static InterstitialAdOperator Instance 
	{
        get 
		{
			return instance;
		}
	}

    // Use this for initialization
    public override void initializeAd()
    {
        ATInterstitialAd.Instance.client.onAdLoadEvent += onAdLoad;
        ATInterstitialAd.Instance.client.onAdClickEvent += onAdClick;
        ATInterstitialAd.Instance.client.onAdCloseEvent += onAdClose;
        ATInterstitialAd.Instance.client.onAdShowEvent += onShow;
        ATInterstitialAd.Instance.client.onAdLoadFailureEvent += onAdLoadFail;
        ATInterstitialAd.Instance.client.onAdShowFailureEvent += onAdShowFail;
        ATInterstitialAd.Instance.client.onAdVideoStartEvent += startVideoPlayback;
        ATInterstitialAd.Instance.client.onAdVideoEndEvent += endVideoPlayback;
        ATInterstitialAd.Instance.client.onAdVideoFailureEvent += failVideoPlayback;
        ATInterstitialAd.Instance.client.onAdSourceAttemptEvent += startLoadingADSource;
        ATInterstitialAd.Instance.client.onAdSourceFilledEvent += finishLoadingADSource;
        ATInterstitialAd.Instance.client.onAdSourceLoadFailureEvent += failToLoadADSource;
        ATInterstitialAd.Instance.client.onAdSourceBiddingAttemptEvent += startBiddingADSource;
        ATInterstitialAd.Instance.client.onAdSourceBiddingFilledEvent += finishBiddingADSource;
        ATInterstitialAd.Instance.client.onAdSourceBiddingFailureEvent += failBiddingADSource;
    }

    public override void destroyAd()
    {
        ATInterstitialAd.Instance.client.onAdLoadEvent -= onAdLoad;
        ATInterstitialAd.Instance.client.onAdClickEvent -= onAdClick;
        ATInterstitialAd.Instance.client.onAdCloseEvent -= onAdClose;
        ATInterstitialAd.Instance.client.onAdShowEvent -= onShow;
        ATInterstitialAd.Instance.client.onAdLoadFailureEvent -= onAdLoadFail;
        ATInterstitialAd.Instance.client.onAdShowFailureEvent -= onAdShowFail;
        ATInterstitialAd.Instance.client.onAdVideoStartEvent -= startVideoPlayback;
        ATInterstitialAd.Instance.client.onAdVideoEndEvent -= endVideoPlayback;
        ATInterstitialAd.Instance.client.onAdVideoFailureEvent -= failVideoPlayback;
        ATInterstitialAd.Instance.client.onAdSourceAttemptEvent -= startLoadingADSource;
        ATInterstitialAd.Instance.client.onAdSourceFilledEvent -= finishLoadingADSource;
        ATInterstitialAd.Instance.client.onAdSourceLoadFailureEvent -= failToLoadADSource;
        ATInterstitialAd.Instance.client.onAdSourceBiddingAttemptEvent -= startBiddingADSource;
        ATInterstitialAd.Instance.client.onAdSourceBiddingFilledEvent -= finishBiddingADSource;
        ATInterstitialAd.Instance.client.onAdSourceBiddingFailureEvent -= failBiddingADSource;
    }

    public override void showAd() 
    {
        Debug.Log("Inter showOrLoadAd() >>> ");
        bool isAdReady = ATInterstitialAd.Instance.hasInterstitialAdReady(mPlacementId_interstitial_all);
        if (isAdReady) {
            Dictionary<string, string> jsonmap = new Dictionary<string, string>();
            jsonmap.Add(AnyThinkAds.Api.ATConst.SCENARIO, showingScenario);

            ATInterstitialAd.Instance.showInterstitialAd(mPlacementId_interstitial_all, jsonmap);
        } else {
            setAdReadyStatus(false);
        }
    }

    // static InterstitalCallback callback;
    public override void loadAd()
    {
        Dictionary<string, object> jsonmap = new Dictionary<string, object>();
        jsonmap.Add(AnyThinkAds.Api.ATConst.USE_REWARDED_VIDEO_AS_INTERSTITIAL, AnyThinkAds.Api.ATConst.USE_REWARDED_VIDEO_AS_INTERSTITIAL_NO);
        //jsonmap.Add(AnyThinkAds.Api.ATConst.USE_REWARDED_VIDEO_AS_INTERSTITIAL, AnyThinkAds.Api.ATConst.USE_REWARDED_VIDEO_AS_INTERSTITIAL_YES);
        setLoading();
        ATInterstitialAd.Instance.loadInterstitialAd(mPlacementId_interstitial_all, jsonmap);
    }


    public void onAdClick(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer callback onAdClick :" + erg.placementId + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
    }

    public void onAdClose(object sender, ATAdEventArgs erg)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        Debug.Log("Developer callback onAdClose :" + erg.placementId);
        loadAd();
    }

    public void onAdLoad(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer callback onAdLoad :" + erg.placementId);
        setLoadSuccess();
    }

    public void onShow(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer callback onShow :" + erg.placementId);
    }


    public void onAdLoadFail(object sender, ATAdErrorEventArgs erg)
    {
        Debug.Log("Developer callback onAdLoadFail : : " + erg.placementId + "--erg.errorCode:" + erg.errorCode + "--msg:" + erg.errorMessage);
        setLoadFailed(erg);
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        retryAdAttempt();
    }

    public void onAdShowFail(object sender, ATAdErrorEventArgs erg)
    {
        //Interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Developer callback show fail :" + erg.placementId);
        loadAd();
    }

    public void startVideoPlayback(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer startVideoPlayback------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));
    }

    public void endVideoPlayback(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer endVideoPlayback------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));

    }

    public void failVideoPlayback(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer failVideoPlayback------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));

    }

    public void startLoadingADSource(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer startLoadingADSource------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));

    }

    public void finishLoadingADSource(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer finishLoadingADSource------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));

    }

    public void failToLoadADSource(object sender, ATAdErrorEventArgs erg)
    {
        Debug.Log("Developer failToLoadADSource------erg.errorCode:" + erg.errorCode + "---erg.errorMessage:" + erg.errorMessage + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));

    }

    public void startBiddingADSource(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer startBiddingADSource------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));

    }

    public void finishBiddingADSource(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer finishBiddingADSource------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));

    }

    public void failBiddingADSource(object sender, ATAdErrorEventArgs erg)
    {
        Debug.Log("Developer failBiddingADSource------erg.errorCode:" + erg.errorCode + "---erg.errorMessage:" + erg.errorMessage + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));

    }
}
