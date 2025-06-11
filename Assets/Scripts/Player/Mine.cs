using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] GameObject _explosion;
    [SerializeField] AudioClip _explosionClip;

    [SerializeField] Vector2 _boxSize;
    [SerializeField] float _angle;
    
    private Vector2 _center;
    private Player _player;


    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL on Mine");
        }
        StartCoroutine(MineExplosionRoutine());
    }

    void Update()
    {
        _center = transform.position;
    }

    IEnumerator MineExplosionRoutine()
    {
        yield return new WaitForSeconds(5f);
        Instantiate(_explosion, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(_explosionClip, Camera.main.transform.position, 1f);
        ExplosionDamage();
        Destroy(this.gameObject, 0.5f);

    }

    void ExplosionDamage()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(_center, _boxSize, _angle);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.gameObject.tag == "Player")
            {
                _player.Damage();
            }
            else if (collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                _player.AddScore(enemy.Points);
                enemy.Damage();
            }

            else if (collider.TryGetComponent<EnemyRed>(out EnemyRed enemyRed))
            {
                _player.AddScore(enemyRed.Points);
                enemyRed.Damage();
            }

            else if (collider.TryGetComponent<EnemyBoss>(out EnemyBoss enemyBoss))
            {
                _player.AddScore(40);
                enemyBoss.Damage(235f);

            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _boxSize);
    }
}
