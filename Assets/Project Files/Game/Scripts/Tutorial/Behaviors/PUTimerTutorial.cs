using UnityEngine;

namespace Watermelon
{
    public class PUTimerTutorial : BaseTutorial
    {
        private readonly PUType POWER_UP_TYPE = PUType.Timer;

        [Space]
        [SerializeField] Color textHighlightColor = Color.red;

        [Space]
        [SerializeField] string firstMessage = "Use this booster to<br>freeze the timer.";

        public override bool IsActive => saveData.isActive;
        public override bool IsFinished => saveData.isFinished;
        public override int Progress => saveData.progress;

        private TutorialBaseSave saveData;

        private UIGame gameUI;

        private PUUIBehavior powerUpPanel;

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
            PUController.Used -= OnPUUsed;

            TutorialCanvasController.ResetTutorialCanvas();
            TutorialCanvasController.ResetPointer();

            gameUI.GameplayTimer.Hide();
        }

        public override void StartTutorial()
        {
            powerUpPanel = gameUI.PowerUpsUIController.GetPanel(POWER_UP_TYPE);
            powerUpPanel.Behavior.Settings.Save.Amount = 1;
            powerUpPanel.Redraw();

            LevelController.LevelLeft += OnLevelLeft;
            PUController.Used += OnPUUsed;

            HighlightPU();
        }

        private void OnPUUsed(PUType powerUpType)
        {
            if (powerUpType != POWER_UP_TYPE) return;

            PUController.Used -= OnPUUsed;

            gameUI.MessageBox.Disable();

            TutorialCanvasController.ResetPointer();
            TutorialCanvasController.ResetTutorialCanvas();

            powerUpPanel.Behavior.Settings.Save.Amount = powerUpPanel.Behavior.Settings.DefaultAmount;

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
