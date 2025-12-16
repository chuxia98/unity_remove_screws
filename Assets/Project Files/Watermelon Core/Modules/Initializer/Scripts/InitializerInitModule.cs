using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Initializer Settings", true, order: 999)]
    public class InitializerInitModule : InitModule
    {
        public override string ModuleName => "Initializer Settings";

        [Tooltip("If manual mode is enabled, the loading screen will be active until GameLoading.MarkAsReadyToHide method has been called.")]
        [Header("Loading")]
        [SerializeField] bool manualControlMode;

        [Space]
        [SerializeField] GameObject systemMessagesPrefab;

        public override void CreateComponent()
        {
            if (manualControlMode)
                GameLoading.EnableManualControlMode();

            if(systemMessagesPrefab != null)
            {
                if(systemMessagesPrefab.GetComponent<SystemMessage>() != null)
                {
                    GameObject messagesCanvasObject = Instantiate(systemMessagesPrefab);
                    messagesCanvasObject.name = systemMessagesPrefab.name;
                    messagesCanvasObject.transform.SetParent(Initializer.Transform);
                }
                else
                {
                    CreateDummyMessagesObject();
                }
            }
            else
            {
                CreateDummyMessagesObject();
            }
        }

        private void CreateDummyMessagesObject()
        {
            SystemMessage dummyMessage = SystemMessage.CreateObject("[SYSTEM CANVAS]");
            dummyMessage.transform.SetParent(Initializer.Transform);
        }
    }
}
