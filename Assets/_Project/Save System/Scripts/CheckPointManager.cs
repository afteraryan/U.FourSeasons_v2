using System;
using UnityEngine;
using CharacterController = _Project.Player_Character.Script.CharacterController;


public class CheckpointManager : MonoBehaviour
{
    private const string LastCheckpointKey = "LastCheckpoint";

    [SerializeField] private CheckpointTrigger[] checkpoints; // Array of checkpoint transforms

    private void Start()
    {
        LoadCheckpoint();
        ToggleCheckpoints();
    }

    public void SaveCheckpoint(int checkpointIndex)
    {
        SaveManager.SaveCheckpoint(checkpointIndex);
    }

    private int LoadCheckpoint()
    {
        // Load the last checkpoint, defaulting to 0 if no data is found
        int lastCheckpoint = SaveManager.LoadCheckpoint();

        // Ensure the index is within bounds
        if (lastCheckpoint >= checkpoints.Length)
        {
            lastCheckpoint = 0; // Reset to first checkpoint if out of bounds
        }
        SetPlayerPosition(checkpoints[lastCheckpoint].transform.position);
        return lastCheckpoint;
    }

    public void ToggleCheckpoints()
    {
        int lastCheckpoint = LoadCheckpoint();
        foreach (CheckpointTrigger checkpoint in checkpoints)
        {
            if(checkpoint == checkpoints[lastCheckpoint])
                checkpoint.TurnOn();
            else
                checkpoint.TurnOff();
        }
    }
    
    
    
    private void SetPlayerPosition(Vector3 position)
    {
        // Set the player position to the given position
        FindObjectOfType<CharacterController>().transform.position = position;
    }
}