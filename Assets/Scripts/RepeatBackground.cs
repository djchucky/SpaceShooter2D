using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    private float _height;

    void Start()
    {
        _height = GetComponent<BoxCollider2D>().size.y;
    }

    void Update()
    {
        transform.Translate(Vector2.down * 5f * Time.deltaTime);

        if (transform.position.y < -_height)
        {
            Reposition();
        }
    }

    void Reposition()
    {
        Vector2 offset = new Vector2(0, _height * 2f);
        transform.position = (Vector2)transform.position + offset;
    }
}
