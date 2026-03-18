using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy6shieldScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PowerUp")
        {
            Destroy(other.gameObject);
            Debug.Log("PowerUp destroyed!");
        }

    }
}
