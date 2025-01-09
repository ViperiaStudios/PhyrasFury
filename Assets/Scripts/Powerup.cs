using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _pSpeed = 3.0f;

    [SerializeField]  // 0 = TripleShot 1 = Speed 2 = Shields 
    private int powerupID;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
        transform.Translate(Vector3.left * _pSpeed * Time.deltaTime);

        if (transform.position.x <= -11.90f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                switch(powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;

                    case 1:
                        player.SpeedBoostActive();
                        Debug.Log("speed powerup activated");
                        break;

                    case 2:
                        player.ShieldsActive();
                        Debug.Log("Shields activated");
                        break;
                    case 3:
                        player.DebuffOn();
                        Debug.Log("DEBUFF activated");
                        break;
                    case 4:
                        player.HealPlayer(3);
                        Debug.Log("Health increased");
                        break;

                       
                }
            }

            Destroy(this.gameObject);
        }
    }
}
