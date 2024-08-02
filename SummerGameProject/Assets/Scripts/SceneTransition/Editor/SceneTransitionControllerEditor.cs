
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneTransitionController))]
public class SceneTransitionControllerEditor : Editor
{
    SerializedProperty arrTransitionTypes;
    SerializedProperty _transitionTypeName;
    SerializedProperty _transitionGameObject;
    SerializedProperty animationClipToPlay;

    private void OnEnable()
    {
        arrTransitionTypes = serializedObject.FindProperty("arrTransitionTypes");
        animationClipToPlay = serializedObject.FindProperty("animationClipToPlay");

        SceneTransitionController scene = (SceneTransitionController)target;
        scene.UpdateTransitionTypeArray();
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Space(10);

        //--"Transition Type Animations" TITLE--//
        GUIStyle titleText = new GUIStyle();
        titleText.fontSize = 20;
        titleText.normal.textColor = Color.white;


        GUILayout.Label("Transition Type Animations", titleText);


        DrawSeparatorLine();


        for (int i = 0; i < arrTransitionTypes.arraySize; i++)
        {
            _transitionTypeName = arrTransitionTypes.GetArrayElementAtIndex(i).FindPropertyRelative("_transitionTypeName");
            _transitionGameObject = arrTransitionTypes.GetArrayElementAtIndex(i).FindPropertyRelative("_transitionGameObject");
 
            EditorGUILayout.PropertyField(_transitionGameObject, new GUIContent(_transitionTypeName.stringValue));
        }

        GUILayout.Space(20);

        EditorGUILayout.PropertyField(animationClipToPlay);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSeparatorLine( int spaceBeforeLine = 20, int spaceAfterLine = 10)
    {
        Rect lastRect = GUILayoutUtility.GetLastRect();
        Rect separatorLine = new Rect(lastRect.x, lastRect.y + spaceBeforeLine, lastRect.width, 2);
        EditorGUI.DrawRect(separatorLine, Color.white);
        GUILayout.Space(spaceAfterLine);
    }

    
}
