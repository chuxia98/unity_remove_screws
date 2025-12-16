using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Plank Skin Data", menuName = "Content/Skins/Plank Skin Data")]
    public class PlanksSkinData : ScriptableObject
    {
        [SerializeField] List<PlankData> planks = new List<PlankData>();
        public List<PlankData> Planks => planks;

        [Space]
        [SerializeField] GameObject plankHolePrefab;
        public GameObject PlankHolePrefab => plankHolePrefab;

        [SerializeField] GameObject baseHolePrefab;
        public GameObject BaseHolePrefab => baseHolePrefab;

        [SerializeField] GameObject screwPrefab;
        public GameObject ScrewPrefab => screwPrefab;

        [SerializeField] GameObject backPrefab;
        public GameObject BackPrefab => backPrefab;

        [SerializeField] List<Color> layerColors;

        public PlankData GetPlankData(PlankType plankType)
        {
            for (int i = 0; i < planks.Count; i++)
            {
                PlankData data = planks[i];

                if (data.Type == plankType) return data;
            }

            return null;
        }

        public Color GetLayerColor(int layerIndex)
        {
            return layerColors[layerIndex % layerColors.Count];
        }
    }
}
