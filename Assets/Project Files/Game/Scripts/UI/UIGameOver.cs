using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIGameOver : UIPage
    {
        [BoxGroup("Safe Area", "Safe Area")]
        [SerializeField] RectTransform safeAreaRectTransform;

        [BoxGroup("Text", "Text")]
        [SerializeField] TMP_Text additionalTimeText;

        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button menuButton;
        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button replayButton;
        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button reviveButton;

        [BoxGroup("Animations", "Animations")]
        [SerializeField] UIScaleAnimation levelFailed;
        [BoxGroup("Animations", "Animations")]
        [SerializeField] UIFadeAnimation backgroundFade;
        [BoxGroup("Animations", "Animations")]
        [SerializeField] UIScaleAnimation menuButtonScalable;
        [BoxGroup("Animations", "Animations")]
        [SerializeField] UIScaleAnimation replayButtonScalable;
        [BoxGroup("Animations", "Animations")]
        [SerializeField] UIScaleAnimation reviveButtonScalable;

        [BoxGroup("Lives", "Lives")]
        [SerializeField] LivesIndicator livesIndicator;
        [BoxGroup("Lives", "Lives")]
        [SerializeField] AddLivesPanel addLivesPanel;

        private TweenCase continuePingPongCase;

        public override void Init()
        {
            menuButton.onClick.AddListener(MenuButton);
            replayButton.onClick.AddListener(ReplayButton);
            reviveButton.onClick.AddListener(ReviveButton);

            LivesManager.AddIndicator(livesIndicator);
            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            levelFailed.Hide(immediately: true);
            menuButtonScalable.Hide(immediately: true);
            replayButtonScalable.Hide(immediately: true);
            reviveButtonScalable.Hide(immediately: true);

            float fadeDuration = 0.3f;
            backgroundFade.Show(fadeDuration);

            additionalTimeText.text = $"+{GameController.Data.AdditionalTimerTimeOnFail}";

            livesIndicator.Show();

            Tween.DelayedCall(fadeDuration * 0.8f, delegate
            {
                levelFailed.Show();

                menuButtonScalable.Show(scaleMultiplier: 1.05f, delay: 0.75f);
                replayButtonScalable.Show(scaleMultiplier: 1.05f, delay: 0.75f);
                reviveButtonScalable.Show(scaleMultiplier: 1.05f, delay: 0.25f);

                continuePingPongCase = reviveButtonScalable.RectTransform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

                UIController.OnPageOpened(this);
            });

        }

        public override void PlayHideAnimation()
        {
            backgroundFade.Hide(0.3f);

            livesIndicator.Hide();

            Tween.DelayedCall(0.3f, delegate
            {

                if (continuePingPongCase != null && continuePingPongCase.IsActive)
                    continuePingPongCase.Kill();

                UIController.OnPageClosed(this);
            });
        }

        #endregion

        #region Buttons 

        private void ReviveButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            AdsManager.ShowRewardBasedVideo(ReviveCallback);
        }

        private void ReviveCallback(bool watchedRV)
        {
            if (!watchedRV) return;

            UIController.HidePage<UIGameOver>();
            UIController.ShowPage<UIGame>();

            LevelController.GameplayTimer.SetMaxTime(GameController.Data.AdditionalTimerTimeOnFail);
            LevelController.GameplayTimer.Reset();
            LevelController.GameplayTimer.Start();

            GameController.Revive();
        }

        private void ReplayButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            if (LivesManager.Lives > 0)
            {
                LivesManager.RemoveLife();

                UIController.HidePage<UIGameOver>();
                GameController.ReplayLevel();
            }
            else
            {
                addLivesPanel.Show();
            }
        }

        private void MenuButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            UIController.HidePage<UIGameOver>(() =>
            {
                GameController.ReturnToMenu();
            });
        }

        #endregion
    }
}