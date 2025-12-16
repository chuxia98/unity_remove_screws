using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIPause : UIPage
    {
        [BoxGroup("References", "References")]
        [SerializeField] Image backgroundImage;
        [BoxGroup("References", "References")]
        [SerializeField] RectTransform panel;

        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button closeButton;
        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button continueButton;
        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button quitButton;

        [BoxGroup("Popup", "Popup")]
        [SerializeField] UILevelQuitPopUp levelQuitPopUp;

        public override void Init()
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            closeButton.onClick.AddListener(OnContinueButtonClicked);
        }

        public override void PlayHideAnimation()
        {
            panel.DOAnchoredPosition(Vector2.down * 2000, 0.3f).SetEasing(Ease.Type.SineIn);

            backgroundImage.DOFade(0, 0.3f).OnComplete(() => UIController.OnPageClosed(this));
        }

        public override void PlayShowAnimation()
        {
            panel.anchoredPosition = Vector2.down * 2000;
            panel.DOAnchoredPosition(Vector2.zero, 0.3f).SetEasing(Ease.Type.SineOut);

            backgroundImage.SetAlpha(0);
            backgroundImage.DOFade(0.3f, 0.3f).OnComplete(() => UIController.OnPageOpened(this));

            if (GameController.Data.GameplayTimerEnabled) LevelController.GameplayTimer.Pause();
        }

        private void OnEnable()
        {
            levelQuitPopUp.OnConfirmExitEvent += ExitPopUpConfirmExitButton;
            levelQuitPopUp.OnCancelExitEvent += ExitPopCloseButton;
        }

        private void OnDisable()
        {
            levelQuitPopUp.OnConfirmExitEvent -= ExitPopUpConfirmExitButton;
            levelQuitPopUp.OnCancelExitEvent += ExitPopCloseButton;
        }

        public void ExitPopCloseButton()
        {
            levelQuitPopUp.Hide();
        }

        public void ExitPopUpConfirmExitButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            if (LivesManager.IsMaxLives)
                LivesManager.RemoveLife();

            Overlay.Show(0.3f, () =>
            {
                levelQuitPopUp.Hide();

                UIController.DisablePage<UIPause>();
                UIController.DisablePage<UIGame>();

                GameController.ReturnToMenu();

                Overlay.Hide(0.3f);
            });
        }

        private void OnQuitButtonClicked()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            levelQuitPopUp.Show();
        }

        private void OnContinueButtonClicked()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            UIController.HidePage<UIPause>(() => {
                if (GameController.Data.GameplayTimerEnabled) LevelController.GameplayTimer.Resume();
            });
        }
    }
}
