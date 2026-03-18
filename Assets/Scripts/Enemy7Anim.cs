using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy7Anim : MonoBehaviour
{
    [SerializeField] private Animator animator; // ✅ Assign the Animator in the Inspector
    [SerializeField] private SpriteRenderer spriteRenderer; // ✅ Assign the SpriteRenderer in the Inspector
    [SerializeField] private int _eHealth = 12;

    private bool _isPlayerInside = false; // ✅ Tracks if the player is in the trigger
    private bool _canDamagePlayer = true; // ✅ Controls player damage cooldown
    private float _damageCooldown = 1.0f;

    private EvolutionManager evolutionManager;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>(); // ✅ Auto-assign if not set in the Inspector
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>(); // ✅ Auto-assign if not set
        }

        evolutionManager = FindObjectOfType<EvolutionManager>();
        if (evolutionManager == null)
        {
            Debug.LogError("EvolutionManager not found");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _canDamagePlayer) // ✅ Checks if the entering object is the player
        {
            animator.SetBool("isTriggered", true); // ✅ Play animation
            _isPlayerInside = true;

            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(2); // ✅ Deals damage on contact
                _canDamagePlayer = false;
                StartCoroutine(PlayerDamageCooldown());
            }
        }

        if (other.CompareTag("Laser"))
        {
            _eHealth--;

            StartCoroutine(FlashRed()); // ✅ Flash red when taking damage

            if (_eHealth <= 0)
            {
                evolutionManager.EnemyKilled(1);
                Destroy(gameObject);
            }

            Destroy(other.gameObject); // ✅ Destroy the laser
        }

        if (other.CompareTag("Expl1"))
        {
            _eHealth -= 2;
            StartCoroutine(FlashRed()); // ✅ Flash red when taking damage

            if (_eHealth <= 0)
            {
                evolutionManager.EnemyKilled(1);
                Destroy(gameObject);
            }
        }
        Debug.Log("Hit " + other.transform.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // ✅ Checks if the exiting object is the player
        {
            animator.SetBool("isTriggered", false); // ✅ Return to base animation
            _isPlayerInside = false;
        }
    }

    IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = originalColor;
    }

    IEnumerator PlayerDamageCooldown()
    {
        yield return new WaitForSeconds(_damageCooldown);
        _canDamagePlayer = true;
    }
    
}
