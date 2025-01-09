using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : MonoBehaviour
{
    public float _regularSpeed = 3.0f;
    public float _chargeSpeed = 8.0f;
    public float _chargeDistance = 2.0f;
    public float _chargeInterval = 2.0f;
    public float _pauseDuration = 0.2f;

    private float nextChargeTime;
    private SpriteRenderer childSpriteRenderer;

    private bool canDamagePlayer = true; // Flag to control player damage cooldown
    private float damageCooldown = 1.0f; // Cooldown duration in seconds
    [SerializeField]
    private int health = 10;
    private EvolutionManager evolutionManager;

    void Start()
    {
        evolutionManager = FindObjectOfType<EvolutionManager>();
        if (evolutionManager == null)
        {
            Debug.LogError("EvolutionManager not found");
        }

        nextChargeTime = Time.time + _chargeInterval;

        Transform childSpriteTransform = transform.Find("mounted knight black_0");
        if (childSpriteTransform != null)
        {
            childSpriteRenderer = childSpriteTransform.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("Child sprite not found!");
        }
    }

    void Update()
    {
        MoveLeft();

        if (Time.time >= nextChargeTime)
        {
            StartCoroutine(ChargeSequence());
            nextChargeTime = Time.time + _chargeInterval;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player" || other.tag == "Laser") && canDamagePlayer)
        {
            health--;

            // Flash red effect for the child sprite
            StartCoroutine(FlashRed(childSpriteRenderer));

            if (health <= 0)
            {
                evolutionManager.EnemyKilled(3);
                Destroy(gameObject);
            }

            if (other.tag == "Player")
            {
                Player player = other.transform.GetComponent<Player>();

                if (player != null)
                {
                    player.TakeDamage(3);
                    Debug.Log("K DAMAGED PLAYER");
                    StartCoroutine(PlayerDamageCooldown());
                }
            }

            if (other.tag == "Laser")
            {
                Destroy(other.gameObject);
            }
        }

        Debug.Log("Hit" + other.transform.name);
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
        canDamagePlayer = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamagePlayer = true;
    }

    void MoveLeft()
    {
        transform.Translate(Vector3.left * _regularSpeed * Time.deltaTime);

        if (transform.position.x <= -11.90f)
        {
            float randomY = Random.Range(3.50f, 6f);
            transform.position = new Vector3(12, randomY, 0);
        }
    }

    System.Collections.IEnumerator ChargeSequence()
    {
        yield return new WaitForSeconds(_pauseDuration);
        transform.Translate(Vector3.left * _chargeSpeed * _chargeDistance * Time.deltaTime);
        yield return new WaitForSeconds(_pauseDuration);
    }
}