using UnityEngine;
using CharacterController = _Project.Player_Character.Script.CharacterController;

public class MagicSock : MonoBehaviour
{
    [SerializeField] private MagicSockReward reward;
    [SerializeField] private int sockID; // Unique identifier for each Magic Sock
    
    private void Start()
    {
        // Check if the Magic Sock has already been collected and destroy it if so
        if (SaveManager.IsMagicSockCollected(sockID))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (SaveManager.IsMagicSockCollected(sockID))
        {
            Debug.Log("Magic Sock already collected.");
            return;
        }

        Debug.Log("Magic Sock Collected");
        switch (reward)
        {
            case MagicSockReward.JumpyShoes:
                other.GetComponentInParent<CharacterController>().WearJumpyShoes();
                break;
        }
        SaveManager.SaveMagicSockCollected(sockID);
        Destroy(gameObject);
    }
}

enum MagicSockReward
{
    JumpyShoes,
}