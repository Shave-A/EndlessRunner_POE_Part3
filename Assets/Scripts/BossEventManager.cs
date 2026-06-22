using UnityEngine;

public class BossEventManager : MonoBehaviour
{
    [Header("Boss Trigger")]
    public float bossScoreInterval = 12f;
    public float bossEventDuration = 15f;

    [Header("Helicopter Boss")]
    public GameObject helicopterBossPrefab;

    [Header("Car Boss")]
    public GameObject carBossPrefab;

    [Header("Boss Obstacle Settings")]
    public float bossObstacleSpawnInterval = 1.6f;
    public float bossObstacleSpawnDelay = 0.8f;

    [Header("Special Obstacles")]
    public GameObject wideWallLeftPrefab;
    public GameObject wideWallRightPrefab;
    public GameObject floorTrapPrefab;

    private BossHover activeHelicopter;
    private CarBoss activeCar;
    private bool isBossActive = false;
    private float bossEndTime;
    private float nextBossScore;
    private ObstacleSpawner obstacleSpawner;

    void Start()
    {
        obstacleSpawner = FindAnyObjectByType<ObstacleSpawner>();
        nextBossScore = bossScoreInterval;
    }

    void Update()
    {
        if (Game.instance == null) return;

        if (!isBossActive && Game.instance.score >= nextBossScore)
            StartBossEvent();

        if (isBossActive && Time.time >= bossEndTime)
            EndBossEvent();
    }

    public void StartBossEvent()
    {
        if (isBossActive) return;
        isBossActive = true;
        bossEndTime = Time.time + bossEventDuration;

        if (obstacleSpawner != null)
            obstacleSpawner.PauseSpawning();

        PlayerController player = FindAnyObjectByType<PlayerController>();

        // Decide which boss based on biome
        bool inForest = (Game.instance.score / 25) % 2 == 1;

        if (inForest)
        {
            // Spawn Car Boss
            if (player != null && carBossPrefab != null)
            {
                Vector3 spawnPos = player.transform.position + new Vector3(0, 0f, 40f);
                GameObject carObj = Instantiate(carBossPrefab, spawnPos, Quaternion.identity);
                carObj.transform.SetParent(null);
                activeCar = carObj.GetComponent<CarBoss>();
                if (activeCar != null)
                    activeCar.player = player.transform;
            }
        }
        else
        {
            // Spawn Helicopter Boss
            if (player != null && helicopterBossPrefab != null)
            {
                Vector3 spawnPos = player.transform.position + new Vector3(0, 10f, 35f);
                GameObject bossObj = Instantiate(helicopterBossPrefab, spawnPos, Quaternion.identity);
                bossObj.transform.SetParent(null);
                activeHelicopter = bossObj.GetComponent<BossHover>();
                if (activeHelicopter != null)
                    activeHelicopter.player = player.transform;
            }

            InvokeRepeating("SpawnBossObstacle", bossObstacleSpawnDelay, bossObstacleSpawnInterval);
        }

        Debug.Log("🚨 BOSS EVENT STARTED!");
    }

    private void SpawnBossObstacle()
    {
        if (!isBossActive) return;

        PlayerController player = FindAnyObjectByType<PlayerController>();
        float playerZ = player != null ? player.transform.position.z : 0f;

        // Randomly pick ONE obstacle type each time
        int pattern = Random.Range(0, 2);

        if (pattern == 0)
            SpawnSingleWideWall(playerZ);
        else
            SpawnFloorTrap(playerZ);
    }

    private void SpawnSingleWideWall(float playerZ)
    {
        if (Random.value > 0.5f)
        {
            if (wideWallLeftPrefab)
                Instantiate(wideWallLeftPrefab, new Vector3(-2.5f, 1f, playerZ + 45f), Quaternion.identity);
        }
        else
        {
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

        if (activeHelicopter != null)
        {
            activeHelicopter.RemoveBoss();
            activeHelicopter = null;
        }

        if (activeCar != null)
        {
            activeCar.RemoveCar();
            activeCar = null;
        }

        Game.instance.bossesDefeated++;
   

        nextBossScore = Game.instance.score + bossScoreInterval;
        Debug.Log("✅ Boss Event Ended");
    }

    void OnDisable()
    {
        isBossActive = false;
        CancelInvoke("SpawnBossObstacle");

        if (activeHelicopter != null)
        {
            Destroy(activeHelicopter.gameObject);
            activeHelicopter = null;
        }

        if (activeCar != null)
        {
            Destroy(activeCar.gameObject);
            activeCar = null;
        }
    }
}