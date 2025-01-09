using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossOn : MonoBehaviour
{
    [SerializeField]
    private GameObject boss;  // Reference to the GameObject to be activated

    public float delayTime = 60.0f;  // Time delay in seconds (1 minute by default)

    void Start()
    {
        boss.SetActive(false);
        // Start the coroutine to activate the GameObject after the delay
        StartCoroutine(ActivateObjectAfterDelay());
    }

    IEnumerator ActivateObjectAfterDelay()
    {
        // Wait for the specified amount of time (delayTime)
        yield return new WaitForSeconds(delayTime);

        // Activate the GameObject
        boss.SetActive(true);

        Debug.Log("GameObject activated after " + delayTime + " seconds.");
    }
}
