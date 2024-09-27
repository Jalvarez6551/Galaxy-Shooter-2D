using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private float _fireRate = 0.15f;
    private float _canFire = -1f;
    [SerializeField] private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    [SerializeField] private float _tripleShotCoolDownTime = 5f;
    private bool _isSpeedBoostActive = false;
    [SerializeField] private float _speedBoostCoolDownTime = 5f;
    private bool _isSpeedDecreaseActive = false;
    [SerializeField] private float _speedDecreaseCoolDownTime = 5f;
    private bool _isShieldActive = false;
    [SerializeField] private GameObject _shieldVisuals;
    [SerializeField] private int _score;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameObject _rightEngine, _leftEngine;
    private AudioSource _laserShotSound;
    private AudioSource _explosionSound;
    private int _shieldHealth = 0;
    [SerializeField] private SpriteRenderer _shieldColor;
    [SerializeField] private int _ammoCount = 20;
    private AudioSource _noAmmoSound;
    [SerializeField] private GameObject _piercingLaserPrefab;
    private bool _isPiercingShotActive = false;
    [SerializeField] private float _piercingShotCoolDownTime = 5f;
    [SerializeField] private Scrollbar _thrusterBar;
    private Animator _camera_Shake;
    private bool _isHomingShotActive = false;
    [SerializeField] private float _homingShotCoolDownTime = 5f;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _laserShotSound = GameObject.Find("Laser Shot Sound").GetComponent<AudioSource>();
        _explosionSound = GameObject.Find("Explosion Sound").GetComponent<AudioSource>();
        _noAmmoSound = GameObject.Find("No Ammo Sound").GetComponent<AudioSource>();
        _camera_Shake = GameObject.Find("Main Camera").GetComponent<Animator>();

        _uiManager.UpdateLives(_lives);

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manger is NULL");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }

        if (_laserShotSound == null)
        {
            Debug.LogError("Laser Shot Sound is NULL");
        }

        if (_explosionSound == null)
        {
            Debug.LogError("Explosion Sound is NULL");
        }

        if (_noAmmoSound == null)
        {
            Debug.LogError("No Ammo Sound is NULL");
        }

        if (_camera_Shake == null)
        {
            Debug.LogError("No Camera Animator");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

        if (_isSpeedBoostActive == true || Input.GetKey(KeyCode.LeftShift))
        {
            _thrusterBar.size += 2f * Time.deltaTime;
        }
        else
        {
            _thrusterBar.size -= 2f * Time.deltaTime;
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }

        if (_isSpeedBoostActive == true || Input.GetKey(KeyCode.LeftShift) && _isSpeedDecreaseActive == false)
        {
            _speed = 10.0f;
        }
        else if (_isSpeedDecreaseActive == true)
        {
            _speed = 3.0f;
        }
        else
        {
            _speed = 8.0f;
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        _ammoCount--;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        else if (_isPiercingShotActive == true)
        {
            Instantiate(_piercingLaserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        else if (_isHomingShotActive == true)
        {
            GameObject homingLaser = Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            Laser lasers = homingLaser.GetComponent<Laser>();
            lasers.AssignHomingLaser();
        }
        else if (_ammoCount >= 0)
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }


        if (_ammoCount >= 0)
        {
            _laserShotSound.Play();
        }
        else
        {
            _ammoCount = 0;
            _noAmmoSound.Play();
        }
    }

    public void Damage()
    {
        HandleShields();

        _camera_Shake.SetTrigger("Player Damaged");

        if (_isShieldActive == false)
        {
            _lives--;
        }

        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _explosionSound.Play();
            Destroy(gameObject);
        }
    }

    private void HandleShields()
    {
        if (_isShieldActive == true)
        {
            while (_shieldHealth >= 0)
            {
                _shieldHealth--;

                if (_shieldHealth == 2)
                {
                    _shieldColor.color = new Color(1f, 1f, 1f, .75f);
                }
                else if (_shieldHealth == 1)
                {
                    _shieldColor.color = new Color(1f, 1f, 1f, .50f);
                }

                if (_shieldHealth <= 0)
                {
                    _isShieldActive = false;
                    _shieldVisuals.SetActive(false);
                }
                return;
            }
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_tripleShotCoolDownTime);
        _isTripleShotActive = false;
    }

    public void PiercingShotActive()
    {
        _isPiercingShotActive = true;
        StartCoroutine(PiercingShotPowerDownRouting());
    }

    IEnumerator PiercingShotPowerDownRouting()
    {
        yield return new WaitForSeconds(_piercingShotCoolDownTime);
        _isPiercingShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(_speedBoostCoolDownTime);
        _isSpeedBoostActive = false;
    }

    public void SpeedDecreaseActive()
    {
        _isSpeedDecreaseActive = true;
        StartCoroutine(SpeedDecreasePowerDownRoutine());
    }

    IEnumerator SpeedDecreasePowerDownRoutine()
    {
        yield return new WaitForSeconds(_speedDecreaseCoolDownTime);
        _isSpeedDecreaseActive = false;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldHealth = 3;
        _shieldColor.color = new Color(1f, 1f, 1f, 1f);
        _shieldVisuals.SetActive(true);
    }

    public void AmmoRefill()
    {
        _ammoCount = 15;
    }

    public void HealthPowerUp()
    {
        if (_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
        }

        if (_lives == 2)
        {
            _leftEngine.SetActive(false);
        }
        else if (_lives == 3)
        {
            _rightEngine.SetActive(false);
        }
    }

    public void HomingShotActive()
    {
        _isHomingShotActive = true;
        StartCoroutine(HomingShotPowerDown());
        
    }

    IEnumerator HomingShotPowerDown()
    {
        yield return new WaitForSeconds(_homingShotCoolDownTime);
        _isHomingShotActive = false;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}