using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion1Destroy : MonoBehaviour
{
   
        
    void Start()
    {
        // Automatically explode after the lifetime
        Destroy(gameObject, 0.5f);
    }

    
}
