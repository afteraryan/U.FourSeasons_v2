using System;
using UnityEngine;
using CharacterController = _Project.Player_Character.Script.CharacterController;


public class CheckpointManager : MonoBehaviour
{
    private const string LastCheckpointKey = "LastCheckpoint";
    private const string BackupCheckpointKey = "BackupCheckpoint";

    [SerializeField] private Transform[] checkpoints; // Array of checkpoint transforms

    private void Awake()
    {
        LoadCheckpoint();
    }

    public void SaveCheckpoint(int checkpointIndex)
    {
        // Save the current checkpoint index and a backup
        PlayerPrefs.SetInt(BackupCheckpointKey, PlayerPrefs.GetInt(LastCheckpointKey, 0));
        PlayerPrefs.SetInt(LastCheckpointKey, checkpointIndex);
        PlayerPrefs.Save();
    }

    public int LoadCheckpoint()
    {
        // Load the last checkpoint, or use backup if corrupted/invalid
        int lastCheckpoint = PlayerPrefs.GetInt(LastCheckpointKey, -1);
        if (lastCheckpoint < 0 || lastCheckpoint >= checkpoints.Length)
        {
            // Load backup if the last checkpoint is invalid
            lastCheckpoint = PlayerPrefs.GetInt(BackupCheckpointKey, 0);
        }
        SetPlayerPosition(checkpoints[lastCheckpoint].position);
        return lastCheckpoint;
    }

    public void ResetCheckpoints()
    {
        // Clear both primary and backup checkpoint data
        PlayerPrefs.DeleteKey(LastCheckpointKey);
        PlayerPrefs.DeleteKey(BackupCheckpointKey);
    }
    
    private void SetPlayerPosition(Vector3 position)
    {
        // Set the player position to the given position
        FindObjectOfType<CharacterController>().transform.position = position;
    }
}