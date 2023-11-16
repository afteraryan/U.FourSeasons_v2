using UnityEngine;
using CharacterController = _Project.Player_Character.Script.CharacterController;

public class JumpingFlower : MonoBehaviour
{
    [SerializeField] private float boostHeightMultiplier;
    [SerializeField] private float boostImpulseMultiplier;
    [SerializeField] private float boostDuration;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision with: " + other.name + ", " + other.gameObject.layer);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collide");
            other.GetComponentInParent<CharacterController>().FlowerBoost(boostImpulseMultiplier, boostHeightMultiplier, boostDuration, this.gameObject);
        }
    }
}
