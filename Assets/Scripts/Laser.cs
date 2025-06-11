using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private bool _isEnemyLaser;
    public bool IsEnemyLaser {get {return _isEnemyLaser;}}

    [SerializeField] private int _enemyType = 0;

    void Update()
    {
        if (_isEnemyLaser == false)
        {
            MoveUp();  // Move up for player's laser
        }
        else if (_isEnemyLaser == true && _enemyType == 0)
        {
            MoveDown();  // Move down for enemy's laser
        }
    }

    private void MoveUp()
    {
        transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime,Space.Self);
        if (transform.position.y > 8f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime,Space.Self);

        if (transform.position.y < -8f || transform.position.x < -18f || transform.position.x > 18f || transform.position.y > 8f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;  // Set the flag to true when it's an enemy's laser
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && _isEnemyLaser)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
        }

        else if (other.gameObject.TryGetComponent<PowerUp>(out PowerUp powerUp) && _isEnemyLaser)
        {
            powerUp.DestroyPowerUP();
        }
    }
}
