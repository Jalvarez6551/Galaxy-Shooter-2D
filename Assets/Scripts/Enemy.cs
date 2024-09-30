using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    private Player _player;
    private SpawnManager _spawnManager;
    private Animator _animator;
    private AudioSource _explosionSound;
    [SerializeField] private GameObject _enemyLaserPrefab;
    private float _fireRate = 3f;
    private float _canFire = -1f;
    private bool _isAlive = true;
    [SerializeField] private int _movementStyle;
    private int _diagonalDirection;
    private bool _isZigZagRight = true;
    private bool _isZigZagLeft = false;
    public bool _hasShield;
    private int _enemyShieldHealth = 1;
    public GameObject _enemyShieldVisuals;
    private float _distanceFromPlayer;
    [SerializeField] private float _tackleRange = 3;
    [SerializeField] private float _tackleSpeed = 4;
    [SerializeField] private Transform _raycastOriginSmart;
    [SerializeField] private GameObject _smartLaser;
    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private float _dodgeSpeed = 3f;
    private bool _canDodge = true;
    public bool _isBoss = false;
    private int _bossHealth = 15;
    [SerializeField] private GameObject[] _bossLasers;
    [SerializeField] private GameManager _gameManager;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _animator = GetComponent<Animator>();
        _explosionSound = GameObject.Find("Explosion Sound").GetComponent<AudioSource>();

        SpawnWithShield();

        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
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
        EnemyType();
        FireLaser();
        DetectPowerUp();
    }

    private void FireLaser()
    {
        if (_isAlive == true)
        {
            if (Time.time > _canFire)
            {
                _fireRate = Random.Range(3f, 7f);
                _canFire = Time.time + _fireRate;
                if (_movementStyle == 4)
                {
                    DetectPlayer();
                    InstantiateLaser();
                }
                else if (_isBoss == true)
                {
                    int randomLaser = Random.Range(0, 3);
                    GameObject bossLaser = Instantiate(_bossLasers[randomLaser], transform.position, Quaternion.identity);
                    Laser[] lasers = bossLaser.GetComponentsInChildren<Laser>();

                    for (int i = 0; i < _bossLasers.Length; i++)
                    {
                        lasers[i].AssignEnemyLaser();
                    }
                }
                else
                {
                    InstantiateLaser();
                }
            }
        }
    }

    private void InstantiateLaser()
    {
        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void EnemyType()
    {
        switch (_movementStyle)
        {
            case 0:
                MoveDown();
                break;
            case 1:
                if (_diagonalDirection == 0)
                {
                    transform.Translate((Vector3.down + Vector3.left) * _speed * Time.deltaTime);
                }
                else
                {
                    transform.Translate((Vector3.down + Vector3.right) * _speed * Time.deltaTime);
                }
                break;
            case 2:
                MoveDown();
                if (_isZigZagRight == true)
                {
                    StartCoroutine(ZigZagRight());
                }
                else if (_isZigZagLeft == true)
                {
                    StartCoroutine(ZigZagLeft());
                }
                break;
            case 3:
                MoveDown();
                TacklePlayer();
                break;
            case 4:
                MoveDown();
                break;
            case 5:
                MoveDown();
                break;
            case 6:
                BossAI();
                break;
            default:
                Debug.Log("Default Value");
                break;
        }

        if (transform.position.y < -5.5f)
        {
            float randomX = Random.Range(9.5f, -9.5f);
            transform.position = new Vector3(randomX, 7.75f, 0);

            if (randomX >= 0)
            {
                _diagonalDirection = 0;
            }
            else
            {
                _diagonalDirection = 1;
            }
        }

        if (transform.position.x > 10.25f)
        {
            transform.position = new Vector3(-10.25f, transform.position.y, 0);
        }
        else if (transform.position.x < -10.25f)
        {
            transform.position = new Vector3(10.25f, transform.position.y, 0);
        }
    }

    public void HandleEnemyDeath()
    {
        _isAlive = false;
        _animator.SetTrigger("OnEnemyDeath");
        _explosionSound.Play();
        _speed = 0;
        _dodgeSpeed = 0;
        gameObject.tag = "Dead";
        Destroy(GetComponentInChildren<Collider2D>());
        Destroy(gameObject, 2.8f);
    }

    public void HandleEnemyShields()
    {
        if (_hasShield == true)
        {
            while (_enemyShieldHealth >= 0)
            {
                _enemyShieldHealth--;

                if (_enemyShieldHealth <= 0)
                {
                    _hasShield = false;
                    _enemyShieldVisuals.SetActive(false);
                }
            }
        }
    }

    private void SpawnWithShield()
    {
        int randomNumber = Random.Range(0, 5);
        if (randomNumber == 0)
        {
            _hasShield = true;
            _enemyShieldVisuals.SetActive(true);
        }
    }

    private void TacklePlayer()
    {
        _distanceFromPlayer = Vector2.Distance(transform.position, _player.transform.position);
        
        if (_isAlive == true)
        {
            if (_distanceFromPlayer < _tackleRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _tackleSpeed * Time.deltaTime);
            }
        }
    }

    private void DetectPlayer()
    { 
        Vector2 distance = new Vector2(0, 8);
        RaycastHit2D ray = Physics2D.Raycast(_raycastOriginSmart.transform.position, distance);

        if (ray)
        {
            if (ray.collider.CompareTag("Player"))
            {
                GameObject smartLaser = Instantiate(_smartLaser, _raycastOriginSmart.position, Quaternion.identity);
                Laser[] lasers = smartLaser.GetComponentsInChildren<Laser>();
                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignSmartEnemy();
                }
            }
        }
    }

    private void DetectPowerUp()
    {
        Vector2 distance = new Vector2(0, -8);
        RaycastHit2D ray = Physics2D.Raycast(_raycastOrigin.position, distance);

        if (ray)
        {
            if (ray.collider.CompareTag("Pickup"))
            {
                FireLaser();
            }
        }  
    }

    public void DetectLaser()
    {
        if (_canDodge == true)
        {
            transform.Translate(Vector2.right * _dodgeSpeed * Time.deltaTime);
            StartCoroutine(DodgeCooldown()); 
        }
    }

    private void BossAI()
    {
        _isBoss = true;
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, 4), _speed * Time.deltaTime);

        if (_bossHealth <= 0)
        {
            HandleEnemyDeath();
        }
    }

    public void DamageBoss()
    {
        _bossHealth--;
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    IEnumerator ZigZagRight()
    {
        while (_isZigZagRight == true)
        {
            transform.Translate(Vector3.right * _speed * Time.deltaTime);
            yield return new WaitForSeconds(.5f);
            _isZigZagRight = false;
            _isZigZagLeft = true;
        }
    }

    IEnumerator ZigZagLeft()
    {
        while (_isZigZagLeft == true)
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
            yield return new WaitForSeconds(.5f);
            _isZigZagLeft = false;
            _isZigZagRight = true;
        }
    }

    IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(1f);
        _canDodge = false;
        yield return new WaitForSeconds(1.5f);
        _canDodge = true;
    }
}