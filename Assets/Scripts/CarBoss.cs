using System.Collections;
using UnityEngine;

public class CarBoss : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Movement")]
    public float driveSpeed = 15f;
    public Vector3 offset = new Vector3(0, 0f, 30f);
    public float smoothSpeed = 3f;

    [Header("Laser Settings")]
    public float warningDuration = 2f;
    public float shootInterval = 3f;
    public GameObject laserWarningPrefab;   // red laser line
    public GameObject laserFirePrefab;      // green laser line

    private bool isShooting = false;
    private GameObject activeLaser;

    // Lane positions matching your laneDistance = 3f
    private float[] lanePositions = { -3f, 0f, 3f };

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>()?.transform;

        StartCoroutine(ShootRoutine());
    }

    void Update()
    {
        if (player == null) return;

        // Stay in front of player in the middle lane
        Vector3 targetPos = player.position + offset;
        targetPos.x = 0f; // always middle of road
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        // Always face back toward player
        transform.LookAt(new Vector3(transform.position.x, transform.position.y, player.position.z));
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);

            // Pick a random lane to shoot
            int targetLane = Random.Range(0, 3);
            float targetX = lanePositions[targetLane];

            // Show red warning laser
            if (laserWarningPrefab != null)
            {
                activeLaser = Instantiate(laserWarningPrefab,
                    new Vector3(targetX, 1f, transform.position.z),
                    Quaternion.identity);
            }

            yield return new WaitForSeconds(warningDuration);

            // Destroy red, show green laser (fire)
            if (activeLaser != null)
                Destroy(activeLaser);

            if (laserFirePrefab != null)
            {
                activeLaser = Instantiate(laserFirePrefab,
                    new Vector3(targetX, 1f, transform.position.z),
                    Quaternion.identity);
            }

            // Check if player is in that lane
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                float playerX = playerController.transform.position.x;
                if (Mathf.Abs(playerX - targetX) < 1.5f)
                {
                    // Player is in the lane — kill them
                    playerController.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
                }
            }

            // Remove green laser after short delay
            yield return new WaitForSeconds(0.3f);
            if (activeLaser != null)
                Destroy(activeLaser);
        }
    }

    public void RemoveCar()
    {
        StopAllCoroutines();
        if (activeLaser != null)
            Destroy(activeLaser);
        Destroy(gameObject, 1f);
    }
}