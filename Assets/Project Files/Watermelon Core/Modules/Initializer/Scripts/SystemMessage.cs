using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    [RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
    public class SystemMessage : MonoBehaviour
    {
        private static SystemMessage floatingMessage;

        [SerializeField] RectTransform panelRectTransform;
        [SerializeField] TextMeshProUGUI messageText;

        private TweenCase animationTweenCase;

        private CanvasGroup panelCanvasGroup;

        private void Start()
        {
            if (floatingMessage != null) return;

            floatingMessage = this;

            CanvasScaler canvasScaler = gameObject.GetComponent<CanvasScaler>();
            canvasScaler.MatchSize();

            panelCanvasGroup = gameObject.AddComponent<CanvasGroup>();

            messageText.AddEvent(EventTriggerType.PointerClick, (data) => OnPanelClick());

            panelRectTransform.gameObject.SetActive(false);
        }

        public void SetData(RectTransform panelRectTransform, TextMeshProUGUI messageText)
        {
            this.panelRectTransform = panelRectTransform;
            this.messageText = messageText;
        }

        private void OnPanelClick()
        {
            if (floatingMessage.animationTweenCase != null && !floatingMessage.animationTweenCase.IsCompleted)
                floatingMessage.animationTweenCase.Kill();

            floatingMessage.animationTweenCase = floatingMessage.panelCanvasGroup.DOFade(0, 0.3f, unscaledTime: true).SetEasing(Ease.Type.CircOut).OnComplete(delegate
            {
                floatingMessage.panelRectTransform.gameObject.SetActive(false);
            });
        }

        public static void ShowMessage(string message, float duration = 2.5f)
        {
            if(floatingMessage != null)
            {
                if (floatingMessage.animationTweenCase != null && !floatingMessage.animationTweenCase.IsCompleted)
                    floatingMessage.animationTweenCase.Kill();

                floatingMessage.messageText.text = message;

                floatingMessage.panelRectTransform.gameObject.SetActive(true);

                floatingMessage.panelCanvasGroup.alpha = 1.0f;
                floatingMessage.animationTweenCase = Tween.DelayedCall(duration, delegate
                {
                    floatingMessage.animationTweenCase = floatingMessage.panelCanvasGroup.DOFade(0, 0.5f, unscaledTime: true).SetEasing(Ease.Type.CircOut).OnComplete(delegate
                    {
                        floatingMessage.panelRectTransform.gameObject.SetActive(false);
                    });
                }, unscaledTime: true);
            }
            else
            {
                Debug.Log("[System Message]: " + message);
                Debug.LogError("[System Message]: ShowMessage() method has called, but module isn't initialized! Add Floating Message to Init scene.");
            }
        }

        public static SystemMessage CreateObject(string name)
        {
            // Base object
            GameObject systemCanvasObject = new GameObject(name);

            Canvas systemCanvas = systemCanvasObject.AddComponent<Canvas>();
            systemCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            systemCanvas.pixelPerfect = true;
            systemCanvas.sortingOrder = 999;

            CanvasScaler canvasScaler = systemCanvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            GraphicRaycaster graphicRaycaster = systemCanvasObject.AddComponent<GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;
            graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            // Panel
            GameObject panelObject = new GameObject("Panel");
            panelObject.transform.SetParent(systemCanvasObject.transform);

            RectTransform panelRectTransform = panelObject.AddComponent<RectTransform>();
            panelRectTransform.anchorMin = new Vector2(0, 1);
            panelRectTransform.anchorMax = new Vector2(1, 1);
            panelRectTransform.pivot = new Vector2(0.5f, 1.0f);
            panelRectTransform.sizeDelta = new Vector2(0, 185f);
            panelRectTransform.anchoredPosition = new Vector2(0, -600);

            Image panelImage = panelObject.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.6f);
            panelImage.raycastTarget = false;

            VerticalLayoutGroup panelVerticalGroup = panelObject.AddComponent<VerticalLayoutGroup>();
            panelVerticalGroup.padding = new RectOffset(0, 0, 40, 40);
            panelVerticalGroup.childControlHeight = true;
            panelVerticalGroup.childControlWidth = true;
            panelVerticalGroup.childForceExpandHeight = true;
            panelVerticalGroup.childForceExpandWidth = true;
            panelVerticalGroup.childAlignment = TextAnchor.MiddleCenter;

            ContentSizeFitter panelContentSizeFitter = panelObject.AddComponent<ContentSizeFitter>();
            panelContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Offset
            GameObject offsetObject = new GameObject("Offset");
            offsetObject.transform.SetParent(panelRectTransform);

            RectTransform offsetRectTransform = offsetObject.AddComponent<RectTransform>();
            offsetRectTransform.anchorMin = new Vector2(0, 0);
            offsetRectTransform.anchorMax = new Vector2(1, 1);
            offsetRectTransform.pivot = new Vector2(0.5f, 0.5f);
            offsetRectTransform.anchoredPosition = new Vector2(0, 0);

            VerticalLayoutGroup offsetVerticalGroup = offsetObject.AddComponent<VerticalLayoutGroup>();
            offsetVerticalGroup.padding = new RectOffset(150, 150, 0, 0);
            offsetVerticalGroup.childControlHeight = true;
            offsetVerticalGroup.childControlWidth = true;
            offsetVerticalGroup.childForceExpandHeight = true;
            offsetVerticalGroup.childForceExpandWidth = true;
            offsetVerticalGroup.childAlignment = TextAnchor.MiddleCenter;

            // Text
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(offsetRectTransform);

            RectTransform textRectTransform = textObject.AddComponent<RectTransform>();
            textRectTransform.anchorMin = new Vector2(0, 0);
            textRectTransform.anchorMax = new Vector2(1, 1);
            textRectTransform.pivot = new Vector2(0.5f, 0.5f);
            textRectTransform.sizeDelta = new Vector2(-30, 0);
            textRectTransform.anchoredPosition = new Vector2(0, 0);

            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Converted;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
            text.overflowMode = TextOverflowModes.Overflow;
            text.richText = true;
            text.fontSize = 52;

            // System Message Component
            SystemMessage systemMessage = systemCanvasObject.AddComponent<SystemMessage>();
            systemMessage.SetData(panelRectTransform, text);

            return systemMessage;
        }
    }
}