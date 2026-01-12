using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AnyThinkAds.Api;
using UnityEngine.UI;
using AnyThinkAds.ThirdParty.LitJson;

public class RewardVideoAdOperator : BaseAdOperator
{

#if UNITY_ANDROID
    static string mPlacementId_rewardvideo_all = "n6943bf414b33c";
    static string showingScenario = "f5e71c46d1a28f";
#elif UNITY_IOS || UNITY_IPHONE
    static string mPlacementId_rewardvideo_all = "b5b44a0f115321";//"b5b44a0f115321";
    static string showingScenario = "f5e54970dc84e6";

#endif

    ATRewardedVideo rewardedVideo;

    private static readonly RewardVideoAdOperator instance = new RewardVideoAdOperator();

    private RewardVideoAdOperator() 
	{
        
	}

	public static RewardVideoAdOperator Instance 
	{
        get 
		{
			return instance;
		}
	}

    // Use this for initialization
    public override void initializeAd()
    {
        ATRewardedVideo.Instance.client.onAdLoadEvent += onAdLoad;
        ATRewardedVideo.Instance.client.onAdLoadFailureEvent += onAdLoadFail;
        ATRewardedVideo.Instance.client.onRewardEvent += onReward;
        ATRewardedVideo.Instance.client.onAdVideoCloseEvent += onAdVideoClosedEvent;
        ATRewardedVideo.Instance.client.onAdVideoEndEvent += onAdVideoEndEvent;
        ATRewardedVideo.Instance.client.onAdVideoStartEvent += onAdVideoStartEvent;
        ATRewardedVideo.Instance.client.onAdVideoFailureEvent += onAdVideoPlayFailure;
        ATRewardedVideo.Instance.client.onAdClickEvent += onAdClick;
        ATRewardedVideo.Instance.client.onPlayAgainStart += onRewardedVideoAdAgainPlayStart;
        ATRewardedVideo.Instance.client.onPlayAgainFailure += onRewardedVideoAdAgainPlayFail;
        ATRewardedVideo.Instance.client.onPlayAgainEnd += onRewardedVideoAdAgainPlayEnd;
        ATRewardedVideo.Instance.client.onPlayAgainClick += onRewardedVideoAdAgainPlayClicked;
        ATRewardedVideo.Instance.client.onPlayAgainReward += onAgainReward;
        ATRewardedVideo.Instance.client.onAdSourceAttemptEvent += startLoadingADSource;
        ATRewardedVideo.Instance.client.onAdSourceFilledEvent += finishLoadingADSource;
        ATRewardedVideo.Instance.client.onAdSourceLoadFailureEvent += failToLoadADSource;
        ATRewardedVideo.Instance.client.onAdSourceBiddingAttemptEvent += startBiddingADSource;
        ATRewardedVideo.Instance.client.onAdSourceBiddingFilledEvent += finishBiddingADSource;
        ATRewardedVideo.Instance.client.onAdSourceBiddingFailureEvent += failBiddingADSource;
    }

    public override void destroyAd()
    {
        ATRewardedVideo.Instance.client.onAdLoadEvent -= onAdLoad;
        ATRewardedVideo.Instance.client.onAdLoadFailureEvent -= onAdLoadFail;
        ATRewardedVideo.Instance.client.onRewardEvent -= onReward;
        ATRewardedVideo.Instance.client.onAdVideoCloseEvent -= onAdVideoClosedEvent;
        ATRewardedVideo.Instance.client.onAdVideoEndEvent -= onAdVideoEndEvent;
        ATRewardedVideo.Instance.client.onAdVideoStartEvent -= onAdVideoStartEvent;
        ATRewardedVideo.Instance.client.onAdVideoFailureEvent -= onAdVideoPlayFailure;
        ATRewardedVideo.Instance.client.onAdClickEvent -= onAdClick;
        ATRewardedVideo.Instance.client.onPlayAgainStart -= onRewardedVideoAdAgainPlayStart;
        ATRewardedVideo.Instance.client.onPlayAgainFailure -= onRewardedVideoAdAgainPlayFail;
        ATRewardedVideo.Instance.client.onPlayAgainEnd -= onRewardedVideoAdAgainPlayEnd;
        ATRewardedVideo.Instance.client.onPlayAgainClick -= onRewardedVideoAdAgainPlayClicked;
        ATRewardedVideo.Instance.client.onPlayAgainReward -= onAgainReward;
        ATRewardedVideo.Instance.client.onAdSourceAttemptEvent -= startLoadingADSource;
        ATRewardedVideo.Instance.client.onAdSourceFilledEvent -= finishLoadingADSource;
        ATRewardedVideo.Instance.client.onAdSourceLoadFailureEvent -= failToLoadADSource;
        ATRewardedVideo.Instance.client.onAdSourceBiddingAttemptEvent -= startBiddingADSource;
        ATRewardedVideo.Instance.client.onAdSourceBiddingFilledEvent -= finishBiddingADSource;
        ATRewardedVideo.Instance.client.onAdSourceBiddingFailureEvent -= failBiddingADSource;
    }

