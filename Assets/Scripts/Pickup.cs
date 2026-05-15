using UnityEngine;

public enum PickupType
{
    SpeedBoost,
    Shield,
    SuperJump
}

public class Pickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public PickupType pickupType;
    public float value = 1f;           // Multiplier or strength (e.g. jump height)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CollectPickup(this);
            }

            Destroy(gameObject);   // Pickup disappears after collection
        }
    }
}