using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class SkinsManager
    {
        public PlanksSkinData Data { get; private set; }
        private bool IsSkinLoaded => Data != null;

        private Dictionary<PlankType, PoolGeneric<PlankBehavior>> plankPools;
        private PoolGeneric<ScrewBehavior> screwPool;
        private PoolGeneric<BaseHoleBehavior> baseHolePool;
        private PoolGeneric<PlankHoleBehavior> plankHolePool;
        private PoolGeneric<BaseBehavior> baseBoardPool;

        public void LoadSkin(PlanksSkinData data)
        {
            Data = data;

            plankPools = new Dictionary<PlankType, PoolGeneric<PlankBehavior>>();

            for(int i = 0; i < data.Planks.Count; i++)
            {
                PlankData plankData = data.Planks[i];

                PoolGeneric<PlankBehavior> plankPool = new PoolGeneric<PlankBehavior>(plankData.Prefab, $"{plankData.Type}");

                plankPool.Init();

                plankPools.Add(plankData.Type, plankPool);
            }

            screwPool = new PoolGeneric<ScrewBehavior>(data.ScrewPrefab, "Screw");
            screwPool.Init();

            plankHolePool = new PoolGeneric<PlankHoleBehavior>(data.PlankHolePrefab, "Plank Hole");
            plankHolePool.Init();

            baseHolePool = new PoolGeneric<BaseHoleBehavior>(data.BaseHolePrefab, "Base Hole");
            baseHolePool.Init();

            baseBoardPool = new PoolGeneric<BaseBehavior>(data.BackPrefab, "Base Board");
            baseBoardPool.Init();
        }

        public PlankBehavior GetPlank(PlankType plankType)
        {
            if (!IsSkinLoaded) return null;

            if(!plankPools.ContainsKey(plankType)) return null;

            return plankPools[plankType].GetPooledComponent();
        }

        public ScrewBehavior GetScrew()
        {
            if(!IsSkinLoaded) return null;

            return screwPool.GetPooledComponent();
        }

        public BaseHoleBehavior GetBaseHole()
        {
            if(!IsSkinLoaded) return null;

            return baseHolePool.GetPooledComponent();
        }

        public PlankHoleBehavior GetPlankHole()
        {
            if (!IsSkinLoaded) return null;

            return plankHolePool.GetPooledComponent();
        }

        public BaseBehavior GetBase()
        {
            if(!IsSkinLoaded) return null;

            return baseBoardPool.GetPooledComponent();
        }

        public Color GetLayerPlankColor(int layerId)
        {
            return Data.GetLayerColor(layerId);
        }

        public void UnloadSkin()
        {
            if(!IsSkinLoaded) return;

            plankHolePool.ReturnToPoolEverything();
            PoolManager.DestroyPool(plankHolePool);

            baseHolePool.ReturnToPoolEverything();
            PoolManager.DestroyPool(baseHolePool);

            screwPool.ReturnToPoolEverything();
            PoolManager.DestroyPool(screwPool);

            baseBoardPool.ReturnToPoolEverything();
            PoolManager.DestroyPool(baseBoardPool);

            foreach(PoolGeneric<PlankBehavior> plankPool in plankPools.Values)
            {
                plankPool.ReturnToPoolEverything();
                PoolManager.DestroyPool(plankPool);
            }

            Data = null;
        }
    }
}