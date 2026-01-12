using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyThinkAds.Api;
using AnyThinkAds.ThirdParty.LitJson;

namespace Watermelon
{
    public class ATRewardedHandler : ATRewardedVideoListener
    {
        public void failBiddingADSource(string placementId, ATCallbackInfo callbackInfo, string code, string message)
        {
            Debug.Log("[A] ATRewardedHandler failBiddingADSource");
        }

        public void failToLoadADSource(string placementId, ATCallbackInfo callbackInfo, string code, string message)
        {
            Debug.Log("[A] ATRewardedHandler failToLoadADSource");
        }

        public void finishBiddingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATRewardedHandler finishBiddingADSource");
        }

        public void finishLoadingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATRewardedHandler finishLoadingADSource");
        }

        public void onReward(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATRewardedHandler onReward");
        }

        public void onRewardedVideoAdLoaded(string placementId)
        {
            Debug.Log("[A] ATRewardedHandler onRewardedVideoAdLoaded");
        }

        public void onRewardedVideoAdLoadFail(string placementId, string code, string message)
        {
            Debug.Log("[A] ATRewardedHandler onRewardedVideoAdLoadFail");
        }

        public void onRewardedVideoAdPlayClicked(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATRewardedHandler onRewardedVideoAdPlayClicked");
        }

        public void onRewardedVideoAdPlayClosed(string placementId, bool isReward, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATRewardedHandler onRewardedVideoAdPlayClosed");
        }

        public void onRewardedVideoAdPlayEnd(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATRewardedHandler onRewardedVideoAdPlayEnd");
        }

        public void onRewardedVideoAdPlayFail(string placementId, string code, string message)
        {
            Debug.Log("[A] ATRewardedHandler onRewardedVideoAdPlayFail");
        }

        public void onRewardedVideoAdPlayStart(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATRewardedHandler onRewardedVideoAdPlayStart");
        }

        public void startBiddingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATRewardedHandler startBiddingADSource");
        }

        public void startLoadingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATRewardedHandler startLoadingADSource");
        }
    }
}
