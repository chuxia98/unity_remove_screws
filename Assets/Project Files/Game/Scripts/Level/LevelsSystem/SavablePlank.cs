using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class SavablePlank : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] PlankType plankType;
        [SerializeField] [OnValueChanged("HandleManualLayerChange")] int plankLayer;
        [SerializeField] [HideInInspector] int actualPlankLayer;
        [SerializeField] List<Vector3> screwsPositions;

        public PlankType PlankType { get => plankType; set => plankType = value; }
        public int PlankLayer
        {
            get => actualPlankLayer; 
            set
            {
                actualPlankLayer = value;
                plankLayer = value + 1;

                if (actualPlankLayer > EditorSceneController.Instance.CurrentLayerIndex)
                {
                    EditorSceneController.Instance.CurrentLayerIndex = actualPlankLayer;

                    if (EditorSceneController.Instance.CurrentLayerIndex > EditorSceneController.Instance.MaxLayerIndex)
                    {
                        EditorSceneController.Instance.MaxLayerIndex = EditorSceneController.Instance.CurrentLayerIndex;
                    }
                }

                LayerUpdate();
            }
        }
        public List<Vector3> ScrewsPositions { get => screwsPositions; set => screwsPositions = value; }

        public void LayerUpdate()
        {
            Color newColor = EditorSceneController.Instance.LayerColors[Mathf.Clamp(actualPlankLayer, 0, EditorSceneController.Instance.LayerColors.Length - 1)];
            SpriteRenderer[] renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
            bool isVisible = (actualPlankLayer <= EditorSceneController.Instance.CurrentLayerIndex);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (isVisible)
                {
                    renderers[i].color = newColor;
                    
                }
                else //make it barely visible
                {
                    renderers[i].color = newColor.SetAlpha(0.2f);
                }

                renderers[i].sortingOrder = plankLayer;
            }

            if (isVisible)
            {
                UnityEditor.SceneVisibilityManager.instance.EnablePicking(gameObject, true);
            }
            else
            {
                UnityEditor.SceneVisibilityManager.instance.DisablePicking(gameObject, true);
            }
        }

        private void HandleManualLayerChange()
        {
            PlankLayer = plankLayer - 1;
        }
#endif
    }
}
