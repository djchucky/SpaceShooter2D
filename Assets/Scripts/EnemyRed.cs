using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRed : MonoBehaviour
{
    private bool _isFalling = true;
    private float _fixedX = 0f;
    private float _sinOffsetX = 0f;
    private bool _isDead = false;

    private AudioSource _audioSource;
    private Player _player;

    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] AudioClip _explosionClip;
    [SerializeField] private int _points = 30;
    public int Points{get{ return _points; }}

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _amplitude = 3f;
    [SerializeField] private float _zigZagSpeed = 5f;

    [Header("Laser")]
    [SerializeField] private GameObject _laserObject;
    [SerializeField] private AudioClip _laserBeamClip;

    void Start()
    {
        _speed = Random.Range(2, 4);
        _amplitude = Random.Range(2.5f, 8);
        _zigZagSpeed = Random.Range(1, 3.5f);
        _player = GameObject.Find("Player").GetComponent<Player>();

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Audiosource is Null on EnemyRed");
        }

        _laserObject.SetActive(false);
        Fire();

    }

    void Update()
    {
        if(!_isDead)
        CalculateZigZagMovement();
    }

    private void CalculateZigZagMovement()
    {
        if (_isFalling)
        {
            // Move downward while keeping a fixed X position
            transform.Translate(Vector3.down * _speed * Time.deltaTime, Space.World);

            // When object goes off the bottom of the screen, respawn it at the top
            if (transform.position.y < -6f)
            {
                _fixedX = Random.Range(-13f, 13f); // choose a new random X for falling
                transform.position = new Vector3(_fixedX, 9.5f, transform.position.z);
            }

            // Once the object has re-entered visible area from the top,
            // prepare to transition back to zig-zag mode
            if (transform.position.y < 8f && transform.position.y > 7f)
            {
                // Compute the horizontal offset to make the sine wave start
                // from the current X position, preventing it from snapping to 0
                _sinOffsetX = _fixedX - Mathf.Sin(Time.time * _zigZagSpeed) * _amplitude;

                // Switch to zig-zag mode on next frame
                _isFalling = false;
            }
        }
        else
        {
            // Calculate zig-zag horizontal movement using sine wave
            float x = Mathf.Sin(Time.time * _zigZagSpeed) * _amplitude + _sinOffsetX;

            // Continue moving downward
            float y = transform.position.y - _speed * Time.deltaTime;
            transform.position = new Vector3(x, y, transform.position.z);

            // If object falls below screen, reset it and switch to falling mode
            if (transform.position.y < -6f)
            {
                _fixedX = Random.Range(-13f, 13f); // new position for falling phase
                transform.position = new Vector3(_fixedX, 8.5f, transform.position.z);
                _isFalling = true;
            }
        }
    }

    private void Fire()
    {
        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        while (!_isDead)
        {
            yield return new WaitForSeconds(Random.Range(1, 3.5f));
            _laserObject.SetActive(true);
            _audioSource.clip = _laserBeamClip;
            _audioSource.Play();
            yield return new WaitForSeconds(Random.Range(1, 4));
            _laserObject.SetActive(false);
            _audioSource.Stop();
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (_player != null)
            {
                _player.Damage();
                _player.AddScore(_points);
                Damage();
            }
        }

        if (collision.tag == "Laser" && collision.GetComponent<Laser>().IsEnemyLaser == false)
        {
            if (_player != null)
            {
                _player.AddScore(_points);
            }   
            Destroy(collision.gameObject);
            Damage();            
        }
    }

    public void Damage()
    {
        _isDead = true;
        _laserObject.SetActive(false);
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(_explosionClip, Camera.main.transform.position);
        Destroy(this.gameObject, 0.5f);
    }
}
