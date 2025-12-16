using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    [CustomPropertyDrawer(typeof(PlankData))]
    public class PlankDataPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private const string TYPE_PROPERTY_PATH = "type";
        private const string PREFAB_PROPERTY_PATH = "prefab";
        private const string USE_IN_QUICK_MODE_PROPERTY_PATH = "useInQuickMode";
        private const string QUICK_MODE_SIZE_PROPERTY_PATH = "quickModeSize";

        private string name;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect workRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            if(property.FindPropertyRelative(PREFAB_PROPERTY_PATH).objectReferenceValue != null)
            {
                name = property.FindPropertyRelative(PREFAB_PROPERTY_PATH).objectReferenceValue.name;
            }
            else
            {
                name = "[Null]"; 
            }

            property.isExpanded = EditorGUI.Foldout(workRect, property.isExpanded, name, true);
            workRect.y += EditorGUIUtility.singleLineHeight + 2;

            if (property.isExpanded)
            {
                EditorGUI.PropertyField(workRect, property.FindPropertyRelative(TYPE_PROPERTY_PATH));
                workRect.y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.PropertyField(workRect, property.FindPropertyRelative(PREFAB_PROPERTY_PATH));
                workRect.y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.LabelField(workRect, "Editor", EditorCustomStyles.labelBold);
                workRect.y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.PropertyField(workRect, property.FindPropertyRelative(USE_IN_QUICK_MODE_PROPERTY_PATH));
                workRect.y += EditorGUIUtility.singleLineHeight + 2;

                if (property.FindPropertyRelative(USE_IN_QUICK_MODE_PROPERTY_PATH).boolValue)
                {
                    EditorGUI.PropertyField(workRect, property.FindPropertyRelative(QUICK_MODE_SIZE_PROPERTY_PATH));
                    workRect.y += EditorGUIUtility.singleLineHeight + 2;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                if (property.FindPropertyRelative(USE_IN_QUICK_MODE_PROPERTY_PATH).boolValue)
                {
                    return (EditorGUIUtility.singleLineHeight + 2) * 6;
                }
                else
                {
                    return (EditorGUIUtility.singleLineHeight + 2) * 5;
                }
            }
            else
            {
                return (EditorGUIUtility.singleLineHeight + 2);
            }
        }
    }
}
