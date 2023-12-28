using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using static UnityEngine.Animations.AimConstraint;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Portal))]
[CanEditMultipleObjects]

/*
    This script changes how the 'Portal' Component apears in the inspector.
    It adds functioning buttons to test the portal functionality while using the editor.
*/

public class PortalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // target portal object
        Portal portal = (Portal)target;

        // world type of the portal is set by and enum popup.
        portal.worldType = (WorldType)EditorGUILayout.EnumPopup("World Type", portal.worldType);

        // displays the portal's ID in the inspector
        EditorGUILayout.LabelField($"Portal ID: {portal.PortalID}");

        EditorGUILayout.Space();

        // first actions of the portal are set by an enum pop up
        portal.firstUseActions = (FirstUseActions)EditorGUILayout.EnumPopup("First Use Action", portal.firstUseActions);

        // if a first action is selected, here is where you will display the values relivent to the functionality in the inspector.
        // this hides any unused variables to prevent overcrouding with useless variables that aren't in use
        switch (portal.firstUseActions)
        {
            // No functionality
            case FirstUseActions.None: 
                break;

            // Teleport Player
            case FirstUseActions.TeleportPlayer:

                // requires scene name to load the correct level
                portal.FA_SceneName = EditorGUILayout.TextField("Scene Name", portal.FA_SceneName);

                // the name of the object the player will spawn at
                portal.FA_WaypointName = EditorGUILayout.TextField("Waypoint Name", portal.FA_WaypointName);
                break;

            // No valid first action displayed in the inspector
            default: 
                Debug.LogError($"No Display Case for First Action on {portal.name} Inspector! Go To PortalEditor Script, Line 38 to add your case!"); 
                break; 
        }

        EditorGUILayout.Space();

        // Portal Save Data Section
        EditorGUILayout.LabelField($"Portal Save Data");
        EditorGUILayout.BeginHorizontal();

        // Buttons to Lock and Unlock portals are available in the inspector for ease of use
        if (GUILayout.Button("Unlock Portal")) portal.UnlockPortal();
        if (GUILayout.Button("Lock Portal")) portal.LockPortal();

        EditorGUILayout.EndHorizontal();

        // Opens a text file of the saved portals list
        if (GUILayout.Button("Open Portal List")) 
        {
            // The list will not update live while it is opened. For updates it will need to be closed and reopened.
            Debug.LogWarning("This file will not update live while opened! Please close and reopen the file to see changes.");

            // opens the file from the file path
            Application.OpenURL(portal.SaveDataPath);
        }
        
        // will clear any and all portal save data on the text file
        if (GUILayout.Button("Clear List"))
        {
            // a pop up to confirm the action will display
            if (EditorUtility.DisplayDialog("Are you sure?", "Clearing this data will remove all portals from the unlocked list. This action cannot be undone.", "Yes", "No"))
            {
                // clears the data file
                portal.ClearPortalData();
            }
        }

        // sets is dirty to any object to save and changed values
        if (GUI.changed && !Application.isPlaying)
        {
            EditorUtility.SetDirty(portal);
            EditorSceneManager.MarkSceneDirty(portal.gameObject.scene);
        }
    }
}
