using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTransit : MonoBehaviour
{

    private Animator animator; 

    void Start()
    {
        animator = GetComponent<Animator>();

        
        if (animator == null)
        {
            Debug.LogError("Animator component not found! Make sure this GameObject has an Animator attached.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) 
        {
            animator.SetBool("isBreathing", true);
        }
        else
        {
            animator.SetBool("isBreathing", false);
        }
    }

}
