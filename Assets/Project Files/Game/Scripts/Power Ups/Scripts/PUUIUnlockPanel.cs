using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class PUUIUnlockPanel : MonoBehaviour, IPopupWindow
    {
        [SerializeField] GameObject powerUpPanel;

        [Space(5)]
        [SerializeField] Image powerUpPurchasePreview;
        [SerializeField] TMP_Text powerUpPurchaseDescriptionText;

        [Space(5)]
        [SerializeField] Button closeButton;

        private bool isOpened;
        public bool IsOpened => isOpened;

        private List<PUSettings> unlockedPowerUps;
        private int pageIndex = 0;

        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);
        }

        public void Show(List<PUSettings> unlockedPowerUps)
        {
            this.unlockedPowerUps = unlockedPowerUps;

            powerUpPanel.SetActive(true);

            pageIndex = 0;

            PreparePage(pageIndex);

            UIController.OnPopupWindowOpened(this);
        }

        private void PreparePage(int index)
        {
            if (!unlockedPowerUps.IsInRange(index)) return;

            PUSettings settings = unlockedPowerUps[index];

            powerUpPurchasePreview.sprite = settings.Icon;
            powerUpPurchaseDescriptionText.text = settings.Description;

            powerUpPurchasePreview.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            powerUpPurchasePreview.transform.DOScale(1.0f, 0.25f).SetEasing(Ease.Type.BackOut);
        }

        public void ClosePanel()
        {
            pageIndex++;

            if(pageIndex >= unlockedPowerUps.Count)
            {
                powerUpPanel.SetActive(false);

                UIController.OnPopupWindowClosed(this);

                foreach(PUSettings unlockerPowerUp in unlockedPowerUps)
                {
                    PUController.UnlockPowerUp(unlockerPowerUp.Type);
                }
            }
            else
            {
                PreparePage(pageIndex);
            }
        }
    }
}