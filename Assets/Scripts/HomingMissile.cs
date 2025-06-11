using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HomingMissile : MonoBehaviour
{

    private GameObject[] _enemies;
    private GameObject _target;

    private Vector3 dir;
    private float angle;

    [SerializeField] private float _moveSpeed = 15f;
    [SerializeField] private float _damage = 150;

    void Start()
    {
        //Find All enemies in scene
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (_enemies.Length <= 0)
        {
            Debug.LogError("no enemies found");
        }

        _target = GetClosestTarget();

    }

    void Update()
    {
        if (_enemies.Length > 0)
        {
            if (_target != null)
            {
                dir = _target.transform.position - transform.position;
                angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle + -90f, Vector3.forward);
                transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _moveSpeed * Time.deltaTime);
            }

            else
            {
               _target = GetClosestTarget();
            }
        }
    }

    private GameObject GetClosestTarget()
    {
        float closest = 1000f;
        GameObject target = null;
        for (int i = 0; i < _enemies.Length; i++)
        {
            if (_enemies[i] == null) continue;
            
            float dist = Vector3.Distance(_enemies[i].transform.position, transform.position);
            if (dist < closest)
            {
                closest = dist;
                target = _enemies[i];
            }
        }
        return target;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            EnemyRed enemyRed = collision.gameObject.GetComponent<EnemyRed>();
            EnemyBoss enemyBoss = collision.gameObject.GetComponent<EnemyBoss>();

            if (enemy != null)
            {
                enemy.Damage();
                Destroy(this.gameObject);
            }

            if (enemyRed != null)
            {
                enemyRed.Damage();
                Destroy(this.gameObject);
            }

            if (enemyBoss != null)
            {
                enemyBoss.Damage(_damage);
            }
 
        }
    }
}
