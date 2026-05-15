using UnityEngine;

public class BossEventManager : MonoBehaviour
{
    [Header("Boss Trigger")]
    public float triggerScore = 300f;
    public float bossEventDuration = 15f;

    [Header("Boss Object")]
    public GameObject bossPrefab;

    [Header("Boss Obstacle Settings")]
    public float bossObstacleSpawnInterval = 1.6f;     // ← Configurable spawn rate
    public float bossObstacleSpawnDelay = 0.8f;        // Initial delay

    [Header("Special Obstacles")]
    public GameObject wideWallLeftPrefab;
    public GameObject wideWallRightPrefab;
    public GameObject floorTrapPrefab;

    private BossHover activeBoss;
    private bool isBossActive = false;
    private float bossEndTime;

    private ObstacleSpawner obstacleSpawner;

    void Start()
    {
        obstacleSpawner = FindAnyObjectByType<ObstacleSpawner>();
    }

    void Update()
    {
        if (Game.instance == null) return;

        if (!isBossActive && Game.instance.score >= triggerScore)
        {
            StartBossEvent();
        }

        if (isBossActive && Time.time >= bossEndTime)
        {
            EndBossEvent();
        }
    }

    public void StartBossEvent()
    {
        if (isBossActive) return;

        isBossActive = true;
        bossEndTime = Time.time + bossEventDuration;

        Debug.Log("🚨 BOSS EVENT STARTED!");

        if (obstacleSpawner != null)
            obstacleSpawner.PauseSpawning();

        // Spawn Boss
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            Vector3 spawnPos = player.transform.position + new Vector3(0, 10f, 35f);
            GameObject bossObj = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
            activeBoss = bossObj.GetComponent<BossHover>();
            if (activeBoss != null)
                activeBoss.player = player.transform;
        }

        // Start spawning boss obstacles with configurable rate
        InvokeRepeating("SpawnBossObstacle", bossObstacleSpawnDelay, bossObstacleSpawnInterval);
    }

    private void SpawnBossObstacle()
    {
        if (!isBossActive) return;

        int pattern = Random.Range(0, 3);

        PlayerController player = FindAnyObjectByType<PlayerController>();
        float playerZ = player != null ? player.transform.position.z : 0f;

        switch (pattern)
        {
            case 0:
                SpawnSingleWideWall(playerZ);     // Now spawns only ONE wall
                break;
            case 1:
                SpawnFloorTrap(playerZ);
                break;
            default:
                if (Random.value > 0.5f)
                    SpawnSingleWideWall(playerZ);
                else
                    SpawnFloorTrap(playerZ);
                break;
        }
    }

    // NEW: Spawns only ONE wide wall (either left or right)
    private void SpawnSingleWideWall(float playerZ)
    {
        if (Random.value > 0.5f)
        {
            // Spawn Left Wall
            if (wideWallLeftPrefab)
                Instantiate(wideWallLeftPrefab, new Vector3(-2.5f, 1f, playerZ + 45f), Quaternion.identity);
        }
        else
        {
            // Spawn Right Wall
            if (wideWallRightPrefab)
                Instantiate(wideWallRightPrefab, new Vector3(2.5f, 1f, playerZ + 45f), Quaternion.identity);
        }
    }

    private void SpawnFloorTrap(float playerZ)
    {
        if (floorTrapPrefab)
            Instantiate(floorTrapPrefab, new Vector3(0f, 0.5f, playerZ + 40f), Quaternion.identity);
    }

    private void EndBossEvent()
    {
        isBossActive = false;
        CancelInvoke("SpawnBossObstacle");

        if (obstacleSpawner != null)
            obstacleSpawner.ResumeSpawning();

        if (activeBoss != null)
        {
            activeBoss.RemoveBoss();
            activeBoss = null;
        }

        Debug.Log("✅ Boss Event Ended");
        triggerScore += 500f;   // Next boss trigger
    }
}