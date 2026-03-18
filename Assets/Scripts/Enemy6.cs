using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy6 : MonoBehaviour
{
    [SerializeField]
    private float _eSpeed = 4.0f;
    [SerializeField]
    private int _eHealth = 7;

    private SpriteRenderer _childSpriteRenderer;

    private bool _canDamagePlayer = true; // Flag to control player damage cooldown
    private float _damageCooldown = 1.0f; // Cooldown duration in seconds
    private bool _shieldActive = false; // Flag to check if the shield is on

    [SerializeField]
    private GameObject _shieldObject; // The shield GameObject
    private EvolutionManager evolutionManager;

    void Start()
    {
        evolutionManager = FindObjectOfType<EvolutionManager>();
        if (evolutionManager == null)
        {
            Debug.LogError("EvolutionManager not found");
        }

        Transform childSpriteTransform = transform.Find("dragon1");
        if (childSpriteTransform != null)
        {
            _childSpriteRenderer = childSpriteTransform.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("Child sprite not found!");
        }

        if (_shieldObject != null)
        {
            _shieldObject.SetActive(false); // Ensure shield starts off
        }
        else
        {
            Debug.LogError("Shield object not assigned!");
        }

        StartCoroutine(ShieldRoutine()); // Start the shield behavior
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        transform.Translate(Vector3.left * _eSpeed * Time.deltaTime);

        if (transform.position.x <= -5.6f)
        {
            float randomY = Random.Range(6f, 10f);
            transform.position = new Vector3(21, randomY, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_shieldActive)
        {
            // If the shield is on, the enemy takes no damage
            Debug.Log("Shield absorbed the attack!");
            return;
        }

        if ((other.tag == "Player" || other.tag == "Laser") && _canDamagePlayer)
        {
            _eHealth--; // Reduce health by 1 when hit by Player or Laser

            // Flash red effect for the child sprite
            StartCoroutine(FlashRed(_childSpriteRenderer));

            if (_eHealth <= 0)
            {
                evolutionManager.EnemyKilled(1);
                Destroy(gameObject);
            }

            if (other.tag == "Player")
            {
                Player player = other.transform.GetComponent<Player>();

                if (player != null)
                {
                    player.TakeDamage(1);
                    StartCoroutine(PlayerDamageCooldown());
                }
            }

            if (other.tag == "Laser")
            {
                Destroy(other.gameObject);
            }
        }

        // **New Condition: If this collides with "Expl1", reduce _eHealth by 2**
        if (other.tag == "Expl1")
        {
            _eHealth -= 2; // Reduce health by 2

            // Flash red effect for the child sprite
            StartCoroutine(FlashRed(_childSpriteRenderer));

            if (_eHealth <= 0)
            {
                evolutionManager.EnemyKilled(1);
                Destroy(gameObject);
            }
        }

        


        Debug.Log("Hit " + other.transform.name);
    }

    IEnumerator ShieldRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f); // Wait 4 seconds before activating shield

            _shieldActive = true;
            _shieldObject.SetActive(true); // Turn shield ON
            Debug.Log("Shield Activated!");

            yield return new WaitForSeconds(1.5f); // Shield stays on for 1 second

            _shieldActive = false;
            _shieldObject.SetActive(false); // Turn shield OFF
            Debug.Log("Shield Deactivated!");
        }
    }

    IEnumerator FlashRed(SpriteRenderer spriteRenderer)
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f); // Adjust the duration of the flash

        spriteRenderer.color = originalColor;
    }

    IEnumerator PlayerDamageCooldown()
    {
        _canDamagePlayer = false;
        yield return new WaitForSeconds(_damageCooldown);
        _canDamagePlayer = true;
    }
}
