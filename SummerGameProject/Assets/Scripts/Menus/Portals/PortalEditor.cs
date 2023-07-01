using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using static UnityEngine.Animations.AimConstraint;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Portal))]
[CanEditMultipleObjects]

public class PortalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Portal portal = (Portal)target;

        portal.worldType = (WorldType)EditorGUILayout.EnumPopup("World Type", portal.worldType);

        EditorGUILayout.LabelField($"Portal ID: {portal.PortalID}");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Unlock Portal")) portal.UnlockPortal();
        if (GUILayout.Button("Lock Portal")) portal.LockPortal();
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Open Portal List")) 
        {
            Debug.LogWarning("This file will not update live while opened! Please close and reopen the file to see changes.");
            Application.OpenURL(portal.SaveDataPath);
        }
        
        if (GUILayout.Button("Clear List"))
        {
            if (EditorUtility.DisplayDialog("Are you sure?", "Clearing this data will remove all portals from the unlocked list. This action cannot be undone.", "Yes", "No"))
            {
                portal.ClearPortalData();
            }
        }
    }
}
