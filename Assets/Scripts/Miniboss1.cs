using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Miniboss1 : MonoBehaviour
{
    public float speed = 5f; // Speed of movement
    private Vector3[] targets = new Vector3[10]; // Array to store 10 random positions
    private int currentTargetIndex = 0; // Current target index

    public GameObject projectilePrefab;
    public GameObject phase2Particle;
    public GameObject explosionPrefab;
    public float fireRate = 2f;
    private float nextFireTime = 2f;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public Transform player;  // Player reference
    public float chargeSpeed = 15f; // Speed of the boss during the ram
    private float phaseTime = 0f; // Timer for the current phase
    private Vector3 originalPosition; // Position before the ram
    private Vector3 lockedPlayerPosition; // Player position when the boss locks on
    private float nextRamTime = 0f; // Timer to track when to attempt the next ram
    private float rammingPhaseTime = 0f; // Timer for how long the boss is ramming
    private bool returningToOriginal = false; // Flag to track when boss is returning after ramming
    private bool isWaitingToRam = false; // Flag to track if boss is waiting before ramming
    private bool isMovingBetweenRams = false; // Flag to control the boss moving between rams

    [SerializeField]
    public int health = 500; // Boss health value
    //public TMP_Text healthText; // Reference to the UI Text component on Canvas to display health


    private enum BossState { Moving, Charging, Ramming, Firing }
    private BossState currentState = BossState.Moving; // Starting state is moving

    void Start()
    {
        GenerateNewTargets(); // Generate the first 10 targets
        originalPosition = transform.position; // Set the initial position as the original

        spriteRenderer = GetComponent<SpriteRenderer>();
        phase2Particle.SetActive(false);

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the boss!");  // Ensure SpriteRenderer is assigned
        }

        animator = GetComponent<Animator>();

        // Check if the Animator component is found
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the boss!");
        }

        // Debug to confirm player reference at start
        if (player != null)
        {
            Debug.Log("Player Transform assigned correctly.");
        }
        else
        {
            Debug.LogError("Player Transform is not assigned. Please assign it in the Inspector.");
        }

        //UpdateHealthUI();
    }

    void Update()
    {
        switch (currentState)
        {
            case BossState.Moving:
                MoveToTarget();
                FireProjectile();
                HandlePhaseChange();
                break;
            case BossState.Charging:
                ChargeUp();
                break;
            case BossState.Ramming:
                HandleRammingPhase();
                break;
            case BossState.Firing:
                MoveToTarget();
                FireProjectile();
                HandlePhaseChange(); // Continue normal movement and firing after ramming phase
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Laser")  // Check if the collision is with the "Laser"
        {
            TakeDamage(1);  // Decrease health by 1 point
            Debug.Log("Boss hit by Laser!");

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
            }

            Destroy(other.gameObject);
        }

        if (other.tag == "Expl1")
        {
            TakeDamage(2);
            Debug.Log("hit by 2nd shot");

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
            }
        }

        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.TakeDamage(4);
                
            }
        }

    }

    void TakeDamage(int damage)
    {
        health -= damage;  // Reduce health
        //UpdateHealthUI();  // Update the UI to reflect the current health

        if (health <= 200)
        {
            phase2Particle.SetActive(true);
            speed = 10f;  // Increase speed
            fireRate = 1.5f;  // Increase firing rate
            chargeSpeed = 30f;  // Increase charge speed
            Debug.Log("Boss health below 200! Increasing speed, fire rate, and charge speed.");
        }

        if (health <= 0)
        {
            Die();  // Handle boss death when health is 0 or below
        }
    }

    //void UpdateHealthUI()
   // {
       // if (healthText != null)
       // {
         //   healthText.text = health.ToString();  // Update health value on UI
       // }
    //}

    void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);  // Destroy the boss GameObject
    }

    void GenerateNewTargets()
    {
        // Generate 10 random positions within the given boundaries
        for (int i = 0; i < targets.Length; i++)
        {
            float randomX = Random.Range(11.60f, 17.67f);
            float randomY = Random.Range(4.25f, 10.20f);
            targets[i] = new Vector3(randomX, randomY, 0f); // Z is always 0
        }

        currentTargetIndex = 0; // Reset the target index
    }

    void MoveToTarget()
    {
        if (currentTargetIndex < targets.Length)
        {
            // Move towards the current target position
            transform.position = Vector3.MoveTowards(transform.position, targets[currentTargetIndex], speed * Time.deltaTime);

            // Check if the object has reached the target position
            if (Vector3.Distance(transform.position, targets[currentTargetIndex]) < 0.1f)
            {
                currentTargetIndex++; // Move to the next target
            }
        }
        else
        {
            // Once all targets are reached, generate new targets
            GenerateNewTargets();
        }
    }

    void FireProjectile()
    {
        if (Time.time >= nextFireTime)
        {
            // Instantiate the projectile and set its position to the main object's position
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Set the next fire time
            nextFireTime = Time.time + fireRate;
        }
    }

    void HandlePhaseChange()
    {
        phaseTime += Time.deltaTime;

        if (phaseTime >= 15f) // After 15 seconds, switch to Charging phase
        {
            currentState = BossState.Charging;
            phaseTime = 0f; // Reset phase timer
            rammingPhaseTime = 0f; // Reset ramming timer
            Debug.Log("Switching to Charging phase");
        }
    }

    void ChargeUp()
    {
        // Save the original position before ramming
        originalPosition = transform.position;

        // Lock the player's position (record the target)
        lockedPlayerPosition = player.position;
        Debug.Log("Locked player position for ram: " + lockedPlayerPosition);

        if (animator != null)
        {
            animator.speed = 0f;  // Pause the animation
            Debug.Log("Animation paused during charge-up.");
        }

        if (health >= 200)
        {
            // Wait 1 second before ramming
            isWaitingToRam = true;
            Invoke("StartRamming", 1f); // Start ramming after 1 second
        }

        if (health <= 200)
        {
            isWaitingToRam = true;
            Invoke("StartRamming", 0.75f);
        }
    }

    void StartRamming()
    {
        isWaitingToRam = false;
        currentState = BossState.Ramming;

        if (animator != null)
        {
            animator.speed = 1f;  // Resume the animation
            Debug.Log("Animation resumed for ramming.");
        }

        Debug.Log("Ramming now...");
    }

    void HandleRammingPhase()
    {
        // Manage the ramming phase duration
        rammingPhaseTime += Time.deltaTime;

        if (rammingPhaseTime >= 20f)
        {
            // After 20 seconds of ramming, return to moving and firing state
            currentState = BossState.Firing;
            phaseTime = 0f; // Reset the timer for the next phase
            Debug.Log("Returning to Moving and Firing phase");
        }
        else if (!returningToOriginal && !isWaitingToRam)
        {
            if (isMovingBetweenRams)
            {
                // Boss moves around random targets for 3 seconds before next ram attempt
                MoveToTarget();
                if (Time.time >= nextRamTime)
                {
                    isMovingBetweenRams = false; // Stop random movement after 3 seconds
                    ChargeUp(); // Start new ram cycle
                }
            }
            else
            {
                RamTowardsPlayer(); // Charge toward the saved player position
            }
        }
        else if (returningToOriginal)
        {
            ReturnToOriginalPosition(); // After ramming, return to the original position
        }
    }

    void RamTowardsPlayer()
    {

        // Turn off the sprite when the boss starts ramming if health is 200 or below
        if (health <= 200 && spriteRenderer != null && spriteRenderer.enabled)
        {
            Debug.Log("Turning off sprite during ramming!");
            spriteRenderer.enabled = false;  // Turn off the sprite during ram
            
        }

        // Move towards the locked player position (saved before ramming)
        Debug.Log("Ramming towards saved player position: " + lockedPlayerPosition);

        transform.position = Vector3.MoveTowards(transform.position, lockedPlayerPosition, chargeSpeed * Time.deltaTime);

        // Check if the boss has reached the player's locked position
        if (Vector3.Distance(transform.position, lockedPlayerPosition) < 0.1f)
        {
            returningToOriginal = true; // Once the boss reaches the player, return to the original position
            Debug.Log("Reached saved player position, now returning to original position");
            Invoke("TurnSpriteBackOn", 0.2f);
        }

        
    }

    void TurnSpriteBackOn()
    {
        if (health <= 200 && spriteRenderer != null)
        {
            Debug.Log("Turning sprite back on after ramming!");
            spriteRenderer.enabled = true;

        }
    }
        void ReturnToOriginalPosition()
    {
        Debug.Log("Returning to original position");

        // Move back to the original position after ramming
        transform.position = Vector3.MoveTowards(transform.position, originalPosition, chargeSpeed * Time.deltaTime);

        // Check if the boss has returned to its original position
        if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
        {
            returningToOriginal = false; // Reset the flag to allow next ram
            isMovingBetweenRams = true; // Boss will move around for 3 seconds
            nextRamTime = Time.time + 3f; // Move randomly for 3 seconds before next ram
        }
    }
}
