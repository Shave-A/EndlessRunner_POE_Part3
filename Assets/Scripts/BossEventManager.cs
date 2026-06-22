using UnityEngine;

public class BossEventManager : MonoBehaviour
{
    [Header("Boss Triggers")]
    public float firstBossScore = 300f;
    public float bossInterval = 500f;

    [Header("Boss Prefabs")]
    public GameObject boss1Prefab;
    public GameObject boss2Prefab;

    [Header("Boss Event Settings")]
    public float bossEventDuration = 18f;

    [Header("Special Obstacles")]
    public GameObject wideWallLeftPrefab;
    public GameObject wideWallRightPrefab;
    public GameObject floorTrapPrefab;

    private GameObject activeBossObject;
    private bool isBossActive = false;
    private float bossEndTime;
    private int currentBossIndex = 1;

    private ObstacleSpawner obstacleSpawner;

    void Start()
    {
        obstacleSpawner = FindAnyObjectByType<ObstacleSpawner>();
        Debug.Log("BossEventManager Started");
    }

    void Update()
    {
        if (Game.instance == null) return;

        // Trigger new boss
        if (!isBossActive)
        {
            float nextTrigger = firstBossScore + (bossInterval * (currentBossIndex - 1));
            if (Game.instance.score >= nextTrigger)
            {
                StartBossEvent();
            }
        }

        // Check if current boss should end
        if (isBossActive && Time.time >= bossEndTime)
        {
            Debug.Log("Boss timer reached - Ending boss event");
            EndBossEvent();
        }
    }

    public void StartBossEvent()
    {
        if (isBossActive) return;

        isBossActive = true;
        bossEndTime = Time.time + bossEventDuration;

        Debug.Log($"🚨 BOSS {currentBossIndex} STARTED! Will end in {bossEventDuration} seconds.");

        if (obstacleSpawner != null)
            obstacleSpawner.PauseSpawning();

        // Spawn boss
        GameObject prefabToUse = (currentBossIndex == 1) ? boss1Prefab : boss2Prefab;

        if (prefabToUse != null)
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                Vector3 spawnPos = player.transform.position + new Vector3(0, 10f, 35f);
                activeBossObject = Instantiate(prefabToUse, spawnPos, Quaternion.identity);

                CarBoss carBoss = activeBossObject.GetComponent<CarBoss>();
                if (carBoss != null)
                    carBoss.player = player.transform;
            }
        }

        InvokeRepeating("SpawnBossObstacle", 0.8f, 1.6f);
    }

    private void SpawnBossObstacle()
    {
        if (!isBossActive) return;

        int pattern = Random.Range(0, 3);
        PlayerController player = FindAnyObjectByType<PlayerController>();
        float playerZ = player != null ? player.transform.position.z : 0f;

        switch (pattern)
        {
            case 0: SpawnSingleWideWall(playerZ); break;
            case 1: SpawnFloorTrap(playerZ); break;
            default:
                if (Random.value > 0.5f) SpawnSingleWideWall(playerZ);
                else SpawnFloorTrap(playerZ);
                break;
        }
    }

    private void SpawnSingleWideWall(float playerZ)
    {
        if (Random.value > 0.5f && wideWallLeftPrefab)
            Instantiate(wideWallLeftPrefab, new Vector3(-4.5f, 1f, playerZ + 45f), Quaternion.identity);
        else if (wideWallRightPrefab)
            Instantiate(wideWallRightPrefab, new Vector3(4.5f, 1f, playerZ + 45f), Quaternion.identity);
    }

    private void SpawnFloorTrap(float playerZ)
    {
        if (floorTrapPrefab)
            Instantiate(floorTrapPrefab, new Vector3(0f, 0.5f, playerZ + 40f), Quaternion.identity);
    }

    private void EndBossEvent()
    {
        Debug.Log("EndBossEvent() called - Cleaning up");

        isBossActive = false;
        CancelInvoke("SpawnBossObstacle");

        if (obstacleSpawner != null)
            obstacleSpawner.ResumeSpawning();

        if (activeBossObject != null)
        {
            CarBoss carBoss = activeBossObject.GetComponent<CarBoss>();
            if (carBoss != null)
                carBoss.RemoveCar();
            else
                Destroy(activeBossObject, 1f);

            activeBossObject = null;
        }

        Debug.Log($"✅ Boss {currentBossIndex} has ended.");
        currentBossIndex++;
    }
}