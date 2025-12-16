using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    public class SavableHole : MonoBehaviour
    {
        [SerializeField] [OnValueChanged("UpdateScrew")] bool hasScrew;
        [SerializeField] [HideInInspector] GameObject screwRef;

        public bool HasScrew
        {
            get => hasScrew; set
            {
                hasScrew = value;
                UpdateScrew();
            }
        }

        public void UpdateScrew()
        {
#if UNITY_EDITOR
            if (hasScrew)
            {
                if(screwRef != null)
                {
                    return;
                }

                screwRef = PrefabUtility.InstantiatePrefab(EditorSceneController.Instance.ScrewPrefab) as GameObject;
                screwRef.transform.SetParent(transform);
                screwRef.transform.ResetLocal();
            }
            else
            {
                if(screwRef != null)
                {
                    DestroyImmediate(screwRef);
                }
            }
#endif
        }
    }
}
