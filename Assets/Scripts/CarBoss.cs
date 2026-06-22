using System.Collections;
using UnityEngine;

public class CarBoss : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Spawn Settings")]
    public Vector3 spawnOffset = new Vector3(0, 10f, 35f);
    [Tooltip("Y = 180 usually faces the player")]
    public Vector3 spawnRotation = new Vector3(0, 180f, 0);

    [Header("Movement")]
    public Vector3 followOffset = new Vector3(0, 8f, 30f);
    public float smoothSpeed = 3f;

    [Header("Laser Settings")]
    public float warningDuration = 2f;
    public float shootInterval = 3f;
    public GameObject laserWarningPrefab;
    public GameObject laserFirePrefab;

    private GameObject activeLaser;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>()?.transform;

        // Force spawn position and rotation
        if (player != null)
            transform.position = player.position + spawnOffset;

        transform.rotation = Quaternion.Euler(spawnRotation);

        Debug.Log("Boss Spawned with Rotation: " + transform.eulerAngles);

        StartCoroutine(ShootRoutine());
    }

    void Update()
    {
        if (player == null) return;

        // Follow player
        Vector3 targetPos = player.position + followOffset;
        targetPos.x = 0f;

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        // FORCE rotation every frame
        transform.rotation = Quaternion.Euler(spawnRotation);
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);

            int targetLane = Random.Range(0, 3);
            float targetX = -3f + (targetLane * 3f);

            if (laserWarningPrefab != null)
                activeLaser = Instantiate(laserWarningPrefab, new Vector3(targetX, 1f, transform.position.z - 5f), Quaternion.identity);

            yield return new WaitForSeconds(warningDuration);
            if (activeLaser != null) Destroy(activeLaser);

            if (laserFirePrefab != null)
                activeLaser = Instantiate(laserFirePrefab, new Vector3(targetX, 1f, transform.position.z - 5f), Quaternion.identity);

            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null && Mathf.Abs(pc.transform.position.x - targetX) < 1.5f)
                pc.SendMessage("Die", SendMessageOptions.DontRequireReceiver);

            yield return new WaitForSeconds(0.3f);
            if (activeLaser != null) Destroy(activeLaser);
        }
    }

    public void RemoveCar()
    {
        StopAllCoroutines();
        if (activeLaser != null) Destroy(activeLaser);
        Destroy(gameObject, 1.5f);
    }
}