using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType
{
    public GameObject prefab;
    public Vector3 spawnRotation = Vector3.zero;   // ← Add rotation here
}

public class LevelGenerator : MonoBehaviour
{
    [Header("Road Settings")]
    public TileType cityTile;
    public TileType forestTile;

    public int poolSize = 15;
    public float tileLength = 20f;
    public Transform player;

    private List<GameObject> cityTiles = new List<GameObject>();
    private List<GameObject> forestTiles = new List<GameObject>();
    private float spawnZ = 0f;
    private int nextTileIndex = 0;

    void Start()
    {
        if (cityTile.prefab == null || forestTile.prefab == null || player == null)
        {
            Debug.LogError("LevelGenerator: Missing tile prefabs or Player!");
            return;
        }

        // Create City Tile Pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject city = Instantiate(cityTile.prefab);
            city.SetActive(false);
            cityTiles.Add(city);
        }

        // Create Forest Tile Pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject forest = Instantiate(forestTile.prefab);
            forest.SetActive(false);
            forestTiles.Add(forest);
        }

        spawnZ = -tileLength * 3f;
        for (int i = 0; i < 12; i++)
            SpawnTile();
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.z > spawnZ - (tileLength * 3f))
        {
            SpawnTile();
        }

        DespawnOldTiles();
    }

    void SpawnTile()
    {
        int score = Game.instance != null ? Game.instance.score : 0;
        bool useForest = (score / 5) % 2 == 1;

        List<GameObject> pool = useForest ? forestTiles : cityTiles;
        TileType currentTileType = useForest ? forestTile : cityTile;

        GameObject tile = pool[nextTileIndex];
        tile.SetActive(true);
        tile.transform.position = new Vector3(0, 0, spawnZ);
        tile.transform.rotation = Quaternion.Euler(currentTileType.spawnRotation);

        spawnZ += tileLength;
        nextTileIndex = (nextTileIndex + 1) % pool.Count;
    }

    void DespawnOldTiles()
    {
        float despawnZ = player.position.z - tileLength * 3f;

        foreach (GameObject tile in cityTiles)
        {
            if (tile.activeInHierarchy && tile.transform.position.z < despawnZ)
                tile.SetActive(false);
        }

        foreach (GameObject tile in forestTiles)
        {
            if (tile.activeInHierarchy && tile.transform.position.z < despawnZ)
                tile.SetActive(false);
        }
    }
}