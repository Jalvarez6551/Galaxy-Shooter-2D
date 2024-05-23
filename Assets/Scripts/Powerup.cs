using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3;
    [SerializeField] private int _powerupID;
    private AudioSource _powerupSound;

    private void Start()
    {
        _powerupSound = GameObject.Find("Powerup Sound").GetComponent<AudioSource>();

        if (_powerupSound == null )
        {
            Debug.LogError("Powerup Sound is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                    default:
                        Debug.Log("default value");
                        break;
                }
            }
            _powerupSound.Play();
            Destroy(gameObject);
        }
    }
}
