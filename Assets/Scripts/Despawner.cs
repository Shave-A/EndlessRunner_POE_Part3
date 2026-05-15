using UnityEngine;

public class Despawner : MonoBehaviour
{
    [Header("Despawn Settings")]
    public float despawnDistanceBehind = 15f;   // How far behind player before despawn

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogWarning("Despawner: Could not find Player!");
    }

    void Update()
    {
        if (player == null) return;

        // Despawn if behind the player
        if (transform.position.z + despawnDistanceBehind < player.position.z)
        {
            Destroy(gameObject);
        }
    }
}