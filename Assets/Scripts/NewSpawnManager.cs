using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab1;
    [SerializeField]
    private GameObject _enemyPrefab2;
    [SerializeField]
    private GameObject _enemyPrefab3;
    [SerializeField]
    private GameObject _enemyPrefab4;
    [SerializeField]
    private GameObject _enemyContainer;
    
    [SerializeField]
    [SerializeReference]
    private GameObject[] powerups;

    private bool _stopSpawning = false;
    private bool _stopSpawningPowerUp = false; 
    

    void Start()
    {
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnRoutine1());
       if (_enemyPrefab2 != null) StartCoroutine(DelayedStart(SpawnRoutine2, 10.0f));
        if (_enemyPrefab3 != null) StartCoroutine(DelayedStart(SpawnRoutine3, 20.0f));
        if (_enemyPrefab4 != null) StartCoroutine(DelayedStart(SpawnRoutine4, 30.0f));

        StartCoroutine(StopSpawningAfterTime(60f));

    }

    void Update()
    {

    }

    IEnumerator DelayedStart(System.Func<IEnumerator> coroutine, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(coroutine.Invoke());
    }

    IEnumerator SpawnRoutine1()
    {
        while (!_stopSpawning)
        {
            float randomY = Random.Range(5f, 10f);
            Vector3 spawnPosition = new Vector3(20.3f, randomY, 0);

            Debug.Log("Spawned at Y (SpawnManager1): " + randomY);

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

            Debug.Log("Spawned at Y (SpawnManager2): " + randomY);

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
            Vector3 spawnPosition = new Vector3(18f, randomY, 0);

            Debug.Log("Spawned at Y (SpawnManager3): " + randomY);

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

            Debug.Log("Spawned at Y (SpawnManager3): " + randomY);

            GameObject newEnemy = Instantiate(_enemyPrefab4, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(8.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (!_stopSpawningPowerUp && powerups != null)
        {
            float randomY = Random.Range(3.6f, 10f);
            Vector3 spawnPosition = new Vector3(20.3f, randomY, 0);
            int randomPowerUp = Random.Range(0, 4);
            if (powerups[randomPowerUp] != null)
            {
                Instantiate(powerups[randomPowerUp], spawnPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(5, 8));
        }
    }

    IEnumerator StopSpawningAfterTime(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        _stopSpawning = true;  // This stops all enemy spawn routines
        Debug.Log("Stopping enemy spawns after " + timeInSeconds + " seconds.");
    }

    public void OnPlayerDeath1()
    {
        _stopSpawning = true;
        _stopSpawningPowerUp = true;
    }

   
}

