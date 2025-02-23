using UnityEditor;
using UnityEngine;

namespace Kibo.Data.Editor
{
    [CustomEditor(typeof(DataManager))]
    public class DataManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DataManager dataManager = (DataManager)target;

            DrawDefaultInspector();

            if (GUILayout.Button("New Game")) dataManager.NewGame();
            if (GUILayout.Button("Save Game")) dataManager.SaveGame();
            if (GUILayout.Button("Load Game")) dataManager.LoadGame(dataManager.GameData.Name);
            if (GUILayout.Button("Delete Game")) dataManager.DeleteGame(dataManager.GameData.Name);
        }
    }
}