using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class PURewardPanel
    {
        [SerializeField] RectTransform containerTransform;
        [SerializeField] GameObject powerUpPrefab;

        private Pool panelsPool;

        public void Init()
        {
            panelsPool = new Pool(powerUpPrefab, containerTransform);
        }
        
        public void Show(PUPrice[] rewards, float baseDelay)
        {
            if (rewards.IsNullOrEmpty())
            {
                containerTransform.gameObject.SetActive(false);

                return;
            }

            containerTransform.gameObject.SetActive(true);
            panelsPool.ReturnToPoolEverything();

            for(int i = 0; i < rewards.Length; i++)
            {
                GameObject uiElement = panelsPool.GetPooledObject();
                uiElement.transform.SetAsLastSibling();

                PURewardUIBehavior rewardUIBehavior = uiElement.GetComponent<PURewardUIBehavior>();
                rewardUIBehavior.Initialise(rewards[i]);

                uiElement.transform.localScale = Vector3.zero;
                uiElement.transform.DOScale(Vector3.one, 0.24f, baseDelay + 0.1f * i).SetEasing(Ease.Type.CubicOut);
            }
        }
    }
}
