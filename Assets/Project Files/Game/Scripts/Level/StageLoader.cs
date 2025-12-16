using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Watermelon
{
    public class StageLoader
    {
        public List<BaseHoleBehavior> BaseHoles { get; private set; }
        public List<PlankBehavior> Planks { get; private set; }
        public List<ScrewBehavior> Screws { get; private set; }

        private BaseBehavior baseBoard;

        public int AmountOfPlanks => Planks.Count;

        public bool StageLoaded { get; private set; }

        private SkinsManager skinsManager;

        public StageLoader(SkinsManager skinsManager)
        {
            this.skinsManager = skinsManager;
        }

        public void LoadStage(StageData levelData)
        {
            float fixedDeltaTile = Time.fixedDeltaTime;
            Time.fixedDeltaTime = 10;

            BaseHoles = new List<BaseHoleBehavior>();
            Planks = new List<PlankBehavior>();
            Screws = new List<ScrewBehavior>();

            for (int i = 0; i < levelData.HolePositions.Count; i++)
            {
                HoleData holeData = levelData.HolePositions[i];

                BaseHoleBehavior baseHole = skinsManager.GetBaseHole();
                baseHole.Init(holeData);
                BaseHoles.Add(baseHole);
            }

            levelData.PlanksData.Sort((data1, data2) => data1.PlankLayer - data2.PlankLayer);

            for (int i = 0; i < levelData.PlanksData.Count; i++)
            {
                PlankLevelData plankLevelData = levelData.PlanksData[i];

                PlankBehavior plank = skinsManager.GetPlank(plankLevelData.PlankType);

                Color plankColor = skinsManager.GetLayerPlankColor(plankLevelData.PlankLayer);

                plank.Init(plankLevelData, plankColor, i);
                plank.SetHoles(plankLevelData.ScrewsPositions, skinsManager);
                Planks.Add(plank);
            }

            for(int i = 0; i < Planks.Count; i++)
            {
                PlankBehavior firstPlank = Planks[i];
                for(int j = i + 1; j < Planks.Count; j++)
                {
                    PlankBehavior secondPlank = Planks[j];

                    if(firstPlank.Layer != secondPlank.Layer)
                    {
                        firstPlank.IgnorePlank(secondPlank);
                    } else
                    {
                        firstPlank.CollideWithPlank(secondPlank);
                    }
                }
            }

            for (int i = 0; i < levelData.HolePositions.Count; i++)
            {
                HoleData holeData = levelData.HolePositions[i];

                if (holeData.HasScrew)
                {
                    ScrewBehavior screw = skinsManager.GetScrew();

                    screw.transform.position = holeData.Position.SetZ(0);
                    screw.Init(BaseHoles, Planks);

                    Screws.Add(screw);
                }
            }

            for (int i = 0; i < Planks.Count; i++)
            {
                PlankBehavior plank = Planks[i];

                plank.EnableColliders();
            }

            baseBoard = skinsManager.GetBase();
            baseBoard.transform.position = Vector3.forward;

            Tween.DelayedCall(0.2f, () => {
                Time.fixedDeltaTime = fixedDeltaTile; 

                for(int i = 0; i < Planks.Count;i++)
                {
                    Planks[i].StartSimulation();
                }

                for(int i = 0; i < Screws.Count; i++)
                {
                    Screws[i].EnableCollider();
                }
            });

            StageLoaded = true;
        }

        public void PlaceAdditionalBaseHole(Vector2 position)
        {
            HoleData holeData = new HoleData(position, false);

            BaseHoleBehavior baseHole = skinsManager.GetBaseHole();
            baseHole.Init(holeData);
            BaseHoles.Add(baseHole);
        }

        public void UnloadStage(bool withParticle)
        {
            if (!StageLoaded) return;

            for (int i = 0; i < Planks.Count; i++)
            {
                if (Planks[i] != null && Planks[i].gameObject.activeSelf) Planks[i].Discard(withParticle);
            }
            for(int i = 0; i < BaseHoles.Count; i++)
            {
                if (BaseHoles[i] != null) BaseHoles[i].Discard();
            }

            if (ScrewBehavior.SelectedScrew != null) ScrewBehavior.SelectedScrew.Deselect();
            for(int i = 0; i < Screws.Count; i++)
            {
                if (Screws[i] != null) Screws[i].Discard();
            }

            baseBoard.Discard();

            Planks.Clear();
            BaseHoles.Clear();
            Screws.Clear();

            StageLoaded = false;
        }
    }  
}
