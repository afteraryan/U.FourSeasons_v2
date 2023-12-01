using UnityEngine;
using UnityEngine.UI;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private int checkpointIndex;
    private CheckpointManager checkpointManager;
    
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;
    private SpriteRenderer _spriteRenderer;
    private CheckpointManager _checkpointManager;

    void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _checkpointManager = FindObjectOfType<CheckpointManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        checkpointManager.SaveCheckpoint(checkpointIndex);
        _checkpointManager.ToggleCheckpoints();
        Debug.Log("Checkpoint saved!: " + checkpointIndex);
    }

    public void TurnOn()
    {
        _spriteRenderer.sprite = spriteOn;
        GetComponentInChildren<Collider2D>().enabled = false;
    }
    public void TurnOff()
    {
        _spriteRenderer.sprite = spriteOff;
        GetComponentInChildren<Collider2D>().enabled = true;
    }
}
