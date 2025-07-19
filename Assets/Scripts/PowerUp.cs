using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private int _powerUpID = 0;

    [SerializeField] private AudioClip _pickupClip;
    [SerializeField] private AudioClip _explosionClip;
    [SerializeField] private GameObject _explosionPrefab;


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
                switch (_powerUpID)
                {
                    case 0:
                        player.ActivateTripleShoot();
                        break;

                    case 1:
                        player.ActivateSpeedPower();
                        break;

                    case 2:
                        player.ActivateShieldPowerUp();
                        break;

                    case 3:
                        player.ActivateMinePower();
                        break;

                    case 4:
                        player.DisableShoot();
                        break;

                    case 5:
                        player.ActiveHomingMissile();
                        break;

                    default:
                        break;
                }

                AudioSource.PlayClipAtPoint(_pickupClip, Camera.main.transform.position, 1f); // Audiosource.PLay()
            }

            Destroy(this.gameObject);
        }
    }

    public void DestroyPowerUP()
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(_explosionClip, Camera.main.transform.position);
        Destroy(this.gameObject, 0.5f);
    }

}
