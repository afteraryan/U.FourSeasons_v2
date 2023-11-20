using UnityEngine;
using CharacterController = _Project.Player_Character.Script.CharacterController;

public class MagicSock : MonoBehaviour
{
    [SerializeField] private MagicSockReward reward;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (reward)
        {
            case MagicSockReward.JumpyShoes:
                other.GetComponentInParent<CharacterController>().WearJumpyShoes();
                break;
        }
        Destroy(gameObject);
    }
}

enum MagicSockReward
{
    JumpyShoes,
}
