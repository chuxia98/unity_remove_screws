using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    [CustomEditor(typeof(InitializerInitModule))]
    public class InitializerInitModuleEditor : InitModuleEditor
    {
        public override void PrepareMenuItems(ref GenericMenu genericMenu)
        {
            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("Create Custom Messages Prefab"), false, () =>
            {
                SystemMessage systemMessage = SystemMessage.CreateObject("System Messages Canvas");

                string path = EditorUtility.SaveFilePanelInProject("Save Prefab", systemMessage.name, "prefab", "Please enter a file name to save the prefab to");

                if (!string.IsNullOrEmpty(path))
                {
                    // Save the object as a prefab at the selected path
                    GameObject prefabObject = PrefabUtility.SaveAsPrefabAsset(systemMessage.gameObject, path);

                    serializedObject.Update();
                    serializedObject.FindProperty("systemMessagesPrefab").objectReferenceValue = prefabObject;
                    serializedObject.ApplyModifiedProperties();
                }

                DestroyImmediate(systemMessage.gameObject);
            });
        }
    }
}
