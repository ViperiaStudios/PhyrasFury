using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _lSpeed = 8.0f;

    void Start()
    {
        StartCoroutine(DestroyAfterDelay(2.0f));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Translate(Vector3.right * _lSpeed * Time.deltaTime);
    }
}