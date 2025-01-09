using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goblinAnimate : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
   // private bool goblinRun;
   
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        //goblinRun = false;

    }

    void Update()
    {

        goblinMovement();

        goblinDirection();
           
    }

    void goblinMovement()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool("goblinRun", true);
        }

        else

            animator.SetBool("goblinRun", false);
    }

    void goblinDirection()
    {
        float goblinInput = Input.GetAxis("Horizontal");
       
        if(goblinInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        else if (goblinInput > 0)
        {
            spriteRenderer.flipX = false;
        }
       
    }
}
