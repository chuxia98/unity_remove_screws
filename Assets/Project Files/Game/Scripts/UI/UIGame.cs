using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIGame : UIPage
    {
        [BoxGroup("Safe Area", "Safe Area")]
        [SerializeField] RectTransform safeAreaRectTransform;
        [BoxGroup("Safe Area")]
        [SerializeField] CurrencyUIPanelSimple coinsPanel;
        [BoxGroup("Safe Area")]
        [SerializeField] UILevelNumberText levelNumberText;
        [BoxGroup("Safe Area")]
        [SerializeField] StagePanel stagePanel;
        [BoxGroup("Safe Area")]
        [SerializeField] TimerVisualiser gameplayTimer;
        [BoxGroup("Safe Area")]
        [SerializeField] CanvasGroup noMoreMovesIndicator;
        public TimerVisualiser GameplayTimer => gameplayTimer;

        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button exitButton;
        [BoxGroup("Buttons")]
        [SerializeField] UIFadeAnimation exitButtonFadeAnimation;
        [BoxGroup("Buttons")]
        [SerializeField] Button replayButton;
        [BoxGroup("Buttons")]
        [SerializeField] UIFadeAnimation replayButtonFadeAnimation;

        [BoxGroup("Power Ups", "Power Ups")]
        [SerializeField] PUUIController powerUpsUIController;
        public PUUIController PowerUpsUIController => powerUpsUIController;

        [BoxGroup("Popups", "Popups")]
        [SerializeField] ReplayPopupBehavior replayPopupBehavior;
        [BoxGroup("Popups")]
        [SerializeField] StageCompletePopup stageCompletePopup;

        [BoxGroup("Message Box", "Message Box")]
        [SerializeField] MessageBox messageBox;
        public MessageBox MessageBox => messageBox;

        [BoxGroup("Dev")]
        [SerializeField] GameObject devOverlay;

        private TweenCase noMoreMovesCase;

        public override void Init()
        {
            coinsPanel.Init();
            exitButton.onClick.AddListener(ShowExitPopUp);
            replayButton.onClick.AddListener(ShowReplayPopup);
            exitButtonFadeAnimation.Hide(immediately: true);
            replayButtonFadeAnimation.Hide(immediately: true);

            messageBox.Init();

            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);

            DevPanelEnabler.RegisterPanel(devOverlay);

            stagePanel.Init();
        }

        #region Show/Hide
        public override void PlayShowAnimation()
        {
            exitButton.gameObject.SetActive(true);
            levelNumberText.gameObject.SetActive(true);

            coinsPanel.Activate();
            exitButtonFadeAnimation.Show();

            replayButton.gameObject.SetActive(true);
            replayButtonFadeAnimation.Show();

            messageBox.Disable();

            UILevelNumberText.Show();

            if (GameController.Data.GameplayTimerEnabled)
            {
                gameplayTimer.Show(LevelController.GameplayTimer);
            }

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            coinsPanel.Disable();
            exitButtonFadeAnimation.Hide();
            replayButtonFadeAnimation.Hide(onCompleted: () => UIController.OnPageClosed(this));

            messageBox.Disable();

            UILevelNumberText.Hide();

            if (GameController.Data.GameplayTimerEnabled)
            {
                gameplayTimer.Hide();
            }

            if (noMoreMovesIndicator.gameObject.activeSelf) HideNoMoreMovesIndicator(true);
        }

        public void UpdateLevelNumber(int levelNumber)
        {
            levelNumberText.UpdateLevelNumber(levelNumber);
        }
        #endregion

        public void SpawnLevelStages(int stages)
        {
            stagePanel.Spawn(stages);
        }

        public void SetActiveStage(int stageIndex)
        {
            stagePanel.Activate(stageIndex);
        }

        public void GideLevelStages()
        {
            stagePanel.Clear();
        }

        public void ShowExitPopUp()
        {
            UIController.ShowPage<UIPause>();

            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        public void ShowReplayPopup()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            replayPopupBehavior.Show();
        }

        public void ShowNoMoreMovesIndicator()
        {
            if (noMoreMovesIndicator.gameObject.activeSelf && noMoreMovesIndicator.alpha == 1) return;

            noMoreMovesCase.KillActive();

            noMoreMovesIndicator.gameObject.SetActive(true);
            noMoreMovesCase = noMoreMovesIndicator.DOFade(1, 0.3f);
        }

        public void HideNoMoreMovesIndicator(bool instantly = false)
        {
            if (!noMoreMovesIndicator.gameObject.activeSelf) return;

            noMoreMovesCase.KillActive();

            if (instantly)
            {
                noMoreMovesIndicator.gameObject.SetActive(false);
                noMoreMovesIndicator.alpha = 0;
            }
            else
            {
                noMoreMovesCase = noMoreMovesIndicator.DOFade(0, 0.3f).OnComplete(() => {
                    noMoreMovesIndicator.gameObject.SetActive(false);
                });
            }
        }

        public void ShowStageComplete(SimpleCallback onMaxFade = null)
        {
            stageCompletePopup.Show(onMaxFade);
        }

        #region Tutorial
        public void ActivateTutorial()
        {
            exitButton.gameObject.SetActive(false);
            levelNumberText.gameObject.SetActive(false);
            replayButton.gameObject.SetActive(false);

            powerUpsUIController.HidePanels();

            if (GameController.Data.GameplayTimerEnabled)
            {
                gameplayTimer.Hide();
            }
        }
        #endregion

        #region Development

        public void ReloadDev()
        {
            GameController.ReplayLevel();
        }

        public void HideDev()
        {
            devOverlay.SetActive(false);
        }

        public void OnLevelInputUpdatedDev(string newLevel)
        {
            int level = -1;

            if (int.TryParse(newLevel, out level))
            {
                LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
                levelSave.DisplayLevelIndex = Mathf.Clamp((level - 1), 0, int.MaxValue);
                if (levelSave.DisplayLevelIndex >= LevelController.Database.AmountOfLevels)
                {
                    levelSave.DisplayLevelIndex = LevelController.Database.AmountOfLevels - 1;
                }
                levelSave.RealLevelIndex = levelSave.DisplayLevelIndex;

                GameController.ReplayLevel();
            }
        }

        public void PrevLevelDev()
        {
            LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
            levelSave.DisplayLevelIndex = Mathf.Clamp(levelSave.DisplayLevelIndex - 1, 0, int.MaxValue);
            if (levelSave.DisplayLevelIndex >= LevelController.Database.AmountOfLevels)
            {
                levelSave.DisplayLevelIndex = LevelController.Database.AmountOfLevels - 1;
            }
            levelSave.RealLevelIndex = levelSave.DisplayLevelIndex;

            GameController.ReplayLevel();
        }

        public void NextLevelDev()
        {
            LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
            
            levelSave.DisplayLevelIndex = levelSave.DisplayLevelIndex + 1;
            if (levelSave.DisplayLevelIndex >= LevelController.Database.AmountOfLevels)
            {
                levelSave.DisplayLevelIndex = LevelController.Database.AmountOfLevels - 1;
            }
            levelSave.RealLevelIndex = levelSave.DisplayLevelIndex;

            GameController.ReplayLevel();
        }
        #endregion
    }
}