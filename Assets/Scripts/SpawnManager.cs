using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
    [SerializeField] private GameObject[] _PowerupPrefab;
    [SerializeField] private GameObject[] _RarePowerupPrefab;
    [SerializeField] private float _spawnTime;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private float _waveCoolDown = 3;
    [SerializeField] private int _enemiesInWave = 10;
    private int _totalEnemyCount;
    public int _enemiesDestroyed;
    private bool _stopSpawning = false;
    private bool _stopSpawningEnemies = false;
    [SerializeField] private int _waveCount;
    [SerializeField] private GameObject _boss;

    private void Update()
    {
        if (_totalEnemyCount == _enemiesInWave)
        {
            _stopSpawningEnemies = true;

            if (_enemiesDestroyed == _enemiesInWave)
            {
                StartCoroutine(WaveCooldown());
            }
        }
    }

    public void StartSpawning()
    { 
        if (_waveCount == 5)
        {
            SpawnBoss();
            StartCoroutine(SpawnPowerupRoutine());
            StartCoroutine(SpawnRarePowerupRoutine());
        }
        else
        {
            StartCoroutine(SpawnEnemyRoutine());
            StartCoroutine(SpawnPowerupRoutine());
            StartCoroutine(SpawnRarePowerupRoutine());
        }
    }

    private void SpawnBoss()
    {
        Instantiate(_boss, new Vector3(0, 10), Quaternion.identity);
    }

    IEnumerator WaveCooldown()
    {
        yield return new WaitForSeconds(_waveCoolDown);
        if (_stopSpawningEnemies == true)
        {
            _waveCount++;
            _enemiesDestroyed = 0;
            _totalEnemyCount = 0;
            _enemiesInWave += 2;
            _stopSpawningEnemies = false;
            StartSpawning();
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawningEnemies == false)
        {
            _totalEnemyCount++;
            float randomX = Random.Range(-9.5f, 9.5f);
            int randomEnemySpawn = Random.Range(0, 6);
            GameObject newEnemy = Instantiate(_enemyPrefab[randomEnemySpawn], new Vector3(randomX, 7.75f, 0), Quaternion.identity);
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
            if (randomPowerUp < 2)
            {
                Instantiate(_PowerupPrefab[randomPowerUp], randomSpawn, Quaternion.identity);
            }
        }
    }

    IEnumerator SpawnRarePowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 randomSpawn = new Vector3(Random.Range(-9.25f, 9.25f), 8, 0);
            int randomRarePowerUp = Random.Range(0, 21);
            yield return new WaitForSeconds(Random.Range(3, 8));
            if (randomRarePowerUp < 5)
            {
                Instantiate(_RarePowerupPrefab[randomRarePowerUp], randomSpawn, Quaternion.identity);
            }
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        _stopSpawningEnemies = true;
    }
}