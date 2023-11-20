using UnityEngine;
using CharacterController = _Project.Player_Character.Script.CharacterController;

public class SpikeBall : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Speed of rotation

    void Update()
    {
        // Constant rotation
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Since the SpikeBall can only collide with the Player, directly handle the death
        other.GetComponentInParent<CharacterController>().Die();
    }
}