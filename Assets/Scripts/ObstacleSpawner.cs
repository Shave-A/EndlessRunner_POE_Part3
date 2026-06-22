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
            bool inForest = (Game.instance.score / 25) % 2 == 1;
            ObstacleType[] currentObstacles = inForest ? forestObstacleTypes : cityObstacleTypes;

            if (currentObstacles.Length == 0) return;

            int obstaclesInGroup = useGroups ? Random.Range(1, maxObstaclesPerGroup + 1) : 1;

            for (int i = 0; i < obstaclesInGroup; i++)
            {
                int chosenIndex = Random.Range(0, currentObstacles.Length);
                ObstacleType chosen = currentObstacles[chosenIndex];

                float xPos;

                // Index 1 in both biomes always spawns in middle
                if (chosenIndex == 1)
                {
                    xPos = 0f;
                }
                else
                {
                    int lane = Random.Range(0, 3);
                    xPos = (lane - 1) * 3f;
                }

                Vector3 spawnPos = new Vector3(xPos, chosen.spawnHeight, player.position.z + spawnDistance);
                Quaternion rotation = Quaternion.Euler(chosen.spawnRotation);
                Instantiate(chosen.prefab, spawnPos, rotation);
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