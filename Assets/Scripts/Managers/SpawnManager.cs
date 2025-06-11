using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpData
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float spawnProbability;
    }

    private GameManager _gameManager;

    [Header("Enemy Section")]
    [SerializeField] GameObject[] _enemyPrefab;
    [SerializeField] GameObject _enemyContainer;

    [Header("PowerUps")]
    [SerializeField] PowerUpData[] _powerUpData;

    [Header("Collectables")]
    [SerializeField] private float _commonSpawnChance = 0.75f;
    [SerializeField] private float _rareSpawnChance = 0.25f;
    [SerializeField] private GameObject[] _collectables;

    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is null on SpawnManager");
        }
    }

    private bool _stopSpawning = false;

    public void StartSpawning() // Called on Asteroid class
    {
        StartCoroutine(_gameManager.StartWave(_enemyPrefab, _enemyContainer));
        //StartCoroutine(SpawnEnemiesRoutine()); // Calling new routine on GameManager
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(SpawnCollectablesRoutine());
    }

    /*IEnumerator SpawnEnemiesRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (!_stopSpawning)
        {
            Vector3 _posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0);
            GameObject _cloneEnemy = Instantiate(_enemyPrefab, _posToSpawn, Quaternion.identity);
            _cloneEnemy.transform.SetParent(_enemyContainer.transform);
            yield return new WaitForSeconds(5f);
        }
    }*/

    IEnumerator SpawnPowerUpRoutine()
    {
        while (!_stopSpawning)
        {
            int randomSpawnTime = Random.Range(5, 12);
            Vector3 _posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0);
            yield return new WaitForSeconds(randomSpawnTime);
            GameObject selectedPowerUp = GetRandomPowerUp();
            Instantiate(selectedPowerUp, _posToSpawn, Quaternion.identity);
        }
    }

    GameObject GetRandomPowerUp()
    {
        float total = 0f;
        foreach (var p in _powerUpData)
            total += p.spawnProbability;

        float randomPoint = Random.value * total;
        float cumulative = 0f;

        foreach (var p in _powerUpData)
        {
            cumulative += p.spawnProbability;
            if (randomPoint <= cumulative)
                return p.prefab;
        }

        return _powerUpData[0].prefab; // Fallback
    }

    IEnumerator SpawnCollectablesRoutine()
    {
        while (!_stopSpawning)
        {
            float randomNumber = Random.value;
            int randomSpawnTime = Random.Range(10, 20);
            int randomCollectable = Random.Range(0, _collectables.Length);
            Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 7f, 0);
            yield return new WaitForSeconds(randomSpawnTime);

            if (randomNumber <= _commonSpawnChance)
            {
                Instantiate(_collectables[0], spawnPos, Quaternion.identity);
            }
            else if (randomNumber <= (_commonSpawnChance + _rareSpawnChance))
            {
                Instantiate(_collectables[1], spawnPos, Quaternion.identity);

            }
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void OnBossDeath()
    {
        _stopSpawning = true;
    }
}
