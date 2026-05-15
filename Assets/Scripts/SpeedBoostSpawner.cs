using UnityEngine;

public class SpeedBoostSpawner : MonoBehaviour
{
    public GameObject speedBoostPrefab;
    public Transform player;
    public float spawnDistance = 40f;
    public float minInterval = 8f;
    public float maxInterval = 15f;

    private float nextSpawnTime = 0f;

    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnSpeedBoost();
            nextSpawnTime = Time.time + Random.Range(minInterval, maxInterval);
        }
    }

    void SpawnSpeedBoost()
    {
        if (speedBoostPrefab == null || player == null) return;

        int lane = Random.Range(0, 3);
        float xPos = (lane - 1) * 3f;

        Vector3 spawnPos = new Vector3(xPos, 1.5f, player.position.z + spawnDistance);

        Instantiate(speedBoostPrefab, spawnPos, Quaternion.identity);
    }
}