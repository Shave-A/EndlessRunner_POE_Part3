using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float baseForwardSpeed = 10f;
    public float laneDistance = 3f;
    public float smoothLaneSpeed = 15f;
    private float speedIncreaseTimer = 0f;

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

    private BoxCollider boxCollider;
    private Vector3 originalSize;
    private Vector3 originalCenter;

    private Canvas gameOverCanvas;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            originalSize = boxCollider.size;
            originalCenter = boxCollider.center;
        }

        
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;

        GameObject canvasObj = GameObject.Find("GameOverCanvas");

        if (canvasObj != null)
        {
            gameOverCanvas = canvasObj.GetComponent<Canvas>();
            gameOverCanvas.enabled = false;
        }

        currentSpeed = baseForwardSpeed;
    }

    void Update()
    {
        if (isDead) return;

        HandleTimers();

        speedIncreaseTimer += Time.deltaTime;
        if (speedIncreaseTimer >= 2f)
        {
            speedIncreaseTimer = 0f;
            if (!isBoosted)
                baseForwardSpeed += 0.1f;
            currentSpeed = isBoosted ? baseForwardSpeed * boostMultiplier : baseForwardSpeed;
        } 

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            currentLane = Mathf.Clamp(currentLane - 1, 0, 2);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            currentLane = Mathf.Clamp(currentLane + 1, 0, 2);

        
        if ((Input.GetKeyDown(jumpKey) || Input.GetKeyDown(KeyCode.Space))
            && isGrounded
            && !isSliding)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); 
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            isSuperJumpReady = false;
        }

        
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity +=
                Vector3.up * Physics.gravity.y *
                (fallMultiplier - 1) *
                Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 &&
                !(Input.GetKey(jumpKey) ||
                Input.GetKey(KeyCode.Space)))
        {
            rb.linearVelocity +=
                Vector3.up * Physics.gravity.y *
                (lowJumpMultiplier - 1) *
                Time.deltaTime;
        }

        
        if (Input.GetKeyDown(slideKey)
            && isGrounded
            && !isSliding)
        {
            StartSlide();
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            if (slideTimer <= 0)
                EndSlide();
        }


    }

    void FixedUpdate()
    {
        if (isDead) return;

      
        float targetX = (currentLane - 1) * laneDistance;

        float xVel = (targetX - transform.position.x) * smoothLaneSpeed;
        float zVel = currentSpeed;
        float yVel = rb.linearVelocity.y; 

        rb.linearVelocity = new Vector3(xVel, yVel, zVel);
    }

    private void HandleTimers()
    {
        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;

            if (boostTimer <= 0)
                EndBoost();
        }

        if (hasShield)
        {
            shieldTimer -= Time.deltaTime;

            if (shieldTimer <= 0)
                hasShield = false;
        }
    }

    public void CollectPickup(Pickup pickup)
    {
        switch (pickup.pickupType)
        {
            case PickupType.SpeedBoost:
                ActivateSpeedBoost();
                break;

            case PickupType.Shield:
                ActivateShield();
                break;

            case PickupType.SuperJump:
                ActivateSuperJump();
                break;
        }
    }

    private void ActivateSpeedBoost()
    {
        isBoosted = true;
        boostTimer = boostDuration;
        currentSpeed = baseForwardSpeed * boostMultiplier;
    }

    private void EndBoost()
    {
        isBoosted = false;
        currentSpeed = baseForwardSpeed;
    }

    private void ActivateShield()
    {
        hasShield = true;
        shieldTimer = shieldDuration;
    }

    private void ActivateSuperJump()
    {
        isSuperJumpReady = true;
    }

    private void StartSlide()
    {
        if (boxCollider == null) return;

        isSliding = true;
        slideTimer = slideDuration;

        Vector3 newSize = originalSize;
        newSize.y = slideHeight;

        boxCollider.size = newSize;

        boxCollider.center =
            new Vector3(
                originalCenter.x,
                slideHeight / 2f,
                originalCenter.z
            );
    }

    private void EndSlide()
    {
        if (boxCollider == null) return;

        isSliding = false;

        boxCollider.size = originalSize;
        boxCollider.center = originalCenter;
    }

    void OnCollisionEnter(Collision collision)
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
                return;
            }

            if (!isDead)
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
        Time.timeScale = 1f;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddTorque(
            new Vector3(5, 0, 10),
            ForceMode.Impulse
        );
        SceneManager.LoadScene("Death_Screen");
    }
}