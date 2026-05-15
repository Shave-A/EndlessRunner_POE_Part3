using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float baseForwardSpeed = 10f;
    public float laneDistance = 3f;
    public float smoothLaneSpeed = 15f;

    [Header("Controls")]
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode slideKey = KeyCode.S;

    [Header("Jump Settings")]
    public float jumpForce = 14f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Sliding")]
    public float slideDuration = 0.8f;
    public float slideHeight = 0.6f;

    [Header("Speed Boost")]
    public float boostMultiplier = 1.8f;
    public float boostDuration = 4f;

    [Header("Shield")]
    public float shieldDuration = 5f;

    [Header("Super Jump")]
    public float superJumpMultiplier = 1.8f;

    private Rigidbody rb;
    private int currentLane = 1;
    private Vector3 targetPosition;
    private bool isGrounded = true;
    private bool isDead = false;

    // Speed Boost
    private float currentSpeed;
    private bool isBoosted = false;
    private float boostTimer = 0f;

    // Shield
    private bool hasShield = false;
    private float shieldTimer = 0f;

    // Super Jump
    private bool isSuperJumpReady = false;

    // Sliding
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float originalHeight;
    private Vector3 originalCenter;
    private CapsuleCollider capsuleCollider;

    //Gane over screen
    private Canvas gameOverCanvas;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        if (capsuleCollider != null)
        {
            originalHeight = capsuleCollider.height;
            originalCenter = capsuleCollider.center;
        }

        // Find Game Over Canvas
        GameObject canvasObj = GameObject.Find("GameOverCanvas");
        if (canvasObj != null)
        {
            gameOverCanvas = canvasObj.GetComponent<Canvas>();
            gameOverCanvas.enabled = false;   // Hide at start
        }

        currentSpeed = baseForwardSpeed;
        targetPosition = transform.position;
    }

    // Replace your current jump code in Update() with this:
    void Update()
    {
        if (isDead) return;

        HandleTimers();

        // Auto run
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Lane switching (keep as is)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            currentLane = Mathf.Clamp(currentLane - 1, 0, 2);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            currentLane = Mathf.Clamp(currentLane + 1, 0, 2);

        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothLaneSpeed * Time.deltaTime);

        // ==================== IMPROVED JUMP ====================
        if ((Input.GetKeyDown(jumpKey) || Input.GetKeyDown(KeyCode.Space)) && isGrounded && !isSliding)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            isSuperJumpReady = false;
        }

        // Better jump physics (makes falling snappier)
        if (rb.linearVelocity.y < 0) // Falling
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !(Input.GetKey(jumpKey) || Input.GetKey(KeyCode.Space))) // Short jump
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Slide logic (keep as is)
        if (Input.GetKeyDown(slideKey) && isGrounded && !isSliding)
            StartSlide();

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
                EndSlide();
        }
    }

    private void HandleTimers()
    {
        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f) EndBoost();
        }

        if (hasShield)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f) hasShield = false;
        }
    }

    // ====================== PICKUPS ======================
    public void CollectPickup(Pickup pickup)
    {
        switch (pickup.pickupType)
        {
            case PickupType.SpeedBoost: ActivateSpeedBoost(); break;
            case PickupType.Shield: ActivateShield(); break;
            case PickupType.SuperJump: ActivateSuperJump(); break;
        }
    }

    private void ActivateSpeedBoost()
    { /* same as before */
        isBoosted = true; boostTimer = boostDuration; currentSpeed = baseForwardSpeed * boostMultiplier;
    }
    private void EndBoost() { isBoosted = false; currentSpeed = baseForwardSpeed; }

    private void ActivateShield() { hasShield = true; shieldTimer = shieldDuration; }
    private void ActivateSuperJump() { isSuperJumpReady = true; }

    // ====================== SLIDING ======================
    private void StartSlide()
    {
        if (capsuleCollider == null) return;
        isSliding = true;
        slideTimer = slideDuration;
        capsuleCollider.height = slideHeight;
        capsuleCollider.center = new Vector3(0, slideHeight / 2f - 0.1f, 0);
    }

    private void EndSlide()
    {
        if (capsuleCollider == null) return;
        isSliding = false;
        capsuleCollider.height = originalHeight;
        capsuleCollider.center = originalCenter;
    }

    // ====================== COLLISIONS ======================
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (hasShield)
            {
                hasShield = false;
                Debug.Log("Shield blocked an obstacle!");
                return;
            }
            else if (!isDead)
                Die();
        }
        else if (other.TryGetComponent<Pickup>(out Pickup pickup))
        {
            CollectPickup(pickup);
            Destroy(other.gameObject);
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("💀 Player Died - Game Over");
        Time.timeScale = 0f;

        // Show Game Over Screen
        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = true;
        }

        // Ragdoll effect
        rb.constraints = RigidbodyConstraints.None;
        rb.AddTorque(new Vector3(5, 0, 10), ForceMode.Impulse);
    }
}