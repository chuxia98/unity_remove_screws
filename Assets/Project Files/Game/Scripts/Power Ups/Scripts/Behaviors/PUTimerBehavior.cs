using UnityEngine;

namespace Watermelon
{
    public class PUTimerBehavior : PUBehavior
    {
        private PUTimer timer;

        private PUTimerSettings timerSettings;

        public override void Init()
        {
            timerSettings = (PUTimerSettings)settings;

            timer = null;
        }

        public override bool IsActive()
        {
            return GameController.Data.GameplayTimerEnabled;
        }

        public override bool Activate()
        {
            IsBusy = true;

            LevelController.GameplayTimer.Pause();

            timer = new PUTimer(timerSettings.TimeFreezeDuration, () =>
            {
                timer = null;
                IsBusy = false;

                LevelController.GameplayTimer.Resume();
            });

            return true;
        }

        public override void ResetBehavior()
        {
            if(timer != null)
            {
                timer.Disable();
                timer = null;

                LevelController.GameplayTimer.Resume();
            }

            IsBusy = false;
        }

        public override PUTimer GetTimer()
        {
            return timer;
        }

        public override bool ApplyToElement(IClickableObject clickableObject, Vector3 clickPosition) => false;
        public override bool IsSelectable() => false;
    }
}
