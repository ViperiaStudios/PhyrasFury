using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4 : MonoBehaviour
{
    private float _e2Speed = 4.0f;
    private int _health = 5;
    private SpriteRenderer _childSpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        Transform childSpriteTransform = transform.Find("E41/E42");
        if (childSpriteTransform != null)
        {
            _childSpriteRenderer = childSpriteTransform.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("Child sprite not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {

        transform.Translate(Vector3.left * _e2Speed * Time.deltaTime);


        if (transform.position.x <= -11.90f)
       {
            float randomY = Random.Range(5.5f, 6.5f);
          transform.position = new Vector3(17, randomY, 0);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player" || other.tag == "Laser"))
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
}
