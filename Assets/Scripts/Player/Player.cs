using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _nextFire = 0f;
    private float _horizontalInput;
    private float _verticalInput;
    private int _score = 0;

    private bool _isTripleShotActive = false;
    private bool _isSpeedPowerActive = false;
    private bool _isShieldActive = false;
    private bool _isSprinting = true;
    private bool _isMinePowerActive = false;
    private bool _canShoot = true;
    private bool _isHomingMissileActive;


    //Managers and miscellaneous
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private AudioSource _audioSource;
    private ChargeBar _chargeBar;
    private Shield _shield;

    [SerializeField] CameraShake _shake;

    [Header("Player Settings")]
    [Range(1, 10)][SerializeField] private float _speed = 5f;
    [Range(5, 15)][SerializeField] private float _sprintSpeed = 10f;
    [SerializeField] private float _boostedSpeed;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _lives = 3;
    [Range(3, 10)][SerializeField] private float _speedPowerUp = 7f;
    [SerializeField] private float _playerPowerUpSpeed = 15f;


    [Header("PowerUps and Projectiles")]
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShootLaser;
    [SerializeField] private Vector3 _laserOffset = new Vector3(-0.04f, 1.69f, 0);
    [Range(2, 5)][SerializeField] private float _lerpTime = 3;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _minePrefab;
    [SerializeField] private GameObject _homingMissile;

    [Header("Player Ship Engine")]
    [SerializeField] private GameObject[] _enginesDamagePrefabs;
    [SerializeField] private GameObject _explosion;

    [Header("Player Ammo")]
    [SerializeField] private int _currentAmmo = 15;
    [SerializeField] private int _maxAmmo = 15;

    [Header("Audios")]
    [SerializeField] private AudioClip _laserClip;
    [SerializeField] private AudioClip _explosionClip;
    [SerializeField] private AudioClip _noAmmo;
    [SerializeField] private AudioClip _mineShootClip;


    void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        transform.position = new Vector3(0f, 0f, 0f);
        _currentAmmo = _maxAmmo;

        _shieldVisualizer = GameObject.Find("Player/Shield");
        _shield = _shieldVisualizer.GetComponent<Shield>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _chargeBar = GameObject.Find("Canvas/Charge_Bar").GetComponent<ChargeBar>();

        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is null on Player");
        }

        if (_shieldVisualizer == null)
        {
            Debug.LogError("Shield visualizer is null on player");
        }
        _shieldVisualizer.SetActive(false);

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is null on player");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio source is NULL on player");
        }
        else
        {
            _audioSource.clip = _laserClip;
        }

        if (_chargeBar == null)
        {
            Debug.LogError("ChargeBar is null on Player");
        }

        if (_shield == null)
        {
            Debug.LogError("Shield is null on PLAYER");
        }

        _uiManager.UpdateAmmoText(_currentAmmo);
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire && _canShoot)
        {
            FireLaser();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CollectPowerUps();
        }

    }

    void CalculateMovement()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        Vector3 _direction = new Vector3(_horizontalInput, _verticalInput, 0);

        Sprint();

        if (!_isSpeedPowerActive && !_isSprinting)
        {
            transform.Translate((_direction * _speed) * Time.deltaTime);
        }
        else if (_isSpeedPowerActive && !_isSprinting)
        {
            transform.Translate((_direction * _boostedSpeed) * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 10.5f)
        {
            transform.position = new Vector3(-10.5f, transform.position.y, 0);
        }

        else if (transform.position.x < -10.5f)
        {
            transform.position = new Vector3(10.5f, transform.position.y, 0);
        }

        void Sprint()
        {
            if (Input.GetKey(KeyCode.LeftShift) && _chargeBar.CanCharge())
            {
                _isSprinting = true;
                transform.Translate((_direction * _sprintSpeed) * Time.deltaTime);
                _chargeBar.IncrementValueBar();
            }
            else
            {
                _isSprinting = false;
                _chargeBar.DecrementValueBar();
            }
        }
    }

    void FireLaser()
    {
        if (_currentAmmo > 0 || _isMinePowerActive)
        {
            _nextFire = Time.time + _fireRate;
            if (_isTripleShotActive && _currentAmmo > 0)
            {
                _isMinePowerActive = false;
                Instantiate(_tripleShootLaser, transform.position, Quaternion.identity);
                _audioSource.clip = _laserClip;
                _audioSource.Play();
                ReduceAmmo();

            }
            else if (_isMinePowerActive)
            {
                _isTripleShotActive = false;
                Instantiate(_minePrefab, transform.position, Quaternion.identity);
                _audioSource.clip = _mineShootClip;
                _audioSource.Play();
            }
            
            else if (_isHomingMissileActive)
            {
                _isHomingMissileActive = false;
                Instantiate(_homingMissile, transform.position + _laserOffset, Quaternion.identity);
            }

            else if (_currentAmmo > 0)
            {
                Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
                _audioSource.clip = _laserClip;
                _audioSource.Play();
                ReduceAmmo();
            }

        }
        else
        {
            _audioSource.PlayOneShot(_noAmmo);
        }

    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            if (_shield.IsShieldBroken())
            {
                _shield.IncrementShieldHits();
                _shield.ShieldManageHits();
            }
            else
            {
                _isShieldActive = false;
                _shieldVisualizer.SetActive(_isShieldActive);
                _shield.ResetShieldHitCounter();
            }
            return;
        }
        StartCoroutine(_shake.Shake(0.20f, 0.65f)); //Camera Shake
        _lives--;

        if (_lives == 2)
        {
            int randomEngine = Random.Range(0, _enginesDamagePrefabs.Length); // List approach for multiple engines damage (using like a parallel - non damage engines and damage engines)
            _enginesDamagePrefabs[randomEngine].SetActive(true);
        }
        else if (_lives == 1)
        {
            foreach (GameObject engine in _enginesDamagePrefabs)
            {
                engine.SetActive(true);
            }
        }

        _uiManager.UpdateLivesImage(_lives);

        if (_lives <= 0)
        {
            _speed = 0;
            Instantiate(_explosion, transform.position, Quaternion.identity);
            _spawnManager.OnPlayerDeath();
            AudioSource.PlayClipAtPoint(_explosionClip, Camera.main.transform.position, 1f);
            Destroy(this.gameObject, 0.2f);
        }
    }

    public void ActivateTripleShoot()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShootDownRoutine());
    }

    IEnumerator TripleShootDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    public void ActivateSpeedPower()
    {
        if (!_isSpeedPowerActive)
        {
            StartCoroutine(SpeedPowerDownRoutine());
        }

    }

    IEnumerator SpeedPowerDownRoutine()
    {
        _isSpeedPowerActive = true;
        float elapsedTime = 0f;
        float startSpeed = _speed;
        float endSpeed = _speed + _speedPowerUp;

        while (elapsedTime < _lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _lerpTime;
            _boostedSpeed = Mathf.Lerp(startSpeed, endSpeed, t);
            yield return null;
        }

        yield return new WaitForSeconds(5f);

        elapsedTime = 0f;
        while (elapsedTime < _lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _lerpTime;
            _boostedSpeed = Mathf.Lerp(endSpeed, startSpeed, t);
            yield return null;
        }

        _isSpeedPowerActive = false;
    }

    public void ActivateShieldPowerUp()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(_isShieldActive);
    }

    public void ActivateMinePower()
    {
        _isMinePowerActive = true;
        StartCoroutine(MinePowerRoutine());
    }

    IEnumerator MinePowerRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isMinePowerActive = false;
    }

    public void AddScore(int pointsToAdd)
    {
        _score += pointsToAdd;
        _uiManager.UpdateScoreText(_score);
    }

    private void ReduceAmmo()
    {
        _currentAmmo--;
        _uiManager.UpdateAmmoText(_currentAmmo);
    }

    public void AddAmmo(int amount)
    {
        int ammoSpace = _maxAmmo - _currentAmmo;
        if (ammoSpace > 0)
        {
            int ammoToAdd = Mathf.Min(amount, ammoSpace);
            _currentAmmo += ammoToAdd;
        }
        _uiManager.UpdateAmmoText(_currentAmmo);
    }

    public void AddLives()
    {
        if (_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLivesImage(_lives);

            foreach (GameObject engine in _enginesDamagePrefabs)
            {
                engine.SetActive(false);
            }

            if (_lives == 2)
            {
                int randomEngine = Random.Range(0, _enginesDamagePrefabs.Length); // List approach for multiple engines damage (using like a parallel - non damage engines and damage engines)
                _enginesDamagePrefabs[randomEngine].SetActive(true);
            }
        }
    }

    public void DisableShoot()
    {
        StartCoroutine(DisableCanShootRoutine());
    }

    private IEnumerator DisableCanShootRoutine()
    {
        _canShoot = false;
        yield return new WaitForSeconds(3f);
        _canShoot = true;
    }

    private void CollectPowerUps()
    {
        //if there is a powerUp and Player Presses C
        //PowerUp should come quickly to the player
        StartCoroutine(MovePowerUPtoPlayerRoutine());
    }

    IEnumerator MovePowerUPtoPlayerRoutine()
    {
        PowerUp powerUpComponent = FindObjectOfType<PowerUp>();

        if (powerUpComponent == null)
        {
            Debug.Log("No PowerUp Found");
            yield break;
        }

        GameObject powerUp = powerUpComponent.gameObject;

        while (powerUp != null && Vector2.Distance(powerUp.transform.position, transform.position) > 0.1f)
        {
            float step = _playerPowerUpSpeed * Time.deltaTime;

            if (powerUp == null)
            {
                yield break;
            }

            powerUp.transform.position = Vector2.MoveTowards(powerUpComponent.transform.position, transform.position, step);
            yield return null;
        }

        if (powerUp != null)
        {
            powerUpComponent.transform.position = transform.position;
        }
    }

    public void ActiveHomingMissile()
    {
        _isHomingMissileActive = true;
    }
}
