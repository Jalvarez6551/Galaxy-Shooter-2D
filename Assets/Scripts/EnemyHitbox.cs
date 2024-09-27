using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private Player _player;
    private Enemy _enemy;
    private SpawnManager _spawnManager;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _enemy = GetComponentInParent<Enemy>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        if (_enemy == null)
        {
            Debug.LogError("Enemy is NULL");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is NULL");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();

                if (_enemy._isBoss == true)
                {
                    _enemy.DamageBoss();
                }
                else
                {
                    if (_enemy._hasShield == true)
                    {
                        _enemy.HandleEnemyShields();
                    }
                    else
                    {
                        _enemy.HandleEnemyDeath();
                        _spawnManager._enemiesDestroyed++;
                    }
                }

            }
        }

        if (other.gameObject.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                if (_enemy._isBoss == true)
                {
                    _enemy.DamageBoss();
                }
                else
                {
                    if (_enemy._hasShield == true)
                    {
                        _enemy.HandleEnemyShields();
                    }
                    else
                    {
                        _player.AddScore(10);
                        _spawnManager._enemiesDestroyed++;
                        _enemy.HandleEnemyDeath();
                    }
                }
            }
        }

        if (other.gameObject.CompareTag("Piercing Laser"))
        {
            if (_player != null)
            {
                if (_enemy._isBoss == true)
                {
                    _enemy.DamageBoss();
                }
                else
                {
                    _player.AddScore(10);
                    _spawnManager._enemiesDestroyed++;
                    _enemy.HandleEnemyDeath();
                }
            }
        }
    }
}
