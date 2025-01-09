using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpawnManager2 : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab1;
    [SerializeField]
    private GameObject _enemyContainer1;

    private bool _stopSpawning = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnRoutine()
    {
        // Ensure a unique seed for each enemy outside the loop
        // int seed = System.DateTime.Now.Millisecond;
         //Random.InitState(seed);

        while (_stopSpawning == false)
        {
            // Randomize y-coordinate within the specified range
            float randomY = Random.Range(3.6f, 6f);

            Vector3 spawnPosition = new Vector3(20.3f, randomY, 0);

            Debug.Log("Spawned at Y:" + randomY);

            GameObject newEnemy = Instantiate(_enemyPrefab1, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer1.transform;

            yield return new WaitForSeconds(3.0f);
        }
    }




    public void OnPlayerDeath2()
    {
        _stopSpawning = true;
    }
}

