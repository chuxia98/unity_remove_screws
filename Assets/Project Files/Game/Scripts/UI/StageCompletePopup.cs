using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class StageCompletePopup : MonoBehaviour
    {
        private SimpleCallback onMaxFade;

        public void Show(SimpleCallback onMaxFade)
        {
            gameObject.SetActive(true);

            this.onMaxFade = onMaxFade;
        }

        public void OnMaxFade()
        {
            onMaxFade?.Invoke();
        }

        public void OnFadeDone()
        {
            gameObject.SetActive(false);
        }
    }
}
