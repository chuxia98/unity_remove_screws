using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    [CustomEditor(typeof(AbstractSkinsProvider), true)]
    public class AbstractSkinsProviderEditor : CustomInspector
    {
        private SkinsHandler skinsHandler;
        private bool isRegistered;

        protected override void OnEnable()
        {
            base.OnEnable();

            skinsHandler = EditorUtils.GetAsset<SkinsHandler>();

            if(skinsHandler != null)
            {
                isRegistered = skinsHandler.HasSkinsProvider((AbstractSkinsProvider)target);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(skinsHandler == null)
            {
                GUILayout.Space(12);

                EditorGUILayout.BeginVertical();

                EditorGUILayout.HelpBox("SkinsHandler can't be found in the project!\nCreate it using the context menu Content/Skins/Skins Handler", MessageType.Error);

                EditorGUILayout.EndVertical();
            }
            else
            {
                if (!isRegistered)
                {
                    GUILayout.Space(12);

                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.HelpBox("This database isn't linked to SkinsHandler", MessageType.Warning);

                    if (GUILayout.Button("Add to Skins Handler"))
                    {
                        SerializedObject skinsHandlerSerializedObject = new SerializedObject(skinsHandler);

                        skinsHandlerSerializedObject.Update();

                        SerializedProperty providersProperty = skinsHandlerSerializedObject.FindProperty("skinProviders");
                        int index = providersProperty.arraySize;

                        providersProperty.arraySize = index + 1;

                        SerializedProperty providerProperty = providersProperty.GetArrayElementAtIndex(index);
                        providerProperty.objectReferenceValue = target;

                        skinsHandlerSerializedObject.ApplyModifiedProperties();

                        isRegistered = true;
                    }

                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}
