using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon.IAPStore;
using Watermelon.Map;
using Watermelon.SkinStore;

namespace Watermelon
{
    public class UIMainMenu : UIPage
    {
        public readonly float STORE_AD_RIGHT_OFFSET_X = 300F;

        [BoxGroup("Safe Area", "Safe Area")]
        [SerializeField] RectTransform safeAreaRectTransform;

        [Space]
        [BoxGroup("Play", "Play")]
        [SerializeField] Button playButton;
        [BoxGroup("Play")]
        [SerializeField] RectTransform tapToPlayRect;
        [BoxGroup("Play")]
        [SerializeField] TMP_Text playButtonText;

        [Space]
        [BoxGroup("Coins", "Coins")]
        [SerializeField] UIScaleAnimation coinsLabelScalable;
        [BoxGroup("Coins", "Coins")]
        [SerializeField] CurrencyUIPanelSimple coinsPanel;
        [BoxGroup("Lives", "Lives")]
        [SerializeField] UIScaleAnimation livesIndicatorScalable;
        [BoxGroup("Lives", "Lives")]
        [SerializeField] AddLivesPanel addLivesPanel;

        [Space]
        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] UIMainMenuButton iapStoreButton;
        [BoxGroup("Buttons")]
        [SerializeField] UIMainMenuButton skinsStoreButton;
        [BoxGroup("Buttons")]
        [SerializeField] UIMainMenuButton noAdsButton;

        [Space]
        [BoxGroup("Popup", "Popup")]
        [SerializeField] UINoAdsPopUp noAdsPopUp;

        private TweenCase tapToPlayPingPong;
        private TweenCase showHideStoreAdButtonDelayTweenCase;

        private void OnEnable()
        {
            IAPManager.OnPurchaseComplete += OnAdPurchased;
        }

        private void OnDisable()
        {
            IAPManager.OnPurchaseComplete -= OnAdPurchased;
        }

        public override void Init()
        {
            coinsPanel.Init();

            iapStoreButton.Init(STORE_AD_RIGHT_OFFSET_X);
            noAdsButton.Init(STORE_AD_RIGHT_OFFSET_X);
            skinsStoreButton.Init(STORE_AD_RIGHT_OFFSET_X);

            iapStoreButton.Button.onClick.AddListener(IAPStoreButton);
            noAdsButton.Button.onClick.AddListener(NoAdButton);
            skinsStoreButton.Button.onClick.AddListener(SkinsStoreButton);
            coinsPanel.AddButton.onClick.AddListener(AddCoinsButton);
            playButton.onClick.AddListener(PlayButton);

            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            showHideStoreAdButtonDelayTweenCase?.Kill();

            HideAdButton(true);
            iapStoreButton.Hide(true);
            skinsStoreButton.Hide(true);
            ShowTapToPlay();

            coinsLabelScalable.Show();
            livesIndicatorScalable.Show();

            UILevelNumberText.Show();
            playButtonText.text = "LEVEL " + (LevelController.MaxReachedLevelIndex + 1);

            showHideStoreAdButtonDelayTweenCase = Tween.DelayedCall(0.12f, delegate
            {
                ShowAdButton();
                iapStoreButton.Show();
                skinsStoreButton.Show();
            });

            MapLevelAbstractBehavior.OnLevelClicked += OnLevelOnMapSelected;

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            if (!isPageDisplayed)
                return;

            showHideStoreAdButtonDelayTweenCase?.Kill();

            isPageDisplayed = false;

            HideTapToPlayButton();

            coinsLabelScalable.Hide();
            livesIndicatorScalable.Hide();

            HideAdButton();

            showHideStoreAdButtonDelayTweenCase = Tween.DelayedCall(0.1f, delegate
            {
                iapStoreButton.Hide();
                skinsStoreButton.Hide();
            });

            MapLevelAbstractBehavior.OnLevelClicked -= OnLevelOnMapSelected;

            Tween.DelayedCall(0.3f, delegate
            {
                UIController.OnPageClosed(this);
            });
        }

        #endregion

        #region Tap To Play Label

