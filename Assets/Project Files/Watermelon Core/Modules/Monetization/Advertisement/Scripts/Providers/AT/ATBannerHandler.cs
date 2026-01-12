using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnyThinkAds.Api;

namespace Watermelon
{

    public class ATBannerHandler : ATInterstitialAdListener
    {
        public void failBiddingADSource(string placementId, ATCallbackInfo callbackInfo, string code, string message)
        {
            Debug.Log("[A] ATBannerHandler failBiddingADSource");
        }

        public void failToLoadADSource(string placementId, ATCallbackInfo callbackInfo, string code, string message)
        {
            Debug.Log("[A] ATBannerHandler failToLoadADSource");
        }

        public void finishBiddingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATBannerHandler finishBiddingADSource");
        }

        public void finishLoadingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATBannerHandler finishLoadingADSource");
        }

        public void onInterstitialAdClick(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATBannerHandler onInterstitialAdClick");
        }

        public void onInterstitialAdClose(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATBannerHandler onInterstitialAdClose");
        }

        public void onInterstitialAdEndPlayingVideo(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATBannerHandler onInterstitialAdEndPlayingVideo");
        }

        public void onInterstitialAdFailedToPlayVideo(string placementId, string code, string message)
        {
            Debug.Log("[A] ATBannerHandler onInterstitialAdFailedToPlayVideo");
        }

        public void onInterstitialAdFailedToShow(string placementId)
        {
            Debug.Log("[A] ATBannerHandler onInterstitialAdFailedToShow");
        }

        public void onInterstitialAdLoad(string placementId)
        {
            Debug.Log("[A] ATBannerHandler onInterstitialAdLoad");
        }

        public void onInterstitialAdLoadFail(string placementId, string code, string message)
        {
            Debug.Log("[A] ATBannerHandler onInterstitialAdLoadFail");
        }

        public void onInterstitialAdShow(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATBannerHandler onInterstitialAdShow");
        }

        public void onInterstitialAdStartPlayingVideo(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATBannerHandler onInterstitialAdStartPlayingVideo");
        }

        public void startBiddingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATBannerHandler startBiddingADSource");
        }

        public void startLoadingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log("[A] ATBannerHandler startLoadingADSource");
        }
    }
}
