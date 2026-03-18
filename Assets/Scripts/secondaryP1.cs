using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondaryP1 : MonoBehaviour
{
    [SerializeField]
   private GameObject explosionEffect; // Prefab for the explosion effect

    [SerializeField]
    private float lifetime = 0.3f; // Time before the projectile explodes

    void Start()
    {
        // Automatically explode after the lifetime
        Invoke(nameof(Explode), lifetime);
    }

    void Explode()
    {
        // Instantiate explosion effect at the projectile's position
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Destroy the projectile itself
        Destroy(gameObject);
    }
}