
using UnityEngine;
using System.Collections.Generic;

using AnyThinkAds.Api;

namespace Watermelon
{
    public class AdDummyHandler : AdProviderHandler, ATSDKInitListener
    {
        private AdDummyController dummyController;

        private bool isInterstitialLoaded = false;
        private bool isRewardVideoLoaded = false;
        private ATHandler atHandler;
        private const string SdkAppId = "h6943be368bb6e";
        private const string SdkKey = "a9a4b725c5d9cdc6817c5f42d9e1c0c98";

        public AdDummyHandler(AdProvider providerType) : base(providerType) { }

        public override void Init(AdsSettings adsSettings)
        {
            this.adsSettings = adsSettings;
            if (atHandler == null)
            {
                atHandler = new ATHandler(AdProvider.AT);
            }
            atHandler.Init(adsSettings);

            if (Monetization.VerboseLogging)
                Debug.Log("[AdsManager]: Module " + providerType.ToString() + " has initialized!");

            if (adsSettings.IsDummyEnabled())
            {
                dummyController = AdDummyController.CreateObject();
                dummyController.Init(adsSettings);
            }

            OnProviderInitialized();

            //Debug.Log("[A] ATHandler Init");
            //（可选配置）设置自定义的Map信息，可匹配后台配置的广告商顺序的列表（App纬度）
            //注意：调用此方法会清除setChannel()、setSubChannel()方法设置的信息，如果有设置这些信息，请在调用此方法后重新设置
            ATSDKAPI.initCustomMap(new Dictionary<string, string> { { "unity3d_data", "test_data" } });

            //（可选配置）设置自定义的Map信息，可匹配后台配置的广告商顺序的列表（Placement纬度）
            // ATSDKAPI.setCustomDataForPlacementID(new Dictionary<string, string> { { "unity3d_data_pl", "test_data_pl" } }, placementId);

            //（可选配置）设置渠道的信息，开发者可以通过该渠道信息在后台来区分看各个渠道的广告数据
            // 注意：如果有使用initCustomMap()方法，必须在initCustomMap()方法之后调用此方法
            ATSDKAPI.setChannel("unity3d_test_channel");

            //（可选配置）设置子渠道的信息，开发者可以通过该渠道信息在后台来区分看各个渠道的子渠道广告数据
            //注意：如果有使用initCustomMap()方法，必须在initCustomMap()方法之后调用此方法
            ATSDKAPI.setSubChannel("unity3d_test_subchannel");

            //设置开启Debug日志（强烈建议测试阶段开启，方便排查问题）
            ATSDKAPI.setLogDebug(true);
            //注意：应用上线前需要关闭日志功能
            //ATSDKAPI.setnew
            //TUSDK.setNetworkLogDebug(true);


            //app - id：h6943be368bb6e
            //app - key： a9a4b725c5d9cdc6817c5f42d9e1c0c98
            //插屏：n66a0bb5925c27
            //激励：n66ebda2670e48
            //（必须配置）SDK的初始化
            ATSDKAPI.initSDK(SdkAppId, SdkKey, this);//Use your own app_id & app_key here
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeBannerAds();
        }

        // ATSDKInitListener

        public void initSuccess()
        {
            Debug.Log("[A] ATHandler ATContainer initSuccess");
        }

        public void initFail(string message)
        {
            Debug.Log("[A] ATHandler ATContainer initFail: " + message);
        }
        //
        private void InitializeInterstitialAds()
        {
            //InterstitialAdOperator.Instance.statusChangeEvent += statusChange;
            //InterstitialAdOperator.Instance.retryLoadAdAttemptEvent += retryLoadAdAttempt;
            InterstitialAdOperator.Instance.initializeAd();
            //LoadInterstitialAd();
            InterstitialAdOperator.Instance.loadAd();
        }

        private void InitializeRewardedAds()
        {
            //RewardVideoAdOperator.Instance.statusChangeEvent += statusChange;
            //RewardVideoAdOperator.Instance.retryLoadAdAttemptEvent += retryLoadAdAttempt;
            RewardVideoAdOperator.Instance.initializeAd();
            //LoadRewardVideoAd();
            RewardVideoAdOperator.Instance.loadAd();
        }

        private void InitializeBannerAds()
        {
            //BannerAdOperator.Instance.statusChangeEvent += statusChange;
            //BannerAdOperator.Instance.retryLoadAdAttemptEvent += retryLoadAdAttempt;
            BannerAdOperator.Instance.initializeAd();
            //LoadBannerAd();
            BannerAdOperator.Instance.loadAd();
        }

        //

        public override void ShowBanner()
        {
            dummyController.ShowBanner();
            atHandler.ShowBanner();
            AdsManager.OnProviderAdDisplayed(providerType, AdType.Banner);
        }

        public override void HideBanner()
        {
            dummyController.HideBanner();

            AdsManager.OnProviderAdClosed(providerType, AdType.Banner);
        }

        public override void DestroyBanner()
        {
            dummyController.HideBanner();

            AdsManager.OnProviderAdClosed(providerType, AdType.Banner);
        }

        public override void RequestInterstitial()
        {
            Debug.Log("[A] void RequestInterstitial");
            isInterstitialLoaded = true;
            atHandler.RequestInterstitial();
            AdsManager.OnProviderAdLoaded(providerType, AdType.Interstitial);
        }

        public override bool IsInterstitialLoaded()
        {
            return isInterstitialLoaded;
        }

        public override void ShowInterstitial(InterstitialCallback callback)
        {
            dummyController.ShowInterstitial();
            atHandler.ShowInterstitial((hasReward) =>
            {
                Debug.Log("[A] void ShowInterstitial hasReward: " + hasReward);

            });
            AdsManager.OnProviderAdDisplayed(providerType, AdType.Interstitial);
        }

        public override void RequestRewardedVideo()
        {
            Debug.Log("[A] void RequestRewardedVideo");
            isRewardVideoLoaded = true;
            atHandler.RequestRewardedVideo();
            AdsManager.OnProviderAdLoaded(providerType, AdType.RewardedVideo);
        }

        public override bool IsRewardedVideoLoaded()
        {
            return isRewardVideoLoaded;
        }

        public override void ShowRewardedVideo(RewardedVideoCallback callback)
        {
            Debug.Log("[A] void RequestRewardedVideo");
            dummyController.ShowRewardedVideo();
            atHandler.ShowRewardedVideo((hasReward) =>
            {
                Debug.Log("[A] void ShowRewardedVideo hasReward: " + hasReward);

            });
            AdsManager.OnProviderAdDisplayed(providerType, AdType.RewardedVideo);
        }
    }
}