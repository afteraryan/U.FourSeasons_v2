using UnityEngine;
using UnityEditor;

public class SaveManagerEditor : EditorWindow
{
    [MenuItem("Tools/SaveManager")]
    public static void ShowWindow()
    {
        GetWindow<SaveManagerEditor>("SaveManager");
    }

    void OnGUI()
    {
        GUILayout.Label("SaveManager Controls", EditorStyles.boldLabel);

        if (GUILayout.Button("Delete Physics Parameters"))
        {
            // Show a confirmation dialog before deletion
            if (EditorUtility.DisplayDialog("Confirm Deletion",
                    "Are you sure you want to delete the saved Physics Parameters?",
                    "Yes", "No"))
            {
                // Call the Delete function if "Yes" is clicked
                SaveManager.DeletePhysicsParameters();
            }
        }
        if (GUILayout.Button("Delete Checkpoint Data"))
        {
            // Show a confirmation dialog before deletion
            if (EditorUtility.DisplayDialog("Confirm Deletion",
                    "Are you sure you want to delete the saved Checkpoint Data?",
                    "Yes", "No"))
            {
                // Call the Delete function if "Yes" is clicked
                SaveManager.DeleteCheckpointData();
            }
        }
        if (GUILayout.Button("Delete Magic Sock Data"))
        {
            // Show a confirmation dialog before deletion
            if (EditorUtility.DisplayDialog("Confirm Deletion",
                    "Are you sure you want to delete the saved Magic Sock Data?",
                    "Yes", "No"))
            {
                // Call the Delete function if "Yes" is clicked
                SaveManager.ResetCollectedMagicSocks();
            }
        }
        if (GUILayout.Button("Delete ALL Data"))
        {
            // Show a confirmation dialog before deletion
            if (EditorUtility.DisplayDialog("Confirm Deletion",
                    "Are you sure you want to delete the ALL Data?",
                    "Yes", "No"))
            {
                // Call the Delete function if "Yes" is clicked
                SaveManager.DeleteAllData();
            }
        }
        
    }
}