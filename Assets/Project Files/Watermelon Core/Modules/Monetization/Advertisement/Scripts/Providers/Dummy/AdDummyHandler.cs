
using UnityEngine;

namespace Watermelon
{
    public class AdDummyHandler : AdProviderHandler
    {
        private AdDummyController dummyController;

        private bool isInterstitialLoaded = false;
        private bool isRewardVideoLoaded = false;

        public AdDummyHandler(AdProvider providerType) : base(providerType) { }

        public override void Init(AdsSettings adsSettings)
        {
            this.adsSettings = adsSettings;

            if (Monetization.VerboseLogging)
                Debug.Log("[AdsManager]: Module " + providerType.ToString() + " has initialized!");

            if (adsSettings.IsDummyEnabled())
            {
                dummyController = AdDummyController.CreateObject();
                dummyController.Init(adsSettings);
            }

            OnProviderInitialized();
        }

        public override void ShowBanner()
        {
            dummyController.ShowBanner();

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
            isInterstitialLoaded = true;

            AdsManager.OnProviderAdLoaded(providerType, AdType.Interstitial);
        }

        public override bool IsInterstitialLoaded()
        {
            return isInterstitialLoaded;
        }

        public override void ShowInterstitial(InterstitialCallback callback)
        {
            dummyController.ShowInterstitial();

            AdsManager.OnProviderAdDisplayed(providerType, AdType.Interstitial);
        }

        public override void RequestRewardedVideo()
        {
            isRewardVideoLoaded = true;

            AdsManager.OnProviderAdLoaded(providerType, AdType.RewardedVideo);
        }

        public override bool IsRewardedVideoLoaded()
        {
            return isRewardVideoLoaded;
        }

        public override void ShowRewardedVideo(RewardedVideoCallback callback)
        {
            dummyController.ShowRewardedVideo();

            AdsManager.OnProviderAdDisplayed(providerType, AdType.RewardedVideo);
        }
    }
}