using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private int _powerupID;
    private AudioSource _powerupSound;
    [SerializeField] private float _moveTowardSpeed = 3f;
    private Transform _player;

    private void Start()
    {
        _powerupSound = GameObject.Find("Powerup Sound").GetComponent<AudioSource>();

        _player = GameObject.Find("Player").GetComponent<Transform>();

        if (_powerupSound == null )
        {
            Debug.LogError("Powerup Sound is NULL");
        }

        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();

        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -6.25)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.AmmoRefill();
                        break;
                    case 4:
                        player.HealthPowerUp();
                        break;
                    case 5:
                        player.PiercingShotActive();
                        break;
                    case 6:
                        player.SpeedDecreaseActive();
                        break;
                    case 7:
                        player.HomingShotActive();
                        break;
                    default:
                        Debug.Log("default value");
                        break;
                }
            }
            _powerupSound.Play();
            Destroy(gameObject);
        }
    }

    private void MoveTowardsPlayer()
    {
        if (Input.GetKey(KeyCode.C))
        {
            _speed = 0;
            transform.position = Vector2.MoveTowards(transform.position, _player.position, _moveTowardSpeed * Time.deltaTime);
        }
        else
        {
            _speed = 3;
        }
    }
}
