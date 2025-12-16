using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class LevelController : MonoBehaviour
    {
        private static LevelController instance;

        [SerializeField] LevelDatabase database;

        private static bool isLevelLoaded;
        public static bool IsLevelLoaded => isLevelLoaded;

        private static LevelData level;
        public static LevelData Level => level;

        private static StageData stage;
        public static StageData Stage => stage;

        private static LevelSave levelSave;

        public static LevelDatabase Database => instance.database;

        public static int MaxReachedLevelIndex => levelSave.MaxReachedLevelIndex;
        public static int DisplayedLevelIndex => levelSave.DisplayLevelIndex;

        public static bool IsRaycastEnabled { get; set; } = true;

        private static bool firstTimeCompletedLevel = false;

        public static StageLoader StageLoader { get; private set; }
        private static SkinsManager skinManager;
        public static GameplayTimer GameplayTimer { get; private set; }

        private static int destroyedPlanksCounter = 0;
        private static int stageId;

        private Coroutine obstructedHolesChecker;

        public static event SimpleCallback LevelLoaded;
        public static event SimpleCallback LevelLeft;

        private void Awake()
        {
            instance = this;
        }

        public void Init()
        {
            levelSave = SaveController.GetSaveObject<LevelSave>("level");

            RaycastController raycastController = gameObject.AddComponent<RaycastController>();
            raycastController.Init();

            // Initialise special effects
            skinManager = new SkinsManager();
            StageLoader = new StageLoader(skinManager);
            GameplayTimer = new GameplayTimer();

            PlankSkinData skinData = (PlankSkinData)SkinsController.Instance.GetSelectedSkin<PlanksSkinsDatabase>();
            skinManager.LoadSkin(skinData.PlanksSkinData);

            SkinsController.SkinSelected += SkinSelected;

            GameplayTimer.OnTimerFinished += OnGameplayTimerFinished;
        }

        public static void ClampMaxReachedLevel()
        {
            levelSave.MaxReachedLevelIndex = Mathf.Clamp(levelSave.MaxReachedLevelIndex, 0, Database.AmountOfLevels - 1);
        }

        #region Load/Unload

        public void LoadLevel(int levelIndex, SimpleCallback onLevelLoaded = null)
        {
            UnloadLevel();

            int realLevelIndex;
            if (levelSave.IsPlayingRandomLevel && levelIndex == levelSave.DisplayLevelIndex && levelSave.RealLevelIndex != -1)
            {
                realLevelIndex = levelSave.RealLevelIndex;
            }
            else
            {
                realLevelIndex = database.GetRandomLevelIndex(levelIndex, levelSave.LastPlayerLevelIndex, false);

                levelSave.LastPlayerLevelIndex = realLevelIndex;
                levelSave.RealLevelIndex = realLevelIndex;

                if (realLevelIndex != levelIndex)
                {
                    levelSave.IsPlayingRandomLevel = true;
                }
            }

            levelSave.DisplayLevelIndex = levelIndex;
            firstTimeCompletedLevel = false;

            stageId = 0;

            level = database.GetLevel(realLevelIndex);
            stage = level.Stages[stageId];

            StageLoader.LoadStage(stage);
            destroyedPlanksCounter = 0;
            InitTimer();

            UIGame gameUI = UIController.GetPage<UIGame>();
            gameUI.PowerUpsUIController.OnLevelStarted(levelIndex + 1);
            gameUI.UpdateLevelNumber(levelIndex + 1);

            gameUI.SpawnLevelStages(level.Stages.Count);
            gameUI.SetActiveStage(0);

            RaycastController.Disable();

            isLevelLoaded = true;

            RaycastController.Enable();

            InitTimer();

            onLevelLoaded?.Invoke();

            LevelLoaded?.Invoke();

            obstructedHolesChecker = StartCoroutine(ObstructedHolesChecker());

            IsRaycastEnabled = true;

            SavePresets.CreateSave("Level " + (levelIndex + 1), "Levels", "Level " + (levelIndex + 1));
        }

        public static void UnloadLevel()
        {
            StageLoader.UnloadStage(false);
            PUController.ResetBehaviors();

            if (instance.obstructedHolesChecker != null) instance.StopCoroutine(ObstructedHolesChecker());

            LevelLeft?.Invoke();
        }

        public void ReloadStage()
        {
            StageLoader.UnloadStage(false);

            stage = level.Stages[stageId];

            destroyedPlanksCounter = 0;

            Tween.NextFrame(() =>
            {
                StageLoader.LoadStage(stage);

                UIController.GetPage<UIGame>().SetActiveStage(stageId);

                InitTimer();
            });
        }

        private void SkinSelected(ISkinData skinData)
        {
            if (skinData is PlankSkinData data)
            {
                skinManager.UnloadSkin();
                skinManager.LoadSkin(data.PlanksSkinData);
            }
        }

        #endregion

        #region Gameplay

        public static void ProcessClick(List<IClickableObject> clickableObjects2D)
        {
            int holesCounter = 0;
            int planksCounter = 0;

            List<HoleBehavior> holes = new List<HoleBehavior>();

            for (int i = 0; i < clickableObjects2D.Count; i++)
            {
                IClickableObject clickableObject = clickableObjects2D[i];

                if (clickableObject is HoleBehavior hole)
                {
                    holesCounter++;
                    holes.Add(hole);
                }
                else if (clickableObject is PlankBehavior)
                {
                    planksCounter++;
                }
            }

            if (holes.Count == planksCounter + 1)
            {
                bool alligned = true;

                if (holes.Count > 1)
                {
                    for (int i = 1; i < holes.Count; i++)
                    {
                        if (Vector2.Distance(holes[0].transform.position, holes[i].transform.position) > 0.1f)
                        {
                            alligned = false;
                            break;
                        }
                    }
                }

                if (ScrewBehavior.SelectedScrew != null && alligned)
                {
                    ScrewBehavior.SelectedScrew.ChangeHoles(holes);
                }
            }
        }

        public static void UpdateDestroyedPlanks()
        {
            destroyedPlanksCounter++;

            if (destroyedPlanksCounter == StageLoader.AmountOfPlanks)
            {
                stageId++;

                GameplayTimer.Reset();

                if (stageId >= level.Stages.Count)
                {
                    levelSave.IsPlayingRandomLevel = false;

                    levelSave.DisplayLevelIndex++;

                    if (levelSave.DisplayLevelIndex > levelSave.MaxReachedLevelIndex)
                    {
                        levelSave.MaxReachedLevelIndex = levelSave.DisplayLevelIndex;
                        firstTimeCompletedLevel = true;
                    }

                    PUPrice[] powerUpsReward = GetPUReward();
                    if (!powerUpsReward.IsNullOrEmpty())
                    {
                        foreach (PUPrice reward in powerUpsReward)
                        {
                            PUController.AddPowerUp(reward.PowerUpType, reward.Amount);
                        }
                    }

                    IsRaycastEnabled = false;

                    GameController.OnLevelCompleted();

                    AudioController.PlaySound(AudioController.AudioClips.levelComplete);
                }
                else
                {
                    UIController.GetPage<UIGame>().ShowStageComplete(() => {
                        StageLoader.UnloadStage(true);

                        stage = level.Stages[stageId];

                        destroyedPlanksCounter = 0;

                        Tween.NextFrame(() =>
                        {
                            StageLoader.LoadStage(stage);

                            UIController.GetPage<UIGame>().SetActiveStage(stageId);

                            InitTimer();
                        });

                        AudioController.PlaySound(AudioController.AudioClips.levelComplete);
                    });
                }
            }
        }

        private static IEnumerator ObstructedHolesChecker()
        {
            UIGame uiGame = UIController.GetPage<UIGame>();

            WaitForSeconds wait = new WaitForSeconds(0.5f);

            int counter = 0;
            while (true)
            {
                yield return wait;

                if (!GameController.IsGameActive) continue;

                bool hasMoves = false;

                for (int i = 0; i < StageLoader.BaseHoles.Count; i++)
                {
                    BaseHoleBehavior hole = StageLoader.BaseHoles[i];

                    if (!hole.IsActive)
                    {
                        List<IClickableObject> clickableObjects2D = RaycastController.HasOverlapCircle2D(hole.Position, hole.PhysicsRadius, 2560);

                        int holesCounter = 0;
                        int planksCounter = 0;

                        for (int j = 0; j < clickableObjects2D.Count; j++)
                        {
                            IClickableObject obj = clickableObjects2D[j];

                            if (obj is PlankHoleBehavior plankHole)
                            {
                                if (Vector2.Distance(hole.transform.position, plankHole.transform.position) > 0.1f)
                                {
                                    holesCounter = -1;
                                    break;
                                }

                                holesCounter++;
                            }
                            else if (obj is PlankBehavior)
                            {
                                planksCounter++;
                            }
                        }

                        if (holesCounter == planksCounter)
                        {
                            hasMoves = true;

                            break;
                        }
                    }
                }

                if (hasMoves)
                {
                    uiGame.HideNoMoreMovesIndicator();
                    counter = 0;
                }
                else if (counter == 5)
                {
                    uiGame.ShowNoMoreMovesIndicator();
                    counter++;
                }
                else
                {
                    counter++;
                }
            }
        }

        public static int GetCurrentLevelReward()
        {
            int reward = 0;

            if (level != null && level.OverrideLevelReward.enabled)
            {
                reward = level.OverrideLevelReward.newValue;
            }
            else
            {
                reward = GameController.Data.CoinsRewardPerLevel;
            }

            if (!firstTimeCompletedLevel)
            {
                reward = Mathf.RoundToInt(reward * (GameController.Data.ReplayingLevelRewardPercent.Handle(0) / 100.0f) / 10) * 10;
            }

            return reward;
        }
        #endregion

        #region PU

        public static void PlaceAdditionalBaseHole(Vector3 position)
        {
            StageLoader.PlaceAdditionalBaseHole(position);
        }

        public static PUPrice[] GetPUReward()
        {
            if (level == null) return null;
            if (!firstTimeCompletedLevel) return null;

            return level.PowerUpsReward;
        }

        #endregion

        #region Timer

        private static void InitTimer()
        {
            if (GameController.Data.GameplayTimerEnabled)
            {
                var time = GameController.Data.GameplayTimerValue;

                if (stage.TimerOverrideEnabled) time = stage.TimerOverride;

                GameplayTimer.SetMaxTime(time);

                GameplayTimer.Start();
            }
        }

        private void Update()
        {
            GameplayTimer.Update();
        }

        private static void OnGameplayTimerFinished()
        {
            IsRaycastEnabled = false;

            GameController.OnLevelFailed();

            AudioController.PlaySound(AudioController.AudioClips.levelFailed);
        }

        #endregion
    }
}