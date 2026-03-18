using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    //private float _speedBoostMultiplier = 2;
    private float _speedBoost = 8.0f;

    [SerializeField]
    private GameObject _shieldPrefab;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleshotPrefab;

    [SerializeField]
    private Slider overheatSlider;  // Reference to the UI Slider for the overheat mechanic
    [SerializeField]
    private Image fillImage;  // Reference to the Fill Area's Image component

    [SerializeField]
    private float overheatDuration = 12f;  // How long the player can shoot before overheating
    [SerializeField]
    private float cooldownDuration = 3f;  // How long the player must wait after overheating

    private float overheatTime = 0f;  // Tracks how long the player has been holding the fire key
    private bool isOverheated = false;  // Whether the player is currently overheated

    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;
    private int _health = 10;
   
    private NewSpawnManager _spawnManager;

    [SerializeField]
    private bool _tripleShotON = false;
    [SerializeField]
    private bool _speedBoostON = false;

    [SerializeField]
    private bool _shieldBoostON = false;
   

    [SerializeField]
    private TextMeshProUGUI _livesText;
   
    private statsbar statsBar;

    public GameObject healthbarVisual;

    [SerializeField]
    private GameObject _onFireVisualPrefab;

    private EvolutionManager evolutionmanager;

    [SerializeField]
    private GameObject prefabA;  
    //[SerializeField]
    //private GameObject prefabB;  // Prefab for medium hold
    //[SerializeField]
    //private GameObject prefabC;  // Prefab for long hold

    [SerializeField]
    private float secondaryFireRate = 2.0f;  // Cooldown between secondary shots
    private float nextSecondaryFireTime = 0f;  // Track the next time the player can fire the secondary shot

   // private float holdTime = 0f;  // Track how long the R key is held down
    //private bool isHoldingRKey = false;  // Flag to track if the player is holding the R key


    private Coroutine _tripleShotCoroutine;
    private Coroutine _speedBoostCoroutine;
    private Coroutine _shieldBoostCoroutine;
    private Coroutine _debuffCoroutine; 


    void Start()
    {
        _shieldPrefab.SetActive(false);
        _onFireVisualPrefab.SetActive(false);
        statsBar = FindObjectOfType<statsbar>();
        evolutionmanager = FindObjectOfType<EvolutionManager>();
        if (evolutionmanager == null)
        {
            Debug.LogError("EvolutionManager not found");
        }

        UpdateLivesText();
       
        transform.position = new Vector3(2.25f, 9.45f, 0);

        _spawnManager = GameObject.Find("spawnManager").GetComponent<NewSpawnManager>();
   

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NOT assigned in the Inspector!");
        }


        // Initialize the overheat slider
        overheatSlider.maxValue = overheatDuration;
        overheatSlider.value = 0;

        if (fillImage == null)
        {
            fillImage = overheatSlider.fillRect.GetComponent<Image>();  // Get the Fill Image component from the Slider
        }
    }

   
    void Update()
    {

        CalulateMovement();

        HandleOverheat();

        HandleSecondaryShot();


        if (Input.GetKey(KeyCode.Mouse0) && !isOverheated && Time.time > _canFire)
        {
            FireProjectile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
    }

    private void HandleOverheat()
    {
        if (Input.GetKey(KeyCode.Mouse0)) // Holding the fire key
        {
            overheatTime += Time.deltaTime; // Increase the overheat time

            // Update the slider value
            overheatSlider.value = overheatTime;

            // Update the color of the slider based on how full it is
            UpdateSliderColor(overheatSlider.value / overheatSlider.maxValue);

            // If the player has been holding the fire button for too long, overheat
            if (overheatTime >= overheatDuration)
            {
                isOverheated = true;
                StartCoroutine(CooldownRoutine());
            }
        }
        else if (overheatTime > 0 && !isOverheated) // If the player releases the fire key and is not overheated
        {
            overheatTime -= Time.deltaTime * 3; // Decrease the overheat time more quickly
            overheatSlider.value = overheatTime;
            UpdateSliderColor(overheatSlider.value / overheatSlider.maxValue);
        }
    }

    private IEnumerator CooldownRoutine()
    {
        // Stop the player from firing and reset the overheat time
        Debug.Log("Overheated! Cooling down...");
        overheatSlider.value = overheatDuration;  // Set slider to full
        UpdateSliderColor(1f); // Set the color to the max level

        float elapsedTime = 0f;
        float startValue = overheatSlider.value;

        // Gradually decrease the slider value over the cooldown duration (3 seconds)
        while (elapsedTime < cooldownDuration)
        {
            elapsedTime += Time.deltaTime;
            overheatSlider.value = Mathf.Lerp(startValue, 0, elapsedTime / cooldownDuration);
            UpdateSliderColor(overheatSlider.value / overheatSlider.maxValue);
            yield return null;
        }

        Debug.Log("Cooldown finished, you can fire again.");
        overheatTime = 0f;  // Reset the overheat time after cooldown
        overheatSlider.value = 0;  // Ensure the slider is reset to 0
        UpdateSliderColor(0f);  // Reset the color
        isOverheated = false;  // Allow the player to fire again
    }

    // Update the color of the slider fill as it fills up
    private void UpdateSliderColor(float fillAmount)
    {
        if (fillAmount < 0.3f)
        {
            fillImage.color = Color.green;  // Green for low overheat levels
        }
        else if (fillAmount < 0.7f)
        {
            fillImage.color = Color.yellow;  // Yellow for medium overheat levels
        }
        else
        {
            fillImage.color = Color.red;  // Red for high overheat levels
        }
    }


    void CalulateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //new vector3(1, 0, 0) * 5 * real time 
      
        Vector3 Direction = new Vector3(horizontalInput, verticalInput, 0);

        if (_speedBoostON == false)
        {

            transform.Translate(Direction * _speed * Time.deltaTime);
        }

        else
        {
            transform.Translate(Direction * _speedBoost * Time.deltaTime);
        }
       
        //if player postition on the y is greater than 0
        //y position = 0 
        //else if position on the y is less than -3.8f
        //y pos = 3.8f 

        //Mathf.Clamp allows for a value that passes through a min / max value 
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 3.9f, 10f), 0);

        float minX = 2f;
        float maxX = 16f;

        // Clamp the player's x position between minX and maxX
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);


        // Set the new position based on the clamped x value
        transform.position = new Vector3(clampedX, transform.position.y, 0);

       // if (transform.position.x >= 12.1)
        //{
            //transform.position = new Vector3(12.1f, transform.position.y, 0);

       // }

       // else if (transform.position.x <= -12.1)
       // {
           // transform.position = new Vector3(-12.1f, transform.position.y, 0);
       // }
    }

    void FireProjectile()
    {
        _canFire = Time.time + _fireRate;

        StartCoroutine(FireProjectileWithDelay());


    }

    private IEnumerator FireProjectileWithDelay()
    {
        // Wait for 0.1 seconds
        yield return new WaitForSeconds(0.1f);

        // Check if triple shot is enabled and instantiate accordingly
        if (_tripleShotON == true)
        {
            Instantiate(_tripleshotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0.8f, -0.1f, 0), Quaternion.identity);
        }
    }

    void HandleSecondaryShot()
    {
        // Check if the E key is pressed and if the cooldown allows firing
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextSecondaryFireTime)
        {
            FireSecondaryShot(); // Fire the secondary shot
            nextSecondaryFireTime = Time.time + secondaryFireRate; // Set cooldown
        }
    }

    void FireSecondaryShot()
    {
        StartCoroutine(FireSecondaryProjectiles());
    }

    private IEnumerator FireSecondaryProjectiles()
    {
        int projectileCount = 4; // Number of projectiles to fire
        float explosionDelay = 0.2f; // Time between each explosion
        float yRange = 1.5f; // Limit for random Y-axis offset
        float forwardOffset = 1.0f; // How far in front of the player the fireballs spawn
        float projectileSpeed = 10f; // Speed at which fireballs move forward

        for (int i = 0; i < projectileCount; i++)
        {
            // Calculate random Y offset for spread effect
            float randomYOffset = Random.Range(-yRange, yRange);

            // Set spawn position in front of the player
            Vector3 spawnPosition = transform.position + new Vector3(forwardOffset, randomYOffset, 0);

            // Instantiate the projectile
            GameObject projectile = Instantiate(prefabA, spawnPosition, Quaternion.identity);

            // Apply forward force to push the projectile outward
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.right * projectileSpeed; // Push fireballs forward
            }

            Debug.Log("Fired projectile at position: " + spawnPosition);

            // Wait before firing the next projectile
            yield return new WaitForSeconds(explosionDelay);
        }
    }

    //void FireSecondaryShot()
    //{
    //    GameObject prefabToFire;

    //    // Determine which prefab to fire based on the hold time
    //    if (holdTime < 1f)
    //    {
    //        prefabToFire = prefabA;  // Fire Prefab A if held for less than 1 second
    //    }
    //    else if (holdTime < 2f)
    //    {
    //        prefabToFire = prefabB;  // Fire Prefab B if held for at least 1 second but less than 2 seconds
    //    }
    //    else
    //    {
    //        prefabToFire = prefabC;  // Fire Prefab C if held for 2 seconds or more (including if held for 3+ seconds)
    //    }

    //    // Instantiate the selected prefab at the same position as the triple shot
    //    Instantiate(prefabToFire, transform.position, Quaternion.identity);

    //    Debug.Log("Fired " + prefabToFire.name + " based on hold time of " + holdTime + " seconds");
    //}

    public void LoseLife()
    {
        _lives--;
        UpdateLivesText();
        //_lives = _lives -1;
        //_lives -= 1;

        if(_lives < 1)
        {
            _spawnManager.OnPlayerDeath1();
            
            Destroy(this.gameObject);
            healthbarVisual.SetActive(false);
            _livesText.text = "0";
            
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (_shieldBoostON == true)
        {
           
            

            return;
        }
        else
        {
            evolutionmanager.PlayerDamaged();
            _health -= damageAmount;
            UpdateLivesText();
            Debug.Log("TAKING DAMAGE");
        }


        if (statsBar != null)
        {
            statsBar.DecreaseHealth(damageAmount);
        }

        if (_health <= 0)
        {
            LoseLife();
            _health = 10;
            statsBar.ResetHealth();
        }
    }

    void UpdateLivesText()
    {
        if (_livesText != null)
        {
            _livesText.text =  _lives.ToString();
        }



    }

    public void HealPlayer(int healAmount)
    {
        IncreaseHealth(healAmount); // Increase player's health

        // Increment the statbar by 0.5 units (or any desired value)
        if (statsBar != null)
        {
            statsBar.IncreaseHealth(0.3f);  
        }
    }

    // Method to increase the player's health directly
    public void IncreaseHealth(int amount)
    {
        // Add the healing amount to the player's health
        _health += amount;

        // Clamp the health to a maximum of 10
        if (_health > 10)
        {
            _health = 10;
        }

        Debug.Log("Health increased by " + amount + ". New health: " + _health);
    }



    public void TripleShotActive()
    {
        _tripleShotON = true;

        if (_tripleShotCoroutine != null)
        {
            StopCoroutine(_tripleShotCoroutine);
        }
       
        _tripleShotCoroutine = StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _tripleShotON = false;
        _tripleShotCoroutine = null;
    }

    public void SpeedBoostActive()
    {
        _speedBoostON = true;

        if (_speedBoostCoroutine != null)
        {
            // Stop the existing coroutine to reset its duration
            StopCoroutine(_speedBoostCoroutine);
        }
        _speedBoostCoroutine = StartCoroutine(SpeedBoostPowerDownRoutine());


        //StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ShieldsActive()
    {
        _shieldBoostON = true;
        _shieldPrefab.SetActive(true);
        Debug.Log("SHIELD ON");

        if (_shieldBoostCoroutine != null)
        {
            // Stop the existing coroutine to reset its duration
            StopCoroutine(_shieldBoostCoroutine);
        }

        _shieldBoostCoroutine = StartCoroutine(ShieldBoostPowerDownRoutine());
    }

    public void DebuffOn()
    {
        _speed = 2.0f;
        _fireRate = 0.35f;
        Debug.Log("Debuff on");

        if (_debuffCoroutine != null)
        {
            StopCoroutine(_debuffCoroutine);
        }

        _debuffCoroutine = StartCoroutine(DebuffPowerDownRoutine());
    }

    


    public void EvolutionOneActive()
    {
        _onFireVisualPrefab.SetActive(true);
        
    }

    public void EvolutionOneOff()
    {
        _onFireVisualPrefab.SetActive(false);
    }

    IEnumerator DebuffPowerDownRoutine()
    {
        yield return new WaitForSeconds(4.0f);
        _speed = 4.0f;
        _fireRate = 0.15f;

    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speedBoostON = false;
        _speedBoostCoroutine = null;
        
    }


    IEnumerator ShieldBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _shieldBoostON = false;
        _shieldPrefab.SetActive(false);
        _shieldBoostCoroutine = null;
        Debug.Log("SHIELD OFF");
    }

}



