using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Planks Skins Database", menuName = "Content/Skins/Planks Skins Database")]
    public class PlanksSkinsDatabase : AbstractSkinsProvider
    {
        [SerializeField] PlankSkinData[] skins;

        public override ISkinData[] Skins => skins;
        public override int SkinsCount => skins.Length;

        public override ISkinData GetSkinData(int index)
        {
            return skins[index];
        }

        public override ISkinData GetSkinData(string id)
        {
            for (int i = 0; i < skins.Length; i++)
            {
                PlankSkinData data = skins[i];

                if (data.ID == id) return data;
            }

            return null;
        }
    }
}