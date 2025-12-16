using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class PlankSkinData : AbstractSkinData
    {
        [SkinPreview]
        [SerializeField] Sprite previewSprite;
        public Sprite PreviewSprite => previewSprite;

        [SerializeField] PlanksSkinData planksSkinData;
        public PlanksSkinData PlanksSkinData => planksSkinData;
    }
}
