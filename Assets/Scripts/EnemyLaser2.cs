using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyLaser2 : MonoBehaviour
{
    [SerializeField]
    private float _eLSpeed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("enemy coroutine start");
        StartCoroutine(DestroyAfterDelay(5.0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            player.TakeDamage(2);
            Destroy(this.gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * _eLSpeed * Time.deltaTime);
        


    }

    
    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

