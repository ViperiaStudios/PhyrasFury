using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using UnityEngine;

using System.Collections;
using UnityEngine;

public class NewSpawnRoutine3 : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab1;
    [SerializeField] private GameObject _enemyPrefab2;
    [SerializeField] private GameObject _enemyPrefab3;
    [SerializeField] private GameObject _enemyPrefab4;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject bossObject; // Reference to the boss

    [SerializeField] private GameObject _enemyPrefab5;
    [SerializeField] private GameObject _enemyPrefab6;
    [SerializeField] private GameObject _enemyPrefab7;

    [SerializeField] private GameObject[] powerups;

    private bool _stopSpawning = false;
    private bool _stopSpawningPowerUp = false;

    private int maxEnemies = 100;  // ✅ Maximum number of active enemies
    private int currentEnemyCount = 0;

    private int maxPowerUps = 10;  // ✅ Maximum number of active power-ups
    private int currentPowerUpCount = 0;

    private int logCount = 0;  // ✅ Limit Debug.Log spam

    void Start()
    {
        StartSpawning();
        StartCoroutine(CheckForBossDeath());
    }

    void StartSpawning()
    {
        _stopSpawning = false;
        _stopSpawningPowerUp = false;

        StartCoroutine(SpawnPowerupRoutine());

        if (_enemyPrefab1 != null) StartCoroutine(SpawnRoutine(_enemyPrefab1, 5f, 10f, 20.3f, 3.0f));
        if (_enemyPrefab2 != null) StartCoroutine(DelayedStart(() => SpawnRoutine(_enemyPrefab2, 3.6f, 6f, 20.3f, 3.5f), 10.0f));
        if (_enemyPrefab3 != null) StartCoroutine(DelayedStart(() => SpawnRoutine(_enemyPrefab3, 5.5f, 7.5f, 18f, 3.5f), 20.0f));
        if (_enemyPrefab4 != null) StartCoroutine(DelayedStart(() => SpawnRoutine(_enemyPrefab4, 10f, 11.24f, 18.83f, 8.0f), 30.0f));
    }

    void StopSpawning()
    {
        _stopSpawning = true;
        _stopSpawningPowerUp = true;
    }

    IEnumerator DelayedStart(System.Func<IEnumerator> coroutine, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!_stopSpawning)
        {
            StartCoroutine(coroutine.Invoke());
        }
    }

    IEnumerator SpawnRoutine(GameObject enemyPrefab, float minY, float maxY, float xPos, float delay)
    {
        while (!_stopSpawning)
        {
            if (enemyPrefab == null)
            {
                if (logCount % 50 == 0)
                {
                    Debug.LogWarning("SpawnRoutine attempted to spawn a null enemyPrefab! Skipping this spawn.");
                }
                logCount++;
                yield break;
            }

            // ✅ Check if enemy count has reached the limit
            if (currentEnemyCount >= maxEnemies)
            {
                Debug.LogWarning("Too many enemies! Stopping spawn.");
                yield break;
            }

            float randomY1 = Random.Range(minY, maxY);
            float randomY2;

            do
            {
                randomY2 = Random.Range(minY, maxY);
            } while (Mathf.Abs(randomY1 - randomY2) < 1f || Mathf.Abs(randomY1 - randomY2) > 3f);

            Vector3 spawnPosition1 = new Vector3(xPos, randomY1, 0);
            Vector3 spawnPosition2 = new Vector3(xPos, randomY2, 0);

            GameObject enemy1 = Instantiate(enemyPrefab, spawnPosition1, Quaternion.identity);
            GameObject enemy2 = Instantiate(enemyPrefab, spawnPosition2, Quaternion.identity);

            enemy1.transform.parent = _enemyContainer.transform;
            enemy2.transform.parent = _enemyContainer.transform;

            currentEnemyCount += 2;  // ✅ Track the number of active enemies
            Debug.Log("Active Enemies: " + currentEnemyCount);

            // ✅ Automatically remove enemies after 30 seconds
            Destroy(enemy1, 30f);
            Destroy(enemy2, 30f);
            StartCoroutine(DecreaseEnemyCountAfterDelay(30f));

            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator DecreaseEnemyCountAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentEnemyCount -= 2;
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (!_stopSpawningPowerUp && powerups.Length > 1)
        {
            if (currentPowerUpCount >= maxPowerUps)
            {
                yield return new WaitForSeconds(5f);
                continue;
            }

            float randomY1 = Random.Range(3.6f, 10f);
            float randomY2;

            do
            {
                randomY2 = Random.Range(3.6f, 10f);
            } while (Mathf.Abs(randomY1 - randomY2) < 1f || Mathf.Abs(randomY1 - randomY2) > 3f);

            Vector3 spawnPosition1 = new Vector3(20.3f, randomY1, 0);
            Vector3 spawnPosition2 = new Vector3(20.3f, randomY2, 0);

            int randomPowerUp1 = Random.Range(0, powerups.Length);
            int randomPowerUp2;

            do
            {
                randomPowerUp2 = Random.Range(0, powerups.Length);
            } while (randomPowerUp1 == randomPowerUp2);

            Instantiate(powerups[randomPowerUp1], spawnPosition1, Quaternion.identity);
            Instantiate(powerups[randomPowerUp2], spawnPosition2, Quaternion.identity);

            currentPowerUpCount += 2;

            yield return new WaitForSeconds(Random.Range(5, 8));
        }
    }

    IEnumerator CheckForBossDeath()
    {
        yield return new WaitForSeconds(60f);
        StopSpawning();

        while (bossObject != null)
        {
            yield return null;
        }

        Debug.Log("Boss Destroyed! Starting new enemy waves...");
        StartNewSpawnRoutines();
    }

    void StartNewSpawnRoutines()
    {
        _stopSpawning = false;
        _stopSpawningPowerUp = false;

        if (_enemyPrefab5 != null) StartCoroutine(SpawnRoutine(_enemyPrefab5, 4f, 9f, 20.3f, 2.5f));
        if (_enemyPrefab6 != null) StartCoroutine(DelayedStart(() => SpawnRoutine(_enemyPrefab6, 5f, 8f, 19.5f, 3.5f), 10.0f));
        if (_enemyPrefab7 != null) StartCoroutine(DelayedStart(() => SpawnRoutine(_enemyPrefab7, 6f, 10f, 18f, 4.5f), 20.0f));
    }

    public void OnPlayerDeath1()
    {
        _stopSpawning = true;
        _stopSpawningPowerUp = true;
    }
}
