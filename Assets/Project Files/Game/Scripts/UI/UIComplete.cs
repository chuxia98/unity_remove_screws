using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace Watermelon
{
    public class UIComplete : UIPage
    {
        [BoxGroup("Safe Area", "Safe Area")]
        [SerializeField] RectTransform safeAreaTransform;

        [BoxGroup("Fade", "Fade")]
        [SerializeField] UIFadeAnimation backgroundFade;

        [BoxGroup("Level Complete", "Level Complete")]
        [SerializeField] UIScaleAnimation levelCompleteLabel;

        [BoxGroup("Reward", "Reward")]
        [SerializeField] UIScaleAnimation rewardLabel;
        [BoxGroup("Reward", "Reward")]
        [SerializeField] TextMeshProUGUI rewardAmountText;
        [BoxGroup("Reward", "Reward")]
        [SerializeField] PURewardPanel powerUpsRewardPanel;

        [BoxGroup("Coins", "Coins")]
        [SerializeField] UIScaleAnimation coinsPanelScalable;
        [BoxGroup("Coins", "Coins")]
        [SerializeField] CurrencyUIPanelSimple coinsPanelUI;

        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button multiplyRewardButton;
        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button homeButton;
        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button nextLevelButton;

        [BoxGroup("Button Animations", "Button Animations")]
        [SerializeField] UIFadeAnimation multiplyRewardButtonFade;
        [BoxGroup("Button Animations", "Button Animations")]
        [SerializeField] UIScaleAnimation homeButtonScaleAnimation;
        [BoxGroup("Button Animations", "Button Animations")]
        [SerializeField] UIScaleAnimation nextLevelButtonScaleAnimation;
        

        private TweenCase noThanksAppearTween;

        private int coinsHash = "Coins".GetHashCode();
        private int currentReward;

        public override void Init()
        {
            multiplyRewardButton.onClick.AddListener(MultiplyRewardButton);
            homeButton.onClick.AddListener(HomeButton);
            nextLevelButton.onClick.AddListener(NextLevelButton);

            coinsPanelUI.Init();
            powerUpsRewardPanel.Init();

            NotchSaveArea.RegisterRectTransform(safeAreaTransform);
        }

        #region Show/Hide
        public override void PlayShowAnimation()
        {
            rewardLabel.Hide(immediately: true);
            multiplyRewardButtonFade.Hide(immediately: true);
            multiplyRewardButton.interactable = false;
            nextLevelButtonScaleAnimation.Hide(immediately: true);
            nextLevelButton.interactable = false;
            homeButtonScaleAnimation.Hide(immediately: true);
            homeButton.interactable = false;
            coinsPanelScalable.Hide(immediately: true);

            currentReward = LevelController.GetCurrentLevelReward();

            backgroundFade.Show(duration: 0.3f);
            levelCompleteLabel.Show();

            coinsPanelScalable.Show();

            powerUpsRewardPanel.Show(LevelController.GetPUReward(), 0.5f);

            if(currentReward > 0)
            {
                ShowRewardLabel(currentReward, false, 0.3f, delegate
                {
                    rewardLabel.RectTransform.DOPushScale(Vector3.one * 1.1f, Vector3.one, 0.2f, 0.2f).OnComplete(delegate
                    {
                        multiplyRewardButtonFade.Show();
                        multiplyRewardButton.interactable = true;

                        FloatingCloud.SpawnCurrency(coinsHash, rewardLabel.RectTransform, coinsPanelScalable.RectTransform, 10, "", () =>
                        {
                            CurrenciesController.Add(CurrencyType.Coins, currentReward);

                            homeButtonScaleAnimation.Show(1.05f, 0.25f, 1f);
                            nextLevelButtonScaleAnimation.Show(1.05f, 0.25f, 1f);

                            homeButton.interactable = true;
                            nextLevelButton.interactable = true;
                        });
                    });
                });
            }
            else
            {
                rewardLabel.Hide(immediately: true);

                homeButtonScaleAnimation.Show(1.05f, 0.25f, 1f);
                nextLevelButtonScaleAnimation.Show(1.05f, 0.25f, 1f);

                homeButton.interactable = true;
                nextLevelButton.interactable = true;
            }
        }

        public override void PlayHideAnimation()
        {
            UIController.OnPageClosed(this);
        }


        #endregion

        #region RewardLabel

        public void ShowRewardLabel(float rewardAmounts, bool immediately = false, float duration = 0.3f, Action onComplted = null)
        {
            rewardLabel.Show(immediately: immediately);

            if (immediately)
            {
                rewardAmountText.text = "+" + rewardAmounts;
                onComplted?.Invoke();

                return;
            }

            rewardAmountText.text = "+" + 0;

            Tween.DoFloat(0, rewardAmounts, duration, (float value) =>
            {
                rewardAmountText.text = "+" + (int)value;
            }).OnComplete(delegate
            {

                onComplted?.Invoke();
            });
        }

        #endregion

        #region Buttons

        public void MultiplyRewardButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            if (noThanksAppearTween != null && noThanksAppearTween.IsActive)
            {
                noThanksAppearTween.Kill();
            }

            homeButton.interactable = false;
            nextLevelButton.interactable = false;

            AdsManager.ShowRewardBasedVideo((bool success) =>
            {
                if (success)
                {
                    int rewardMult = 3;

                    multiplyRewardButtonFade.Hide(immediately: true);
                    multiplyRewardButton.interactable = false;

                    ShowRewardLabel(currentReward * rewardMult, false, 0.3f, delegate
                    {
                        FloatingCloud.SpawnCurrency(coinsHash, rewardLabel.RectTransform, coinsPanelScalable.RectTransform, 10, "", () =>
                        {
                            CurrenciesController.Add(CurrencyType.Coins, currentReward * rewardMult);

                            homeButton.interactable = true;
                            nextLevelButton.interactable = true;
                        });
                    });
                }
                else
                {
                    NextLevelButton();
                }
            });
        }

        public void NextLevelButton()
        {
            if(!GameController.Data.InfiniteLevels && LevelController.MaxReachedLevelIndex >= LevelController.Database.AmountOfLevels)
            {
                LevelController.ClampMaxReachedLevel();

                HomeButton();
            }
            else
            {
                AudioController.PlaySound(AudioController.AudioClips.buttonSound);

                Overlay.Show(0.3f, () =>
                {
                    UIController.DisablePage<UIComplete>();

                    GameController.LoadNextLevel();

                    Overlay.Hide(0.3f);
                });
            }
        }

        public void HomeButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            Overlay.Show(0.3f, () =>
            {
                UIController.DisablePage<UIComplete>();

                GameController.ReturnToMenu();

                Overlay.Hide(0.3f);
            });

            LivesManager.AddLife();
        }

        #endregion
    }
}
