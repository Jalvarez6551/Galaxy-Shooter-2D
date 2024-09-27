using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetection : MonoBehaviour
{
    private Enemy _enemy;
    
    void Start()
    {
        _enemy = GetComponentInParent<Enemy>();

        if (_enemy == null )
        {
            Debug.LogError("Enemy is NULL");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Laser"))
        {
            _enemy.DetectLaser();
        }
    }
}
