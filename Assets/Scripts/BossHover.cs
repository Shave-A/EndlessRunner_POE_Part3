using UnityEngine;

public class BossHover : MonoBehaviour
{
    [Header("Hover Settings")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 8f, 25f);   // (X, Y, Z) relative to player

    [Header("Movement")]
    public float smoothSpeed = 3f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.5f;

    private Vector3 targetPosition;
    private float startY;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>()?.transform;

        startY = transform.position.y;
    }

    void Update()
    {
        if (player == null) return;

        // Calculate desired position in front and above player
        targetPosition = player.position + offset;

        // Add gentle bobbing motion
        targetPosition.y += Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Always look at the player
        transform.LookAt(player.position + Vector3.up * 2f);
    }

    // Call this when boss event ends
    public void RemoveBoss()
    {
        Destroy(gameObject, 2f);   // Optional dramatic exit
    }
}