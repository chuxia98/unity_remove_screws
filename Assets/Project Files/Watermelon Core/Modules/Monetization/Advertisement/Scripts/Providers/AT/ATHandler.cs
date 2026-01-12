using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnyThinkAds.Api;
using AnyThinkAds.ThirdParty.LitJson;

namespace Watermelon
{
    public class ATHandler : AdProviderHandler
    {
#if UNITY_ANDROID
        private const string SdkAppId = "a5aa1f9deda26d";
        private const string SdkKey = "4f7b9ac17decb9babec83aac078742c7";
        //private const string SdkAppId = "h6943be368bb6e";
        //private const string SdkKey = "a9a4b725c5d9cdc6817c5f42d9e1c0c98";
#elif UNITY_IOS || UNITY_IPHONE
        private const string SdkAppId = "a5b0e8491845b3";
        private const string SdkKey = "7eae0567827cfe2b22874061763f30c9";
#endif

        //private const string screenID = "b5baca53984692";
        //private const string videoID = "b5b449fb3d89d7";
        //private const string bannerID = "b5baca4f74c3d8";
        private const string screenID = "n6943be962cb5d";
        private const string videoID = "n6943bf414b33c";
        private const string bannerID = "n6944c0b3b6d44";

        private ATRewardedHandler videoHandler;
        private ATBannerHandler bannerHandler;

        public ATHandler(AdProvider moduleType) : base(moduleType) { }


        public override void DestroyBanner()
        {

        }

        public override void HideBanner()
        {
            ATBannerAd.Instance.hideBannerAd(bannerID);
        }

        public override void Init(AdsSettings adsSettings)
        {
            this.adsSettings = adsSettings;
        }

        public override bool IsInterstitialLoaded()
        {
            return ATInterstitialAd.Instance.hasInterstitialAdReady(screenID);
            //return ATInterstitialAutoAd.Instance.autoLoadInterstitialAdReadyForPlacementID(screenID);
        }

        public override bool IsRewardedVideoLoaded()
        {
            //bool isLoad =  ATRewardedAutoVideo.Instance.autoLoadRewardedVideoReadyForPlacementID(videoID);
            bool isLoad = ATRewardedVideo.Instance.client.autoLoadRewardedVideoReadyForPlacementID(videoID);
            Debug.Log("[A] ATHandler IsRewardedVideoLoaded: " + isLoad);
            return isLoad; // ATRewardedAutoVideo.Instance.autoLoadRewardedVideoReadyForPlacementID(videoID);
        }

        public override void RequestInterstitial()
        {
            Debug.Log("[A] ATHandler RequestInterstitial");
            //string[] ids = new string[] { screenID };
            //ATInterstitialAutoAd.Instance.addAutoLoadAdPlacementID(ids);
            ATInterstitialAd.Instance.loadInterstitialAd(screenID, new Dictionary<string, object> { });
        }

        public override void RequestRewardedVideo()
        {
            Debug.Log("[A] ATHandler RequestRewardedVideo");
            string[] ids = new string[] { videoID };
            ATRewardedVideo.Instance.client.addAutoLoadAdPlacementID(ids);
            ATRewardedAutoVideo.Instance.addAutoLoadAdPlacementID(ids);
        }

        public override void ShowBanner()
        {
            Debug.Log("[A] ATHandler ShowBanner");
            if (IsInterstitialLoaded())
            {
                ATBannerAd.Instance.showBannerAd(bannerID, ATBannerAdLoadingExtra.kATBannerAdShowingPisitionBottom);
            }
        }

        public override void ShowInterstitial(InterstitialCallback callback)
        {
            if (IsInterstitialLoaded())
            {
                //ATInterstitialAutoAd.Instance.showAutoAd(screenID);
                ATInterstitialAd.Instance.showInterstitialAd(screenID);
            }
        }

        public override void ShowRewardedVideo(RewardedVideoCallback callback)
        {
            if (IsRewardedVideoLoaded())
            {
                //ATRewardedAutoVideo.Instance.showAutoAd(videoID);
                ATRewardedVideo.Instance.client.showAutoAd(videoID, "");
            }
        }

    }

    //public class InitListener : IATSDKInitListener
    //{
    //    public void initSuccess()
    //    {
    //        Debug.Log("[A] ATContainer initSuccess");
    //    }

    //    public void initFail(string message)
    //    {
    //        Debug.Log("[A] ATContainer initFail: " + message);
    //    }
    //}
}
