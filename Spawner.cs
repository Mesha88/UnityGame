using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab; // The enemy prefab to spawn
    [SerializeField] Transform[] spawnPoints; // An array of possible spawn points
    [SerializeField] float timeBetweenWaves = 5f; // Time between waves

    public int waveNumber = 1;
    public int enemiesRemaining; // Number of active enemies

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        while (true)
        {
            // Calculate the number of enemies to spawn for this wave
            int enemiesToSpawn = Mathf.CeilToInt(10 * Mathf.Pow(1.1f, waveNumber - 1));
            enemiesRemaining = enemiesToSpawn; // Set remaining enemies to the number we just calculated

            // Spawn the enemies
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
            }

            // Wait until all enemies are defeated before starting the next wave
            while (enemiesRemaining > 0)
            {
                yield return null; // Wait until all enemies are dead
            }

            // Wait a bit before starting the next wave
            yield return new WaitForSeconds(timeBetweenWaves);

            waveNumber++;
        }
    }

    void SpawnEnemy()
    {
        // Randomly select a spawn point from the array
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the enemy at the selected spawn point
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Register the enemy’s death callback
        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
        if (enemyScript != null)
        {
            enemyScript.OnEnemyDeath += OnEnemyDeath; // Use the OnEnemyDeath event from EnemyStats
        }
    }

    void OnEnemyDeath()
    {
        enemiesRemaining--; // Decrement the number of remaining enemies when one dies
    }
}
