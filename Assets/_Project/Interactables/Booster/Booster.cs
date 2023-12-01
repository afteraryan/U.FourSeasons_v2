using UnityEngine;
using CharacterController = _Project.Player_Character.Script.CharacterController;

public class Booster : MonoBehaviour
{
    [SerializeField] private float boostHeight;
    [SerializeField] private float extraBoostVelocity;
    [SerializeField] private float boostDuration;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        CharacterController characterController = other.GetComponentInParent<CharacterController>();
        if (characterController.GetIsFalling())
            return;
        
        characterController.Boost(extraBoostVelocity, boostHeight, boostDuration);
        //Destroy(gameObject);
    }
}
