using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvolutionManager : MonoBehaviour
{
    private int playerKills = 0;
    private bool isOnFire = false;
    private float lastDamageTime;
    private int killsThreshold = 50; // Number of kills required to become "on fire"
    private float damageCooldown = 10f; // Cooldown period (in seconds) during which the player cannot take damage to become "on fire"
    [SerializeField]
    private TextMeshProUGUI _enemiesText;
    private Player player;
   // public Text pointsText;
    [SerializeField]
    private GameObject heatingUp1;
    [SerializeField]
    private GameObject heatingUp2;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        heatingUp1.SetActive(false);
        heatingUp2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemyKilled(int enemyPoints)
    {
        playerKills += enemyPoints;
        

        if (playerKills >= 25.0f)
        {
            heatingUp1.SetActive(true);
        }

        if (playerKills >= 50.0f)
        {
            heatingUp1.SetActive(false);
            heatingUp2.SetActive(true);
        }

        if (!isOnFire && playerKills >= killsThreshold && Time.time - lastDamageTime >= damageCooldown)
        {
            ActivateOnFire();
        }

        UpdateEnemiesText();
    }

    // Call this method whenever the player takes damage
    public void PlayerDamaged()
    {
        lastDamageTime = Time.time;
        playerKills = 0;
        _enemiesText.text = "0";
        isOnFire = false;
        if (heatingUp1 && heatingUp2 != null)
        {
            heatingUp1.SetActive(false);
            heatingUp2.SetActive(false);
            player.EvolutionOneOff();
            //pointsText.text = "0";
        }
    }

    void UpdateEnemiesText()
    {
        if (_enemiesText != null)
        {

            _enemiesText.text = playerKills.ToString();
        }
    }


        private void ActivateOnFire()
    {
        isOnFire = true;
        player.EvolutionOneActive();
        // Perform actions to indicate that the player is "on fire"
        // For example: change player's appearance, grant special abilities, etc.
    }
}
