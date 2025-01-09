using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float destroyTime = 0.1f;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.speed = 1.0f;  // Set the animation speed to double the normal speed
        }
        else
        {
            Debug.LogError("Animator component not found!");
        }
    
    Destroy(gameObject, destroyTime);
    }

   // Update is called once per frame
    void Update()
    {
        
    }
}
