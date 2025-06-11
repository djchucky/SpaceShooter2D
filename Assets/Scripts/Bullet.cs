using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;

    void Update()
    {
        transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime);
        if (transform.position.y > 8f)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player _player = collision.GetComponent<Player>();
            if (_player != null)
            {
                _player.Damage();
                Destroy(this.gameObject);
            }
        }
    }
}
