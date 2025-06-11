using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EnemyBoss : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _health = 1000f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject[] _laserBeam;
    [SerializeField] private AudioClip _laserClip;
    [SerializeField] private AudioClip _laserBeamClip;
    [SerializeField] private AudioClip _explosionClip;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private Transform[] _explosionTransforms;
    [SerializeField] private Transform[] _laserSpawnPositions;


    [Header("Flash Settings")]
    [SerializeField] private float _flashTime = 3f;
    [SerializeField] private Color _flashColor;

    private SpriteRenderer _spriteRenderer;
    private Color _baseColor;
    private CameraShake _shake;
    private GameManager _gameManager;
    private bool _canUseLaserBeam = true;
    private Player _player;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseColor = _spriteRenderer.material.color;
        _shake = GameObject.Find("Camera_Handler/Main_Camera").GetComponent<CameraShake>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_shake == null)
        {
            Debug.LogError("Shake is NULL on enemy");
        }

        if (_gameManager == null)
        {
            Debug.LogError("Game manager is null on Boss");
        }

        StartCoroutine(WaitForShoot());
    }

    void Update()
    {
        _health = Mathf.Clamp(_health, 0, 1500f);
        if (transform.position.y > 4f)
        {
            transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime, Space.World);
        }

        transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
    }

    public void Damage(float damageAmount)
    {
        StartCoroutine(Flash());
        StartCoroutine(_shake.Shake(0.3f,0.4f));
        if (_health <= 250)
        {
            _rotationSpeed = 80f;
        }

        _health -= damageAmount;
        if (_health <= 0)
        {
            foreach (Transform pos in _explosionTransforms)
            {
                Instantiate(_explosionPrefab, pos.transform.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(_explosionClip, Camera.main.transform.position);
            }
            _gameManager.BossDeath();
            Destroy(this.gameObject, 0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (_player != null)
            {
                _player.Damage();
            }
        }

        else if (collision.gameObject.tag == "Laser" && collision.GetComponent<Laser>().IsEnemyLaser == false)
        {
            Damage(75f);
            _player.AddScore(45);
            Destroy(collision.gameObject);
        }
    }

    IEnumerator FireRoutine()
    {
        while (_health > 0)
        {
            for (int i = 0; i < _laserSpawnPositions.Length; i++)
            {
                GameObject laserClone = Instantiate(_laserPrefab, _laserSpawnPositions[i].transform.position, _laserSpawnPositions[i].transform.rotation);
                Laser laser = laserClone.GetComponent<Laser>();
                if (laser != null)
                {
                    laser.AssignEnemyLaser();
                }
            }
            AudioSource.PlayClipAtPoint(_laserClip, Camera.main.transform.position);

            if (_health < 800f && _canUseLaserBeam)
            {
                StartCoroutine(ActivateLaserBeam());
            }
            float randomTime = Random.Range(0, 2.5f);
            yield return new WaitForSeconds(randomTime);
        }
    }

    IEnumerator WaitForShoot()
    {
        yield return new WaitForSeconds(4f);
        StartCoroutine(FireRoutine());
    }

    IEnumerator Flash()
    {
        bool isFlashing = false;
        float timer = 0f;
        while (timer < _flashTime && !isFlashing)
        {
            _spriteRenderer.color = _flashColor;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.color = _baseColor;
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
            isFlashing = true;
        }
    }

    IEnumerator ActivateLaserBeam()
    {
        _canUseLaserBeam = false;
        foreach (GameObject laserBeam in _laserBeam)
        {
            laserBeam.SetActive(true);
        }
        yield return new WaitForSeconds(Random.Range(1, 4));

        foreach (GameObject laserBeam in _laserBeam)
        {
            laserBeam.SetActive(false);
        }
        AudioSource.PlayClipAtPoint(_laserBeamClip, Camera.main.transform.position);
        _canUseLaserBeam = true;
    }

}
