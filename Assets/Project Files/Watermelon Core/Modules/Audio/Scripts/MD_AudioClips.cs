using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Audio Clips", menuName = "Core/Audio Clips")]
    public class AudioClips : ScriptableObject
    {
        [BoxGroup("Gameplay", "Gameplay")]
        public AudioClip screwPick;
        [BoxGroup("Gameplay")]
        public AudioClip screwPlace;
        [BoxGroup("Gameplay")]
        public AudioClip plankComplete;
        [BoxGroup("Gameplay")]
        public AudioClip tutorialComplete;

        [BoxGroup("Stages", "Stages")]
        public AudioClip levelComplete;
        [BoxGroup("Stages")]
        public AudioClip stageComplete;
        [BoxGroup("Stages")]
        public AudioClip levelFailed;


        [BoxGroup("UI", "UI")]
        public AudioClip buttonSound;

    }
}

// -----------------
// Audio Controller v 0.4
// -----------------