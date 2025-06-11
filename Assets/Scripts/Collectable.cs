using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private int _collectableID = 0;
    [SerializeField] private int _amount = 30;

    [Header("Audios")]
    [SerializeField] private AudioClip _pickupClip;

    void Start()
    {

    }

    void Update()
    {
        transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
        if (transform.position.y < -6f)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch (_collectableID)
                {
                    case 0:
                        player.AddAmmo(_amount);
                        break;

                    case 1:
                        player.AddLives();
                        break;

                    default:
                        break;
                }
            }
                AudioSource.PlayClipAtPoint(_pickupClip, Camera.main.transform.position, 1f);
                Destroy(this.gameObject);
        }
    }
}
