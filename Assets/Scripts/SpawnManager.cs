﻿using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject[] _PowerupPrefab;
    [SerializeField] private float _spawnTime;
    [SerializeField] private GameObject _enemyContainer;
    private bool _stopSpawning = false;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnRarePowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(randomX, 7.75f, 0), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_spawnTime);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 randomSpawn = new Vector3(Random.Range(-9.25f, 9.25f), 8, 0);
            int randomPowerUp = Random.Range(0, 5);
            yield return new WaitForSeconds(Random.Range(3, 8));
            Instantiate(_PowerupPrefab[randomPowerUp], randomSpawn, Quaternion.identity);
        }
    }

    IEnumerator SpawnRarePowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 randomSpawn = new Vector3(Random.Range(-9.25f, 9.25f), 8, 0);
            int randomPowerUp = Random.Range(5, 21);
            yield return new WaitForSeconds(Random.Range(3, 8));
            if (randomPowerUp == 5)
            {
                Instantiate(_PowerupPrefab[5], randomSpawn, Quaternion.identity);
            }
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}