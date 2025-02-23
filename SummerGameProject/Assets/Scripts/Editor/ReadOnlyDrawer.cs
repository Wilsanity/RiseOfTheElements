using Kibo.Attributes;
using UnityEditor;
using UnityEngine;

namespace Kibo.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                if (property.isExpanded) return (property.arraySize + 1) * EditorGUIUtility.singleLineHeight;
                
                return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}