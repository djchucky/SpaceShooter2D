using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private SpawnManager _spawnManager;

    [SerializeField] private float _rotationSpeed = 8f;
    [SerializeField] private GameObject _explosionPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip _explosionClip;

    

    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if(_spawnManager == null)
        {
            Debug.LogError("Spawn manager is NULL on Asteroid");
        }

    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Laser")
        {
            Instantiate(_explosionPrefab,transform.position,Quaternion.identity);
            Destroy(other.gameObject);
            _spawnManager.StartSpawning();
            AudioSource.PlayClipAtPoint(_explosionClip,Camera.main.transform.position ,1f);
            Destroy(this.gameObject,0.5f);
        }
    }
}
