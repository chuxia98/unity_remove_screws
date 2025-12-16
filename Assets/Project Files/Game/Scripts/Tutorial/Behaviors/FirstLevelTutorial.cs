using UnityEngine;

namespace Watermelon
{
    public class FirstLevelTutorial : BaseTutorial
    {
        [Space]
        [SerializeField] Color textHighlightColor = Color.red;

        [Space]
        [SerializeField] string firstMessage = "<color={0}>Tap</color> to unscrew!";
        [SerializeField] string secondMessage = "<color={0}>Tap</color> to place screw!";

        public override bool IsActive => saveData.isActive;
        public override bool IsFinished => saveData.isFinished;
        public override int Progress => saveData.progress;

        private TutorialBaseSave saveData;

        private UIGame gameUI;

        private ScrewBehavior screw;
        private BaseHoleBehavior bottomHole;
        private BaseHoleBehavior topHole;

        public override void Init()
        {
            if (isInitialised) return;

            isInitialised = true;

            // Load save file
            saveData = SaveController.GetSaveObject<TutorialBaseSave>(string.Format(ITutorial.SAVE_IDENTIFIER, tutorialId.ToString()));

            gameUI = UIController.GetPage<UIGame>();
        }

        public override void FinishTutorial()
        {
            if (saveData.isFinished) return;

            saveData.isFinished = true;
        }

        public override void StartTutorial()
        {
            LevelController.LevelLoaded += OnLevelLoaded;
        }

        private void OnLevelLoaded()
        {
            LevelController.LevelLoaded -= OnLevelLoaded;

            // Store level elements
            screw = LevelController.StageLoader.Screws[0];
            topHole = LevelController.StageLoader.BaseHoles[0];
            bottomHole = LevelController.StageLoader.BaseHoles[1];

            // Pause gameplay timer
            LevelController.GameplayTimer.Pause();

            // Hide UI elements
            gameUI.ActivateTutorial();

            // Show message box and activate pointer under the bottom hole
            HighlightFirstHole();

            bottomHole.StateChanged += OnScrewRemoved;

            screw.Selected += OnScrewSelected;
            screw.Deselected += OnScrewDeselected;
        }

        private void HighlightFirstHole()
        {
            // Show message box and activate pointer under the bottom hole
            gameUI.MessageBox.Activate(string.Format(firstMessage, textHighlightColor.ToHex()), bottomHole.transform.position + new Vector3(0, -2.8f, 0));
            TutorialCanvasController.ActivatePointer(bottomHole.transform.position, TutorialCanvasController.POINTER_DEFAULT);
        }

        private void HighlightSecondsHole()
        {
            // Show message box and activate pointer above the top hole
            gameUI.MessageBox.Activate(string.Format(secondMessage, textHighlightColor.ToHex()), topHole.transform.position + new Vector3(0, 2.4f, 0));
            TutorialCanvasController.ActivatePointer(topHole.transform.position, TutorialCanvasController.POINTER_DEFAULT);
        }

        private void OnScrewDeselected()
        {
            if (IsFinished) return;

            HighlightFirstHole();
        }

        private void OnScrewSelected()
        {
            HighlightSecondsHole();
        }

        private void OnScrewRemoved(bool value)
        {
            if(!value)
            {
                gameUI.MessageBox.Disable();
                TutorialCanvasController.ResetPointer();

                AudioController.PlaySound(AudioController.AudioClips.tutorialComplete);

                ParticlesController.PlayParticle("Tutorial Complete");

                bottomHole.StateChanged -= OnScrewRemoved;
                screw.Selected -= OnScrewSelected;
                screw.Deselected -= OnScrewDeselected;

                FinishTutorial();
            }
        }

        public override void Unload()
        {

        }
    }
}
