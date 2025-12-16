#pragma warning disable 0649 

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    public class GDPRPanel : MonoBehaviour
    {
        [SerializeField] GameObject termsOfUseObject;
        [SerializeField] GameObject privacyPolicyObject;

        [SerializeField] Button acceptButton;

        private GDPRLoadingTask gdprLoadingTask;

        public void Init(GDPRLoadingTask gdprLoadingTask)
        {
            this.gdprLoadingTask = gdprLoadingTask;

            // Inititalise panel
            termsOfUseObject.AddEvent(EventTriggerType.PointerDown, (data) => OpenTermsOfUseLinkButton());
            privacyPolicyObject.AddEvent(EventTriggerType.PointerDown, (data) => OpenPrivacyLinkButton());

            acceptButton.onClick.AddListener(() => SetGDPRStateButton(false));

            DontDestroyOnLoad(gameObject);

            gameObject.SetActive(true);
        }

        public void OpenPrivacyLinkButton()
        {
            Application.OpenURL(AdsManager.Settings.PrivacyLink);
        }

        public void OpenTermsOfUseLinkButton()
        {
            Application.OpenURL(AdsManager.Settings.TermsOfUseLink);
        }

        public void SetGDPRStateButton(bool state)
        {
            AdsManager.SetGDPR(state);

            CloseWindow();

            gdprLoadingTask.CompleteTask();
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);
        }
    }
}

// -----------------
// Advertisement v 1.3
// -----------------