        public void ShowTapToPlay(bool immediately = false)
        {
            if (tapToPlayPingPong != null && tapToPlayPingPong.IsActive)
                tapToPlayPingPong.Kill();

            if (immediately)
            {
                tapToPlayRect.localScale = Vector3.one;

                tapToPlayPingPong = tapToPlayRect.transform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

                return;
            }

            // RESET
            tapToPlayRect.localScale = Vector3.zero;

            tapToPlayRect.DOPushScale(Vector3.one * 1.2f, Vector3.one, 0.35f, 0.2f, Ease.Type.CubicOut, Ease.Type.CubicIn).OnComplete(delegate
            {

                tapToPlayPingPong = tapToPlayRect.transform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

            });

        }

        public void HideTapToPlayButton(bool immediately = false)
        {
            if (tapToPlayPingPong != null && tapToPlayPingPong.IsActive)
                tapToPlayPingPong.Kill();

            if (immediately)
            {
                tapToPlayRect.localScale = Vector3.zero;

                return;
            }

            tapToPlayRect.DOScale(Vector3.zero, 0.3f).SetEasing(Ease.Type.CubicIn);
        }

        #endregion

        #region Ad Button Label

        private void ShowAdButton(bool immediately = false)
        {
            if (AdsManager.IsForcedAdEnabled())
            {
                noAdsButton.Show(immediately);
            }
            else
            {
                noAdsButton.Hide(immediately: true);
            }
        }

        private void HideAdButton(bool immediately = false)
        {
            noAdsButton.Hide(immediately);
        }

        private void OnAdPurchased(ProductKeyType productKeyType)
        {
            if (productKeyType == ProductKeyType.NoAds)
            {
                HideAdButton(immediately: true);
            }
        }

        #endregion

        #region Buttons

        private void PlayButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            OnPlayTriggered(LevelController.MaxReachedLevelIndex);
        }

        private void OnLevelOnMapSelected(int levelId)
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            OnPlayTriggered(levelId);
        }

        private void OnPlayTriggered(int levelId)
        {
            if (LivesManager.Lives > 0)
            {
                Overlay.Show(0.3f, () =>
                {
                    UIController.DisablePage<UIMainMenu>();

                    // start level
                    GameController.LoadLevel(levelId);

                    Overlay.Hide(0.3f);
                }, true);

                Tween.DelayedCall(2f, LivesManager.RemoveLife);
            }
            else
            {
                addLivesPanel.Show((bool resultSuccessfull) =>
                {
                    if (resultSuccessfull)
                    {
                        Overlay.Show(0.3f, () =>
                        {
                            UIController.DisablePage<UIMainMenu>();

                            // start level
                            GameController.LoadLevel(levelId);

                            Overlay.Hide(0.3f);
                        }, true);

                        Tween.DelayedCall(2f, LivesManager.RemoveLife);
                    }
                });
            }
        }

        private void IAPStoreButton()
        {
            if (UIController.GetPage<UIStore>().IsPageDisplayed)
                return;

            UILevelNumberText.Hide(true);

            UIController.HidePage<UIMainMenu>();
            UIController.ShowPage<UIStore>();

            // reopening main menu only after store page was opened throug main menu
            UIController.PageClosed += OnIapStoreClosed;
            MapBehavior.DisableScroll();

            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void SkinsStoreButton()
        {
            if (UIController.GetPage<UISkinStore>().IsPageDisplayed)
                return;

            UILevelNumberText.Hide(true);

            UIController.HidePage<UIMainMenu>();
            UIController.ShowPage<UISkinStore>();

            // reopening main menu only after store page was opened throug main menu
            UIController.PageClosed += OnIapStoreClosed;
            MapBehavior.DisableScroll();
        }

        private void OnIapStoreClosed(UIPage page, System.Type pageType)
        {
            if (pageType.Equals(typeof(UIStore)) || pageType.Equals(typeof(UISkinStore)))
            {
                UIController.PageClosed -= OnIapStoreClosed;

                MapBehavior.EnableScroll();
                UIController.ShowPage<UIMainMenu>();
            }
        }

        private void NoAdButton()
        {
            noAdsPopUp.Show();
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void AddCoinsButton()
        {
            IAPStoreButton();
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        #endregion
    }


}
