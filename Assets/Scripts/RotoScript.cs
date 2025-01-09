using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotoScript : MonoBehaviour
{
    public float rotationSpeed = 50f; // Adjustable speed
    public bool reverseRotation = false; // Option to reverse rotation

    // Update is called once per frame
    void Update()
    {
        // Calculate rotation direction based on reverseRotation option
        int direction = reverseRotation ? -1 : 1;

        // Rotate the GameObject on its Y-axis
        transform.Rotate(Vector3.up * rotationSpeed * direction * Time.deltaTime);
    }
}