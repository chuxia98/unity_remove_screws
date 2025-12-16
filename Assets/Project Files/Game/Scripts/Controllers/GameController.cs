using System;
using System.Reflection;
using UnityEngine;
using Watermelon.Map;
using Watermelon.SkinStore;

namespace Watermelon
{
    public class GameController : MonoBehaviour
    {
        private static GameController gameController;

        [DrawReference]
        [SerializeField] GameData data;

        [LineSpacer]
        [SerializeField] UIController uiController;
        [SerializeField] MusicSource musicSource;

        private LevelController levelController;
        private ParticlesController particlesController;
        private FloatingTextController floatingTextController;
        private PUController powerUpController;
        private MapBehavior mapBehavior;
        private TutorialController tutorialController;
        private SkinsController skinController;
        private SkinStoreController skinStoreController;

        public static GameData Data => gameController.data;

        private static bool isGameActive;
        public static bool IsGameActive => isGameActive;

        private void Awake()
        {
            gameController = this;

            // Cache components
            CacheComponent(out particlesController);
            CacheComponent(out floatingTextController);
            CacheComponent(out levelController);
            CacheComponent(out powerUpController);
            CacheComponent(out mapBehavior);
            CacheComponent(out tutorialController);
            CacheComponent(out skinController);
            CacheComponent(out skinStoreController);

            uiController.Init();

            particlesController.Init();
            floatingTextController.Init();
            musicSource.Init();

            skinController.Init();
            skinStoreController.Init(skinController);
            powerUpController.Init();
            levelController.Init();
            tutorialController.Init();

            uiController.InitPages();

            musicSource.Activate();
        }

        private void Start()
        {
            AdsManager.EnableBanner();

            if (LevelAutoRun.CheckIfNeedToAutoRunLevel())
            {
                LoadLevel(LevelAutoRun.GetLevelIndex());
            }
            else
            {
                if (LevelController.DisplayedLevelIndex == 0 && data.ShowTutorial)
                {
                    ITutorial firstLevelTutorial = TutorialController.GetTutorial(TutorialID.FirstLevel);

                    if (firstLevelTutorial != null && !firstLevelTutorial.IsFinished)
                    {
                        firstLevelTutorial.StartTutorial();

                        LoadLevel(0);
                    }
                    else
                    {
                        OpenMainMenu();
                    }
                }
                else
                {
                    OpenMainMenu();
                }
            }

            GameLoading.MarkAsReadyToHide();
        }

        private void OpenMainMenu()
        {
            mapBehavior.Show();

            // Display default page
            UIController.ShowPage<UIMainMenu>();
        }

        public static void LoadLevel(int index, SimpleCallback onLevelLoaded = null)
        {
            AdsManager.ShowInterstitial(null);

            gameController.mapBehavior.Hide();

            AdsManager.EnableBanner();

            UIController.ShowPage<UIGame>();

            isGameActive = true;

            gameController.levelController.LoadLevel(index, onLevelLoaded);
        }

        public static void OnLevelCompleted()
        {
            if (!isGameActive)
                return;

            UIGame gameUI = UIController.GetPage<UIGame>();
            gameUI.PowerUpsUIController.OnLevelFinished();

            UIController.HidePage<UIGame>(() =>
            {
                UIController.ShowPage<UIComplete>();
            });

            isGameActive = false;
        }

        public static void OnLevelFailed()
        {
            if (!isGameActive)
                return;

            UIGame gameUI = UIController.GetPage<UIGame>();
            gameUI.PowerUpsUIController.OnLevelFinished();

            UIController.HidePage<UIGame>(() =>
            {
                UIController.ShowPage<UIGameOver>();
            });

            isGameActive = false;
        }

        public static void LoadNextLevel(SimpleCallback onLevelLoaded = null)
        {
            LoadLevel(LevelController.DisplayedLevelIndex, onLevelLoaded);
        }

        public static void ReplayLevel()
        {
            isGameActive = false;

            Overlay.Show(0.3f, () =>
            {
                LoadLevel(LevelController.DisplayedLevelIndex);

                Overlay.Hide(0.3f);
            }, true);
        }

        public static void ReplayStage()
        {
            isGameActive = true;

            gameController.levelController.ReloadStage();
        }

        public static void ReturnToMenu()
        {
            isGameActive = false;

            LevelController.UnloadLevel();

            gameController.mapBehavior.Show();

            AdsManager.ShowInterstitial(null);

            UIController.ShowPage<UIMainMenu>();

            AdsManager.EnableBanner();
        }

        public static void Revive()
        {
            isGameActive = true;
            LevelController.IsRaycastEnabled = true;
        }

        #region Extensions
        public bool CacheComponent<T>(out T component) where T : Component
        {
            Component unboxedComponent = gameObject.GetComponent(typeof(T));

            if (unboxedComponent != null)
            {
                component = (T)unboxedComponent;

                return true;
            }

            Debug.LogError(string.Format("Scripts Holder doesn't have {0} script added to it", typeof(T)));

            component = null;

            return false;
        }
        #endregion
    }
}