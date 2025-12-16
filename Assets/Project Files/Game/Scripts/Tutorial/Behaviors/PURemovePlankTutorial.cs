using UnityEngine;

namespace Watermelon
{
    public class PURemovePlankTutorial : BaseTutorial
    {
        private readonly PUType POWER_UP_TYPE = PUType.DestroyPlank;

        [Space]
        [SerializeField] Color textHighlightColor = Color.red;

        [Space]
        [SerializeField] string firstMessage = "Use this booster to<br><color={0}>remove</color> a plank from the board.";
        [SerializeField] string secondMessage = "<color={0}>Tap</color> on plank to remove it!";

        public override bool IsActive => saveData.isActive;
        public override bool IsFinished => saveData.isFinished;
        public override int Progress => saveData.progress;

        private TutorialBaseSave saveData;

        private UIGame gameUI;

        private PUUIBehavior powerUpPanel;

        private bool isPUUsed;

        public override void Init()
        {
            if (isInitialised) return;

            isInitialised = true;

            // Load save file
            saveData = SaveController.GetSaveObject<TutorialBaseSave>(string.Format(ITutorial.SAVE_IDENTIFIER, tutorialId.ToString()));

            gameUI = UIController.GetPage<UIGame>();

            if (saveData.isFinished) return;

            PUController.Unlocked += OnPUUnlocked;
        }

        private void OnPUUnlocked(PUType powerUpType)
        {
            if (powerUpType != POWER_UP_TYPE) return;

            StartTutorial();
        }

        private void OnLevelLeft()
        {
            if (saveData.isFinished) return;

            saveData.isFinished = true;

            LevelController.LevelLeft -= OnLevelLeft;
            powerUpPanel.Behavior.SelectStateChanged -= OnSelectStateChanged;
            PUController.Used -= OnPUUsed;

            TutorialCanvasController.ResetTutorialCanvas();
            TutorialCanvasController.ResetPointer();

            gameUI.GameplayTimer.Hide();
        }

        public override void StartTutorial()
        {
            isPUUsed = false;

            LevelController.LevelLeft += OnLevelLeft;

            // Pause and hide gameplay timer
            LevelController.GameplayTimer.Pause();
            gameUI.GameplayTimer.Hide();

            powerUpPanel = gameUI.PowerUpsUIController.GetPanel(POWER_UP_TYPE);
            powerUpPanel.Behavior.Settings.Save.Amount = 1;
            powerUpPanel.Redraw();
            powerUpPanel.Behavior.SelectStateChanged += OnSelectStateChanged;

            HighlightPU();
        }

        private void OnSelectStateChanged(bool value)
        {
            if (isPUUsed) return;

            if(value)
            {
                TutorialCanvasController.ResetTutorialCanvas();
                TutorialCanvasController.ResetPointer();

                gameUI.MessageBox.Activate(string.Format(secondMessage, textHighlightColor.ToHex()), new Vector3(0, 5.2f, 0));

                PUController.Used += OnPUUsed;
            }
            else
            {
                HighlightPU();

                PUController.Used -= OnPUUsed;
            }
        }

        private void OnPUUsed(PUType powerUpType)
        {
            if (powerUpType != POWER_UP_TYPE) return;

            isPUUsed = true;

            powerUpPanel.Behavior.SelectStateChanged -= OnSelectStateChanged;
            PUController.Used -= OnPUUsed;

            gameUI.MessageBox.Disable();

            powerUpPanel.Behavior.Settings.Save.Amount = powerUpPanel.Behavior.Settings.DefaultAmount;

            TutorialCanvasController.ResetPointer();

            AudioController.PlaySound(AudioController.AudioClips.tutorialComplete);

            ParticlesController.PlayParticle("Tutorial Complete");

            FinishTutorial();
        }

        private void HighlightPU()
        {
            gameUI.MessageBox.Activate(string.Format(firstMessage, textHighlightColor.ToHex()), powerUpPanel.transform.position + new Vector3(0, 6f, 0));
            gameUI.MessageBox.ActivateTutorial();

            TutorialCanvasController.ActivateTutorialCanvas((RectTransform)powerUpPanel.transform, true, true);
            TutorialCanvasController.ActivatePointer(powerUpPanel.transform.position + new Vector3(0, 2.8f, 0), TutorialCanvasController.POINTER_SHOW_PU);
        }

        public override void FinishTutorial()
        {
            if (saveData.isFinished) return;

            saveData.isFinished = true;
        }

        public override void Unload()
        {

        }
    }
}