    public override void showAd() 
    {
         bool isAdReady = ATRewardedVideo.Instance.hasAdReady(mPlacementId_rewardvideo_all);
        if (isAdReady) {
            Dictionary<string, string> jsonmap = new Dictionary<string, string>();
            jsonmap.Add(AnyThinkAds.Api.ATConst.SCENARIO, showingScenario);
            ATRewardedVideo.Instance.showAd(mPlacementId_rewardvideo_all, jsonmap);
        } else {
            setAdReadyStatus(false);
        }
    }

    public override void loadAd()
    {
        ATSDKAPI.setCustomDataForPlacementID(new Dictionary<string, string> { { "placement_custom_key", "placement_custom" } }, mPlacementId_rewardvideo_all);

        Dictionary<string, string> jsonmap = new Dictionary<string, string>();
        jsonmap.Add(ATConst.USERID_KEY, "test_user_id");
        jsonmap.Add(ATConst.USER_EXTRA_DATA, "test_user_extra_data");

        setLoading();
        ATRewardedVideo.Instance.loadVideoAd(mPlacementId_rewardvideo_all, jsonmap);
        // ATRewardedVideo.Instance.addAutoLoadAdPlacementID(mPlacementId_rewardvideo_all);

    }

    public void onAdClick(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer callback onAdClick :" + erg.placementId + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
    }

    public void onAdClose(object sender, ATAdRewardEventArgs erg)
    {
        Debug.Log("Developer callback onAdClose :" + erg.placementId);
    }

    public void onAdLoad(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer callback onAdLoad :" + erg.placementId);
        setLoadSuccess();
    }

    public void onAdLoadFail(object sender, ATAdErrorEventArgs erg)
    {
        Debug.Log("Developer callback onAdLoadFail : : " + erg.placementId + "--erg.errorCode:" + erg.errorCode + "--msg:" + erg.errorMessage);
        setLoadFailed(erg);
         // Reward ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        retryAdAttempt();
    }

    public void onAdVideoStartEvent(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer onAdVideoStartEvent------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
    }

    public void onAdVideoEndEvent(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer onAdVideoEndEvent------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
    }

    public void onAdVideoClosedEvent(object sender, ATAdEventArgs erg)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        Debug.Log("Developer onAdVideoClosedEvent------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
        loadAd();
    }

    public void onAdVideoPlayFailure(object sender, ATAdErrorEventArgs erg)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Debug.Log("Developer onAdVideoClosedEvent------" + "->" + JsonMapper.ToJson(erg.errorMessage));
        loadAd();
    }

    public void onReward(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer onReward------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
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

    public void onRewardedVideoAdAgainPlayStart(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer onRewardedVideoAdAgainPlayStart------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
    }

    public void onRewardedVideoAdAgainPlayEnd(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer onRewardedVideoAdAgainPlayEnd------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
    }

    public void onRewardedVideoAdAgainPlayFail(object sender, ATAdErrorEventArgs erg)
    {
        Debug.Log("Developer onRewardedVideoAdAgainPlayFail------code:" + erg.errorCode + "---message:" + erg.errorMessage);
    }

    public void onRewardedVideoAdAgainPlayClicked(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer onRewardedVideoAdAgainPlayClicked------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));

    }

    public void onAgainReward(object sender, ATAdEventArgs erg)
    {
        Debug.Log("Developer onAgainReward------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));

    }
}
