using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 8.0f;
    private bool _isEnemyLaser = false;
    private bool _isSmartEnemy = false;
    private bool _isHomingLaser = false;
    private GameObject[] _enemies;
    private GameObject _closestEnemy;
    private float _distance;
    private float _nearestDistance = 15;

    void Update()
    {
        if (_isEnemyLaser == false && _isHomingLaser == false)
        {
            MoveUp();
        }
        else if (_isSmartEnemy == true)
        {
            MoveUp();
        }
        else if (_isHomingLaser == true)
        {
            HomingShot();
        }
        else
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        DestroyLaser();
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        DestroyLaser();
    }

    void HomingShot()
    {
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (_enemies.Length == 0)
        {
            MoveUp();
        }
        else
        {
            for (int i = 0; i < _enemies.Length; i++)
            {
                _distance = Vector2.Distance(transform.position, _enemies[i].transform.position);

                if (_distance < _nearestDistance)
                {
                    _closestEnemy = _enemies[i];
                    _nearestDistance = _distance;
                }
            }

            if (_closestEnemy != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, _closestEnemy.transform.position, _speed * Time.deltaTime);
            }

        }

        DestroyLaser();
    }

    private void DestroyLaser()
    {
        if (transform.position.y < -8f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void AssignSmartEnemy()
    {
        _isSmartEnemy = true;
    }

    public void AssignHomingLaser()
    {
        _isHomingLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
        }

        if (other.CompareTag("Player") && _isSmartEnemy == true)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
        }

        if (other.CompareTag("Pickup") && _isEnemyLaser == true)
        {
            Destroy(other.gameObject);
        }
    }
}
