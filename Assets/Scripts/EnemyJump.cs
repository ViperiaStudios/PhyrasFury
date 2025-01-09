using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJump : MonoBehaviour
{
    public float jumpHeight = 1.0f; // Adjust this value to control the height of the jump
    public float jumpDuration = 0.5f; // Adjust this value to control the duration of each jump
    public float moveSpeed = 2.0f; // Adjust this value to control the speed of horizontal movement

    private bool isJumping = false;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
        // Start the coroutine to perform the jumping behavior
        StartCoroutine(JumpAndMoveRoutine());
    }

    private System.Collections.IEnumerator JumpAndMoveRoutine()
    {
        while (true) // Loop indefinitely
        {
            // Move the object to the left
            while (!isJumping && transform.position.x > initialPosition.x - 5f) // Adjust the distance threshold as needed
            {
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Jump up
            isJumping = true;
            float t = 0;
            Vector3 startPos = transform.position;
            Vector3 jumpEndPos = startPos + Vector3.up * jumpHeight;

            while (t < 1)
            {
                t += Time.deltaTime / jumpDuration;
                transform.position = Vector3.Lerp(startPos, jumpEndPos, t);
                yield return null;
            }

            // Move the object back down
            t = 0;
            startPos = transform.position;

            while (t < 1)
            {
                t += Time.deltaTime / jumpDuration;
                transform.position = Vector3.Lerp(jumpEndPos, startPos, t);
                yield return null;
            }

            isJumping = false;
        }
    }
}
