using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointIndex;
    private CheckpointManager checkpointManager;

    void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        checkpointManager.SaveCheckpoint(checkpointIndex);
        Debug.Log("Checkpoint saved!: " + checkpointIndex);
    }
}
