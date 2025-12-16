using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class StageData
    {
        [Group("System")]
        [SerializeField] List<HoleData> holePositions;
        public List<HoleData> HolePositions => holePositions;

        [Group("System")]
        [SerializeField] List<PlankLevelData> planksData;
        public List<PlankLevelData> PlanksData => planksData;

        [SerializeField] FloatToggle timerOverride;
        public bool TimerOverrideEnabled => timerOverride.enabled;
        public float TimerOverride => timerOverride.Handle(0);
    }
}
