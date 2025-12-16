using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public abstract class AbstractSkinsProvider : ScriptableObject
    {
        public abstract ISkinData[] Skins { get; }
        public abstract int SkinsCount { get; }

        public abstract ISkinData GetSkinData(int index);
        public abstract ISkinData GetSkinData(string id);

        public void Init()
        {
            for(int i = 0; i < SkinsCount; i++)
            {
                ISkinData skinData = GetSkinData(i);

                skinData.Initialise(this);
            }
        }
    }
}
