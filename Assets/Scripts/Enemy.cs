using System.Collections;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;

    private Vector2 playerPos;
    private Vector3 forward;
    private Vector3 toOther;

    private float _distanceToPlayer;
    private int dodgeDirection = 0;

    private float _lastAttackTime = -Mathf.Infinity;
    private float _attackCoolDown = 1f;

    private bool _isDead;
    private bool _isShieldActive;
    private bool _isCircularMovement = false;
    private bool _isChasing;
    private bool _canFire = true;
    private bool _canDetectShoots = true;

    [Header("Enemy Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _attackSpeed = 10f;
    [SerializeField] private int _points = 10;
    public int Points { get { return _points; } }
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private LayerMask _powerUpMask;
    [SerializeField] private LayerMask _laserMask;
    [SerializeField] private float _rayDistance = 5f;
    [SerializeField] private float _circleCastRadious = 2f;

    [Header("Prefabs")]
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private AudioClip _explosionClip;
    [SerializeField] private AudioClip _laserClip;
    [SerializeField] private GameObject _bulletPrefab;

    [Header("Circular Movement Settings")]
    private Vector3 _circleCenter;
    [SerializeField] float _radious = 5f;
    [SerializeField] float _angle;
    [SerializeField] float _angleSpeed = 3f;

    [Header("Shield Settings")]
    [SerializeField] private Shield _shield;
    [SerializeField] private GameObject _shieldVisualizer;


    void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _shield = GetComponentInChildren<Shield>();

        if (_player == null)
        {
            Debug.LogError("Player is null on Enemy");
        }

        if (_animator == null)
        {
            Debug.LogError("Animator is null on Enemy");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource is NULL on enemy");
        }
        else
        {
            _audioSource.clip = _laserClip;
        }

        if (_shield == null)
        {
            Debug.LogError("Shield is NULL on Enemy");
        }

        if (_isShieldActive)
        {
            _shieldVisualizer.SetActive(true);
        }
        else
        {
            _shieldVisualizer.SetActive(false);
        }

        //Circular Movement Settings
        _radious = Random.Range(0.5f, 2.8f);
        _angleSpeed = Random.Range(0.5f, 3.8f);
        _circleCenter = transform.position;

        int randomMovementCondition = Random.Range(0, 2);
        if (randomMovementCondition == 0)
        {
            _isCircularMovement = false;
        }
        else if (randomMovementCondition == 1)
        {
            _isCircularMovement = true;
        }

        int enemyTypeDetect = Random.Range(0, 2);
        if (enemyTypeDetect == 0)
        {
            _canDetectShoots = false;
        }
        else
        {
            _canDetectShoots = true;
        }

        FireLaser();
    }

    private void Update()
    {
        //Calculate Distance to Player (For Ram Attack)
        if (_player != null)
        {
        playerPos = _player.transform.position;
        _distanceToPlayer = Vector2.Distance(playerPos, transform.position);    
        }

        //if enemy is close to player
        //enemy will move to player fast
        if (_distanceToPlayer < _attackRange && !_isChasing)
        {
            _isChasing = true;
        }

        if (_isChasing == true && !_isDead)
        {
            AttackRamPlayer(playerPos);
        }

        else if (!_isDead)
        {
            CircularMovement();
            CalculateMovementDown();
            AttackPlayerFromBehind();
        }

    }

    private void FixedUpdate()
    {
        if (!_isDead)
        {
            AttackPowerUp();
        }

        if (!_isDead && _canDetectShoots)
        {
            //DetectShoots
            AvoidShots();
        }
    }

    private void CircularMovement()
    {
        if (!_isCircularMovement) return;
        _angle += _angleSpeed * Time.deltaTime;

        float x = _circleCenter.x + Mathf.Cos(_angle) * _radious;
        float y = _circleCenter.y + Mathf.Sin(_angle) * _radious;

        transform.position = new Vector3(x, y, 0);
    }

    private void CalculateMovementDown()
    {
        if (_isCircularMovement)
        {
            _circleCenter += Vector3.down * _moveSpeed * Time.deltaTime; //move the center of the circle down adding the vector multiplied by speed
        }
        else
        {
            transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime); //translate the enemy without 
        }

        if (transform.position.y < -5.5f)
        {
            float randomNum = (Random.Range(-13f, 13f));
            Vector3 newPosition = new Vector3(randomNum, 8f, 0);
            _circleCenter = newPosition;

            if (!_isCircularMovement)
            {
                transform.position = newPosition;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _player.Damage();
            Damage();
        }

        else if (other.gameObject.tag == "Laser" && other.GetComponent<Laser>().IsEnemyLaser == false)
        {
            Destroy(other.gameObject);
            _player.AddScore(_points);
            Damage();
        }
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }
        _isDead = true;
        DeathAnimation();
        Destroy(this.gameObject, 2.8f);
    }

    private void DeathAnimation()
    {
        _animator.SetTrigger("OnEnemyDeath");
        _moveSpeed = 0;
        _angleSpeed = 0;
        Destroy(GetComponent<BoxCollider2D>());
        AudioSource.PlayClipAtPoint(_explosionClip, Camera.main.transform.position, 1f);
    }

    private void FireLaser()
    {
        StartCoroutine(ShootFireRoutine());
    }

    IEnumerator ShootFireRoutine()
    {
        while (!_isDead)
        {
            InstantiateLaser();
            int randomTIme = Random.Range(1, 5);
            yield return new WaitForSeconds(randomTIme);
        }
    }

    private void InstantiateLaser()
    {
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);

        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        foreach (Laser laser in lasers)
        {
            laser.AssignEnemyLaser();
        }
        _audioSource.Play();
    }

    public void ActiveEnemyShield()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    private void AttackRamPlayer(Vector2 playerPos)
    {
        StopAllCoroutines();
        Vector3 dir = _player.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
        transform.position = Vector2.MoveTowards(transform.position, playerPos, _attackSpeed * Time.deltaTime);
    }

    private void AttackPlayerFromBehind()
    {
        forward = Vector3.down;
        toOther = Vector3.Normalize(_player.transform.position - transform.position);
        if (Vector3.Dot(forward, toOther) < -0.7f) //Calculate if Player is behind me
        {
            StartCoroutine(FireBackRoutine());
        }
    }

    IEnumerator FireBackRoutine()
    {
        if (_canFire)
        {
            _canFire = false;
            Instantiate(_bulletPrefab, transform.position + new Vector3(0, 0.84f, 0), Quaternion.identity);
            AudioSource.PlayClipAtPoint(_laserClip, Camera.main.transform.position);
            yield return new WaitForSeconds(Random.Range(2, 4));
            _canFire = true;
        }
    }

    private void AttackPowerUp()
    {
        // Check if there's a powerup in front of the enemy
        // If so, shoot and destroy it
        Vector3 _forwardVector = -transform.up;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _forwardVector, _rayDistance, _powerUpMask);
        if (hit.collider != null && Time.time >= _lastAttackTime + _attackCoolDown)
        {
            _lastAttackTime = Time.time;
            Invoke("InstantiateLaser", 0.1f);
        }
    }

    private void AvoidShots()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _circleCastRadious, -transform.up, 5f, _laserMask);

        if (hit.collider != null)
        {
            if (hit.transform.TryGetComponent<Laser>(out Laser laser) && !_isCircularMovement)
            {
                if (!laser.IsEnemyLaser)
                {
                    if (dodgeDirection == 0)
                    {
                        dodgeDirection = Random.Range(-1, 2);
                        if (dodgeDirection == 0)
                        {
                            dodgeDirection = 1;
                        }
                    }         
                    Debug.Log("Laser detected");
                    transform.Translate((dodgeDirection * Vector3.right) * _moveSpeed * Time.deltaTime);
                }
            }
        }
    }
}

