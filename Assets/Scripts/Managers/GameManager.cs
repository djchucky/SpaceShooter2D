using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    [SerializeField] private GameObject _winTextObject;
    [SerializeField] private GameObject _escapeTextObject;
    [SerializeField] private int _waveBoss = 5;
    [SerializeField] private int _waveNewEnemies = 3;
    [SerializeField] private AudioClip _bossClip;
    [SerializeField] private AudioSource _audioSource;

    Enemy[] currentStandardEnemies;
    EnemyRed[] currentRedEnemies;


    [Header("Wave System Settings")]
    private int _currentWave = 0;
    private int _enemyIndex = 0;
    [SerializeField] private int _enemyWaveMultiplier = 3;
    [SerializeField] GameObject _enemyBoss;

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UIManager is null on GameManager");
        }
    }

    void Update()
    {
        if (_isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    //When the game starts the first wave should begin
    //When the wave begins spawn enemies
    //Keep wave active untill all enemies are dead
    //When all enemies die end wave and increase wave

    public int GetCurrentWave()
    {
        return _currentWave;
    }

    public void IncreaseWave()
    {
        _currentWave++;
        StartCoroutine(_uiManager.WaveUpdateText(_currentWave));
    }

    public IEnumerator StartWave(GameObject[] enemyPrefab, GameObject enemyContainer)
    {
        yield return new WaitForSeconds(0.25f);
        IncreaseWave();
        yield return new WaitForSeconds(1.5f);

        int enemiesToSpawn = _currentWave * _enemyWaveMultiplier;
        //_isWaveCompleted = false;

        if (_currentWave == _waveBoss)
        {
            _audioSource.clip = _bossClip;
            _audioSource.Play();
            Instantiate(_enemyBoss, new Vector3(0, 16f, 0), Quaternion.identity);
            yield break;
        }


        //Check number of Wave
        //if Wave > 3 the index is between 0 and max lenght,which includes both types of enemies
        if (GetCurrentWave() >= _waveNewEnemies)
        {
            _enemyIndex = enemyPrefab.Length;
        }
        else
        {
            _enemyIndex = 0;
        }

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int randomShield = Random.Range(0, 2);
            Vector3 _posToSpawn = new Vector3(Random.Range(-8f, 8f), 8.5f, 0);
            GameObject enemyClone = Instantiate(enemyPrefab[Random.Range(0, _enemyIndex)], _posToSpawn, Quaternion.identity);
            enemyClone.transform.SetParent(enemyContainer.transform);
            if (enemyClone.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (randomShield == 1 && _currentWave >= 3)
                {
                    enemy.ActiveEnemyShield();
                }
            }
            yield return new WaitForSeconds(Random.Range(1, 3.5f));
        }
        StartCoroutine(CheckIfWaveCompleted(enemyPrefab, enemyContainer));
    }

    private IEnumerator CheckIfWaveCompleted(GameObject[] _enemyPrefab, GameObject _enemyContainer)
    {
        while (!_isGameOver)
        {
            currentStandardEnemies = FindObjectsOfType<Enemy>();
            currentRedEnemies = FindObjectsOfType<EnemyRed>();

            if (currentStandardEnemies.Length == 0 && currentRedEnemies.Length == 0)
            {
                Debug.Log("Wave completed!");
                yield return new WaitForSeconds(3f); // Wait 3 seconds before launching the new routine
                StartCoroutine(StartWave(_enemyPrefab, _enemyContainer));
                yield break; // Ends Coroutine
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void BossDeath()
    {
        _winTextObject.SetActive(true);
        _escapeTextObject.SetActive(true);
        _spawnManager.OnBossDeath();
    }


}
