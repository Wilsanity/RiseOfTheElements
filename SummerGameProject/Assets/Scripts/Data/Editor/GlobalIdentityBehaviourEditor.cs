using Kibo.NPCs;
using Kibo.Quests;
using System;
using UnityEditor;
using UnityEngine;

namespace Kibo.Data.Editor
{
    [CustomEditor(typeof(GlobalIdentityBehaviour<,>), true)]
    public class GlobalIdentityBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawGUID();

            DrawDefaultInspector();
        }

        protected void DrawGUID(bool includeButton = true)
        {
            string guid;
            Action assignNewGUID;
            if (target is NPC npc) { guid = npc.GUID; assignNewGUID = npc.GenerateGUID; }
            else if (target is Quest quest) { guid = quest.GUID; assignNewGUID = quest.GenerateGUID; }
            else return;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("GUID", guid);
            EditorGUI.EndDisabledGroup();
            if (includeButton && GUILayout.Button("Generate GUID")) assignNewGUID();
            EditorGUILayout.Space();
        }
    }
}