using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    private Player _player;
    private Animator _animator;
    private AudioSource _explosionSound;
    [SerializeField] private GameObject _enemyLaserPrefab;
    private float _fireRate = 3f;
    private float _canFire = -1f;
    private bool _isAlive = true;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _animator = GetComponent<Animator>();
        _explosionSound = GameObject.Find("Explosion Sound").GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        if (_animator == null )
        {
            Debug.LogError("Animator is NULL");
        }

        if (_explosionSound == null )
        {
            Debug.LogError("Explosion Sound is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (_isAlive == true)
        {
            if (Time.time > _canFire)
            {
                _fireRate = Random.Range(3f, 7f);
                _canFire = Time.time + _fireRate;
                GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignEnemyLaser();
                }
            }
        }
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.5f)
        {
            float randomX = Random.Range(9.5f, -9.5f);
            transform.position = new Vector3(randomX, 7.75f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (_player != null)
            {
                _player.Damage();
            }
            HandleEnemyDeath();
        }

        if (other.gameObject.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            HandleEnemyDeath();
        }

        if (other.gameObject.tag == "Piercing Laser")
        {
            if (_player != null)
            {
                _player.AddScore(10);
            }
            HandleEnemyDeath();
        }
    }

    private void HandleEnemyDeath()
    {
        _isAlive = false;
        _animator.SetTrigger("OnEnemyDeath");
        _explosionSound.Play();
        _speed = 0;
        Destroy(GetComponent<Collider2D>());
        Destroy(gameObject, 2.8f);
    }
}