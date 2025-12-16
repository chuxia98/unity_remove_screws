using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class ReplayPopupBehavior : MonoBehaviour
    {
        [SerializeField] Canvas canvas;
        [SerializeField] Image fadeImage;
        [SerializeField] RectTransform panel;
        [SerializeField] Button adButton;
        [SerializeField] Button coinsButton;
        [SerializeField] TMP_Text coinsText;
        [SerializeField] Button exitButton;

        public void Awake()
        {
            adButton.onClick.AddListener(OnAdsButtonClicked);
            coinsButton.onClick.AddListener(OnCoinsButtonClicked);
            exitButton.onClick.AddListener(Hide);
        }

        public void Show()
        {
            canvas.enabled = true;
            fadeImage.SetAlpha(0);
            fadeImage.DOFade(0.3f, 0.3f);
            panel.anchoredPosition = Vector2.down * 2000;
            panel.DOAnchoredPosition(Vector2.zero, 0.3f).SetEasing(Ease.Type.SineOut);

            coinsText.text = GameController.Data.ReplayStageCost.ToString();

            if (GameController.Data.GameplayTimerEnabled) LevelController.GameplayTimer.Pause();
        }

        public void Hide()
        {
            fadeImage.DOFade(0, 0.3f);
            panel.DOAnchoredPosition(Vector2.down * 2000, 0.3f).SetEasing(Ease.Type.SineIn).OnComplete(() => {
                canvas.enabled = false;

                if (GameController.Data.GameplayTimerEnabled) LevelController.GameplayTimer.Resume();
            });
        }

        private void OnAdsButtonClicked()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            AdsManager.ShowRewardBasedVideo((bool success) =>
            {
                Hide();

                GameController.ReplayStage();
            });
        }

        private void OnCoinsButtonClicked()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            if (CurrenciesController.Get(CurrencyType.Coins) >= GameController.Data.ReplayStageCost)
            {
                CurrenciesController.Substract(CurrencyType.Coins, GameController.Data.ReplayStageCost);

                Hide();

                GameController.ReplayStage();
            }
        }
    }
}
