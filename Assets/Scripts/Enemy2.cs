using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    private float _e2Speed = 4.0f;
    private int _health = 2;
    private SpriteRenderer _childSpriteRenderer;

    private bool _canDamagePlayer = true; // Flag to control player damage cooldown
    private float _damageCooldown = 1.0f; // Cooldown duration in seconds
    





    //public float bounceHeight = 1.50f; // Set the height of the bounce
    // public float bounceSpeed = 2.0f; // Set the speed of the bounce


    //private string laserTag = "Laser";

    void Start()
    {

        

        Transform childSpriteTransform = transform.Find("child");
        if (childSpriteTransform != null)
        {
            _childSpriteRenderer = childSpriteTransform.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("Child sprite not found!");
        }
    }


    void Update()
    {
        Movement();
    }

    void Movement()
    {

        transform.Translate(Vector3.left * _e2Speed * Time.deltaTime);


        if (transform.position.x <= -11.90f)
        {
            float randomY = Random.Range(-0.50f, 6f);
            transform.position = new Vector3(12, randomY, 0);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player" || other.tag == "Laser") && _canDamagePlayer)
        {
            _health--;

            // Flash red effect for the child sprite
            StartCoroutine(FlashRed(_childSpriteRenderer));

            if (_health <= 0)
            {
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
        _canDamagePlayer = false;
        yield return new WaitForSeconds(_damageCooldown);
        _canDamagePlayer = true;
    }

  
}

