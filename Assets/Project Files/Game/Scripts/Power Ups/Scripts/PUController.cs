using System.Collections.Generic;
using UnityEngine;
using Watermelon.IAPStore;

namespace Watermelon
{
    public class PUController : MonoBehaviour
    {
        private static PUController instance;

        [DrawReference]
        [SerializeField] PUDatabase database;

        [LineSpacer("Sounds")]
        [SerializeField] AudioClip activateSound;

        public static PUBehavior[] ActivePowerUps { get; private set; }
        public static PUUIController PowerUpsUIController { get; private set; }
        public static PUBehavior SelectedPU { get; private set; }

        private static Dictionary<PUType, PUBehavior> powerUpsLink;

        public static event PowerUpCallback Used;
        public static event PowerUpCallback Unlocked;

        private Transform behaviorsContainer;

        public void Init()
        {
#if MODULE_POWERUPS
            instance = this;

            behaviorsContainer = new GameObject("[POWER UPS]").transform;
            behaviorsContainer.gameObject.isStatic = true;

            PUSettings[] powerUpSettings = database.PowerUps;
            ActivePowerUps = new PUBehavior[powerUpSettings.Length];
            powerUpsLink = new Dictionary<PUType, PUBehavior>();

            for (int i = 0; i < ActivePowerUps.Length; i++)
            {
                // Initialise power ups
                powerUpSettings[i].InitialiseSave();
                powerUpSettings[i].Init();

                // Spawn behavior object 
                GameObject powerUpBehaviorObject = Instantiate(powerUpSettings[i].BehaviorPrefab, behaviorsContainer);
                powerUpBehaviorObject.transform.ResetLocal();

                PUBehavior powerUpBehavior = powerUpBehaviorObject.GetComponent<PUBehavior>();
                powerUpBehavior.InitialiseSettings(powerUpSettings[i]);
                powerUpBehavior.Init();

                ActivePowerUps[i] = powerUpBehavior;

                // Add power up to dictionary
                powerUpsLink.Add(ActivePowerUps[i].Settings.Type, ActivePowerUps[i]);
            }

            UIGame gameUI = UIController.GetPage<UIGame>();

            PowerUpsUIController = gameUI.PowerUpsUIController;
            PowerUpsUIController.Initialise(this);
#else
            Debug.LogError("[PU Controller]: Module Define isn't active!");
#endif
        }

        public static bool PurchasePowerUp(PUType powerUpType)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];
                if(powerUpBehavior.Settings.HasEnoughCurrency())
                {
                    CurrenciesController.Substract(powerUpBehavior.Settings.CurrencyType, powerUpBehavior.Settings.Price);

                    powerUpBehavior.Settings.Save.Amount += powerUpBehavior.Settings.PurchaseAmount;

                    PowerUpsUIController.RedrawPanels();

                    return true;
                }
                else
                {
                    UIController.ShowPage<UIStore>();

                    return false;
                }
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }

            return false;
        }

        public static void AddPowerUp(PUType powerUpType, int amount)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];

                powerUpBehavior.Settings.Save.Amount += amount;

                PowerUpsUIController.RedrawPanels();
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }
        }

        public static void SetPowerUpAmount(PUType powerUpType, int amount)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];

                powerUpBehavior.Settings.Save.Amount = amount;

                PowerUpsUIController.RedrawPanels();
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }
        }

        public static bool SelectPowerUp(PUType powerUpType)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];

                if(SelectedPU == powerUpBehavior)
                {
                    SelectedPU.OnDeselected(); 
                    SelectedPU = null;

                    return false;
                }

                if (!powerUpBehavior.IsBusy)
                {
                    if(SelectedPU != null)
                    {
                        SelectedPU.OnDeselected();
                        SelectedPU = null;
                    }

                    powerUpBehavior.OnSelected();

                    SelectedPU = powerUpBehavior;

                    return true;
                }
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }

            return false;
        }

        public static void ApplyToElement(IClickableObject clickableObject, Vector3 clickPosition)
        {
            if(SelectedPU != null)
            {
                if(SelectedPU.ApplyToElement(clickableObject, clickPosition))
                {
                    PUSettings settings = SelectedPU.Settings;

                    AudioController.PlaySound(settings.CustomAudioClip.Handle(instance.activateSound));

                    settings.Save.Amount--;

                    PowerUpsUIController.OnPowerUpUsed(SelectedPU);

                    Used?.Invoke(settings.Type);
                }

                SelectedPU.OnDeselected();
                SelectedPU = null;
            }
        }

        public static void UnselectPowerUp()
        {
            if(SelectedPU != null)
            {
                SelectedPU.OnDeselected();
                SelectedPU = null;
            }
        }

        public static bool UsePowerUp(PUType powerUpType)
        {
            if(powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];
                if(!powerUpBehavior.IsBusy)
                {
                    if(powerUpBehavior.Activate())
                    {
                        PUSettings settings = powerUpBehavior.Settings;

                        AudioController.PlaySound(settings.CustomAudioClip.Handle(instance.activateSound));

                        settings.Save.Amount--;

                        PowerUpsUIController.OnPowerUpUsed(powerUpBehavior);

                        Used?.Invoke(powerUpType);

                        return true;
                    }
                }
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }

            return false;
        }

        public static void ResetPowerUp(PUType powerUpType)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];

                powerUpBehavior.Settings.Save.Amount = 0;

                PowerUpsUIController.RedrawPanels();
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }
        }

        public static void UnlockPowerUp(PUType powerUpType)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];
                PUSettings settings = powerUpBehavior.Settings;

                if(!settings.IsUnlocked)
                {
                    settings.IsUnlocked = true;

                    Unlocked?.Invoke(powerUpType);
                }
            }
        }

        public static void ResetPowerUps()
        {
            foreach(PUBehavior powerUp in ActivePowerUps)
            {
                powerUp.Settings.Save.Amount = 0;
            }

            PowerUpsUIController.RedrawPanels();
        }

        public static PUBehavior GetPowerUpBehavior(PUType powerUpType)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                return powerUpsLink[powerUpType];
            }

            Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));

            return null;
        }

        public static void ResetBehaviors()
        {
            for(int i = 0; i < ActivePowerUps.Length; i++)
            {
                ActivePowerUps[i].ResetBehavior();
            }
        }

        [Button("Give Test Amount")]
        public void GiveDebugAmount()
        {
            if (!Application.isPlaying) return;

            for(int i = 0; i < ActivePowerUps.Length; i++)
            {
                ActivePowerUps[i].Settings.Save.Amount = 999;
            }

            PowerUpsUIController.RedrawPanels();
        }

        [Button("Reset Amount")]
        public void ResetDebugAmount()
        {
            if (!Application.isPlaying) return;

            for (int i = 0; i < ActivePowerUps.Length; i++)
            {
                ActivePowerUps[i].Settings.Save.Amount = 0;
            }

            PowerUpsUIController.RedrawPanels();
        }

        public delegate void PowerUpCallback(PUType powerUpType);
    }
}

// -----------------
// PU Controller v1.2.1
// -----------------

// Changelog
// v 1.2.1
// • Added notch offset on mobile devices
// • Added Show, Hide methods to PUUIController
// v 1.2
// • Added isDirty state for UI panels (redraws automatically in Update)
// • Added visuals for busy state
// v 1.1
// • Added ResetPowerUp, ResetPowerUps, SetPowerUpAmount, GetPowerUpBehavior methods
// v 1.0
// • Basic PU logic