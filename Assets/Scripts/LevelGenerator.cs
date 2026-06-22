using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Road Settings")]
    public GameObject[] tilePrefabs;
    public int poolSize = 20;
    public float tileLength = 20f;
    public Transform player;

    private List<GameObject> cityTiles = new List<GameObject>();
    private List<GameObject> forestTiles = new List<GameObject>();
    private float spawnZ = 0f;
    private int nextTileIndex = 0;

    void Start()
    {
        if (tilePrefabs.Length < 2 || player == null) return;

        // Pre-create separate pools for each biome
        for (int i = 0; i < poolSize; i++)
        {
            GameObject city = Instantiate(tilePrefabs[0]);
            city.SetActive(false);
            cityTiles.Add(city);

            GameObject forest = Instantiate(tilePrefabs[1]);
            forest.SetActive(false);
            forestTiles.Add(forest);
        }

        spawnZ = -tileLength;
        for (int i = 0; i < 8; i++)
            SpawnTile();
    }

    void Update()
    {
        foreach (GameObject tile in cityTiles)
        {
            if (tile != null && tile.activeInHierarchy && tile.transform.position.z + tileLength * 2 < player.position.z)
                tile.SetActive(false);
        }
        foreach (GameObject tile in forestTiles)
        {
            if (tile != null && tile.activeInHierarchy && tile.transform.position.z + tileLength * 2 < player.position.z)
                tile.SetActive(false);
        }
    }

    void SpawnTile()
    {
        int score = Game.instance != null ? Game.instance.score : 0;
        bool useForest = (score / 25) % 2 == 1;

        // Pick from the correct pool — no destroying ever
        List<GameObject> pool = useForest ? forestTiles : cityTiles;

        GameObject tile = pool[nextTileIndex];
        tile.SetActive(true);
        tile.transform.position = new Vector3(0, 0, spawnZ);
        spawnZ += tileLength;
        Debug.Log("nextTileIndex: " + nextTileIndex + " | pool.Count: " + pool.Count);
        nextTileIndex = (nextTileIndex + 1) % pool.Count;
    }

    void DespawnOldTiles()
    {
        foreach (GameObject tile in cityTiles)
        {
            if (tile.activeInHierarchy && tile.transform.position.z + tileLength * 2 < player.position.z)
                tile.SetActive(false);
        }

        foreach (GameObject tile in forestTiles)
        {
            if (tile.activeInHierarchy && tile.transform.position.z + tileLength * 2 < player.position.z)
                tile.SetActive(false);
        }
    }
}