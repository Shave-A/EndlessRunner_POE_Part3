using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    public GameObject speedBoostPrefab;
    public GameObject shieldPrefab;
    public GameObject superJumpPrefab;

    [Header("References")]
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnDistance = 35f;        // Reduced for testing
    public float minSpawnInterval = 4f;
    public float maxSpawnInterval = 9f;

    [Header("Spawn Chances")]
    [Range(0, 100)] public float speedBoostChance = 45f;
    [Range(0, 100)] public float shieldChance = 30f;
    [Range(0, 100)] public float superJumpChance = 25f;

    private float nextSpawnTime = 0f;

    void Start()
    {
        if (player == null)
            Debug.LogError("PickupSpawner: Player reference is missing!");

        nextSpawnTime = Time.time + 2f;   // First pickup soon
    }

    void Update()
    {
        if (player == null) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnPickup();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnPickup()
    {
        int lane = Random.Range(0, 3);
        float xPos = (lane - 1) * 3f;

        float spawnZ = player.position.z + spawnDistance;

        Debug.Log($"Spawning pickup → Lane: {lane} | Z: {spawnZ:F1} | Player Z: {player.position.z:F1}");

        Vector3 spawnPos = new Vector3(xPos, 1.8f, spawnZ);

        float roll = Random.Range(0f, 100f);
        GameObject chosen = null;

        if (roll < speedBoostChance)
            chosen = speedBoostPrefab;
        else if (roll < speedBoostChance + shieldChance)
            chosen = shieldPrefab;
        else
            chosen = superJumpPrefab;

        if (chosen != null)
        {
            Instantiate(chosen, spawnPos, Quaternion.identity);
        }
    }
}