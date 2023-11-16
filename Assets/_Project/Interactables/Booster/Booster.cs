using UnityEngine;
using CharacterController = _Project.Player_Character.Script.CharacterController;

public class Booster : MonoBehaviour
{
    [SerializeField] private float boostHeight;
    [SerializeField] private float extraBoostVelocity;
    [SerializeField] private float boostDuration;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision with: " + other.name + ", " + other.gameObject.layer);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collide");
            other.GetComponentInParent<CharacterController>().Boost(extraBoostVelocity, boostHeight, boostDuration);
            Destroy(gameObject);
        }
    }
}
