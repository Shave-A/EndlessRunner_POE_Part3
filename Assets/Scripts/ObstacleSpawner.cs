using UnityEngine;

[System.Serializable]
public class ObstacleType
{
    public GameObject prefab;
    public float spawnHeight = 1f;

    [Header("Rotation")]
    public Vector3 spawnRotation;
}

public class ObstacleSpawner : MonoBehaviour
{
    [Header("City Obstacles")]
    public ObstacleType[] cityObstacleTypes;

    [Header("Forest Obstacles")]
    public ObstacleType[] forestObstacleTypes;

    [Header("References")]
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnDistance = 35f;
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 4f;

    [Header("Group Spawning")]
    public bool useGroups = true;

    [Range(1, 3)]
    public int maxObstaclesPerGroup = 2;

    private float nextSpawnTime = 0f;
    private bool isPaused = false;

    void Start()
    {
        if (cityObstacleTypes.Length == 0)
            Debug.LogError("❌ No city obstacle types assigned!");
        if (forestObstacleTypes.Length == 0)
            Debug.LogError("❌ No forest obstacle types assigned!");
        if (player == null)
            Debug.LogError("❌ Player reference missing!");

        nextSpawnTime = Time.time + 2f;
    }

    void Update()
    {
        if (player == null || isPaused) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnGroup();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnGroup()
    {
        // Pick obstacle set based on current biome
        bool inForest = (Game.instance.score / 25) % 2 == 1;
        ObstacleType[] currentObstacles = inForest ? forestObstacleTypes : cityObstacleTypes;

        if (currentObstacles.Length == 0) return;

        int obstaclesInGroup = useGroups
            ? Random.Range(1, maxObstaclesPerGroup + 1)
            : 1;

        int startLane = Random.Range(0, 3);
        float currentX = (startLane - 1) * 3f;

        for (int i = 0; i < obstaclesInGroup; i++)
        {
            ObstacleType chosen = currentObstacles[Random.Range(0, currentObstacles.Length)];

            Vector3 spawnPos = new Vector3(
                currentX,
                chosen.spawnHeight,
                player.position.z + spawnDistance
            );

            Quaternion rotation = Quaternion.Euler(chosen.spawnRotation);
            Instantiate(chosen.prefab, spawnPos, rotation);

            if (i < obstaclesInGroup - 1)
                currentX += Random.Range(1.5f, 3f);
        }
    }

    public void PauseSpawning()
    {
        isPaused = true;
        Debug.Log("⏸️ Normal Obstacles Paused (Boss Phase)");
    }

    public void ResumeSpawning()
    {
        isPaused = false;
        nextSpawnTime = Time.time + 1f;
        Debug.Log("▶️ Normal Obstacles Resumed");
    }
}