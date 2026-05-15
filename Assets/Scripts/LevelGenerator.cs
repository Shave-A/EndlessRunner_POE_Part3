using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Road Settings")]
    public GameObject[] tilePrefabs;
    public int poolSize = 12;
    public float tileLength = 20f;
    public Transform player;

    private List<GameObject> tiles = new List<GameObject>();
    private float spawnZ = 0f;
    private int nextTileIndex = 0;

    void Start()
    {
        if (tilePrefabs.Length == 0 || player == null) return;

        // Create pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tile = Instantiate(tilePrefabs[0]);
            tile.SetActive(false);
            tiles.Add(tile);
        }

        spawnZ = -tileLength;
        for (int i = 0; i < 8; i++)
            SpawnTile();
    }

    void Update()
    {
        if (player == null) return;

        // Spawn new tiles
        if (player.position.z > spawnZ - (tileLength * 3f))
            SpawnTile();

        // Despawn old tiles
        DespawnOldTiles();
    }

    void SpawnTile()
    {
        GameObject tile = tiles[nextTileIndex];
        tile.SetActive(true);
        tile.transform.position = new Vector3(0, 0, spawnZ);
        spawnZ += tileLength;
        nextTileIndex = (nextTileIndex + 1) % poolSize;
    }

    void DespawnOldTiles()
    {
        foreach (GameObject tile in tiles)
        {
            if (tile.activeInHierarchy && tile.transform.position.z + tileLength * 2 < player.position.z)
            {
                tile.SetActive(false);
            }
        }
    }
}