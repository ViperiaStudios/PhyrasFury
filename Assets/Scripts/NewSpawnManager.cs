using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab1;
    [SerializeField] private GameObject _enemyPrefab2;
    [SerializeField] private GameObject _enemyPrefab3;
    [SerializeField] private GameObject _enemyPrefab4;
    [SerializeField] private GameObject _enemyContainer;

    [SerializeField] private GameObject _enemyPrefab5;
    [SerializeField] private GameObject _enemyPrefab6;
    [SerializeField] private GameObject _enemyPrefab7;

    [SerializeField] private GameObject bossObject; // Reference to the boss

    [SerializeField] private GameObject[] powerups;

    private bool _stopSpawning = false;
    private bool _stopSpawningPowerUp = false;

    void Start()
    {
        StartSpawning();
        StartCoroutine(CheckForBossDeath()); // ✅ New coroutine to check for boss death
    }

    void StartSpawning()
    {
        _stopSpawning = false;
        _stopSpawningPowerUp = false;

        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnRoutine1());
        if (_enemyPrefab2 != null) StartCoroutine(DelayedStart(SpawnRoutine2, 10.0f));
        if (_enemyPrefab3 != null) StartCoroutine(DelayedStart(SpawnRoutine3, 20.0f));
        if (_enemyPrefab4 != null) StartCoroutine(DelayedStart(SpawnRoutine4, 30.0f));

        StartCoroutine(StopSpawningAfterTime(60f));
    }

    void StopEnemySpawning()
    {
        _stopSpawning = true;
    }

    IEnumerator DelayedStart(System.Func<IEnumerator> coroutine, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!_stopSpawning)
        {
            StartCoroutine(coroutine.Invoke());
        }
    }

    IEnumerator SpawnRoutine1()
    {
        while (!_stopSpawning)
        {
            float randomY = Random.Range(5f, 10f);
            Vector3 spawnPosition = new Vector3(20.3f, randomY, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab1, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator SpawnRoutine2()
    {
        while (!_stopSpawning)
        {
            float randomY = Random.Range(3.6f, 6f);
            Vector3 spawnPosition = new Vector3(20.3f, randomY, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab2, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(3.0f);
        }
    }

    IEnumerator SpawnRoutine3()
    {
        while (!_stopSpawning)
        {
            float randomY = Random.Range(5.5f, 7.5f);
            float randomX = Random.Range(17f, 19f);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab3, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(3.0f);
        }
    }

    IEnumerator SpawnRoutine4()
    {
        while (!_stopSpawning)
        {
            float randomY = Random.Range(10f, 11.24f);
            Vector3 spawnPosition = new Vector3(18.83f, randomY, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab4, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(8.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (!_stopSpawningPowerUp)
        {
            float randomY = Random.Range(3.6f, 10f);
            Vector3 spawnPosition = new Vector3(20.3f, randomY, 0);

            GameObject powerupToSpawn = GetWeightedRandomPowerUp();

            if (powerupToSpawn != null)
            {
                Instantiate(powerupToSpawn, spawnPosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(Random.Range(5, 8));
        }
    }

    // Function to choose power-ups with weighted probability (without Debuff)
    private GameObject GetWeightedRandomPowerUp()
    {
        int[] spawnChances = { 3, 3, 3, 1 };
        // Higher values = more frequent spawns.
        // 3 = TripleShot, 3 = Speed, 3 = Shields, 1 = Health (spawns the least)

        List<GameObject> weightedList = new List<GameObject>();

        for (int i = 0; i < spawnChances.Length; i++)
        {
            for (int j = 0; j < spawnChances[i]; j++)
            {
                weightedList.Add(powerups[i]);
            }
        }

        int randomIndex = Random.Range(0, weightedList.Count);
        return weightedList[randomIndex];
    }

    //IEnumerator SpawnPowerupRoutine()
    //{
    //    while (!_stopSpawningPowerUp)
    //    {
    //        float randomY = Random.Range(3.6f, 10f);
    //        Vector3 spawnPosition = new Vector3(20.3f, randomY, 0);
    //        int randomPowerUp = Random.Range(0, powerups.Length);

    //        if (powerups[randomPowerUp] != null)
    //        {
    //            Instantiate(powerups[randomPowerUp], spawnPosition, Quaternion.identity);
    //        }

    //        yield return new WaitForSeconds(Random.Range(5, 8));
    //    }
    //}

    IEnumerator StopSpawningAfterTime(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        StopEnemySpawning();
    }

    public void OnPlayerDeath1()
    {
        _stopSpawning = true;
        _stopSpawningPowerUp = true;
    }

    IEnumerator CheckForBossDeath()
    {
        yield return new WaitForSeconds(60f); // Wait 60 seconds before stopping spawns
        StopEnemySpawning();

        while (bossObject != null)
        {
            yield return null;
        }

        Debug.Log("Boss Destroyed! Restarting SpawnRoutine1, SpawnRoutine2, and New Spawns...");
        RestartSpawnRoutines();
    }

    void RestartSpawnRoutines()
    {
        _stopSpawning = false; // ✅ Allow spawning again

        // ✅ Restart old routines
        StartCoroutine(SpawnRoutine1());
        if (_enemyPrefab2 != null) StartCoroutine(DelayedStart(SpawnRoutine2, 10.0f));

        // ✅ Start new enemy waves
        if (_enemyPrefab5 != null) StartCoroutine(SpawnRoutine5());
        if (_enemyPrefab6 != null) StartCoroutine(DelayedStart(SpawnRoutine6, 10.0f));
        if (_enemyPrefab7 != null) StartCoroutine(DelayedStart(SpawnRoutine7, 20.0f));
    }

    IEnumerator SpawnRoutine5()
    {
        while (!_stopSpawning)
        {
            float randomY = Random.Range(4f, 9f);
            Vector3 spawnPosition = new Vector3(21f, randomY, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab5, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(2.5f);
        }
    }

    IEnumerator SpawnRoutine6()
    {
        while (!_stopSpawning)
        {
            float randomY = Random.Range(5f, 8f);
            Vector3 spawnPosition = new Vector3(21f, randomY, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab6, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(3.5f);
        }
    }

    IEnumerator SpawnRoutine7()
    {
        while (!_stopSpawning)
        {
            float randomY = Random.Range(6f, 10f);
            Vector3 spawnPosition = new Vector3(21f, randomY, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab7, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(4.5f);
        }
    }
}


