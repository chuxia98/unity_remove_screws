using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class LevelData : ScriptableObject
    {
        [Group("System")]
        [SerializeField] List<StageData> stages;
        public List<StageData> Stages => stages;

        
        [SerializeField, HideInInspector][Group("System")] int invalidStageIndex; //Level editor cached validation
        [SerializeField, HideInInspector][Group("System")] int presentHolesCount; //Level editor cached validation
        [SerializeField, HideInInspector][Group("System")] int requiredHolesCount; //Level editor cached validation

        [SerializeField] bool useInRandomizer = true;
        public bool UseInRandomizer => useInRandomizer;

        [SerializeField] IntToggle overrideLevelReward;
        public IntToggle OverrideLevelReward => overrideLevelReward;

        [SerializeField] PUPrice[] powerUpsReward;
        public PUPrice[] PowerUpsReward => powerUpsReward;
    }
}