using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private int _maxShieldHits = 3;
    private int _shieldCurrentHits = 0;
    private Color _baseColor;

    [SerializeField] private bool _isEnemyShield = false;

    [Header("Colors")]
    [SerializeField] Color[] _shieldHitColors;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("Sprite renderer is null on Shield");
        }
        _baseColor = _spriteRenderer.color;
    }

    void OnEnable()
    {
        ResetShieldHitCounter();
    }

    void Start()
    {

    }

    public void ShieldManageHits()
    {
        if (_shieldCurrentHits < _shieldHitColors.Length && !_isEnemyShield)
        {
            _spriteRenderer.color = _shieldHitColors[_shieldCurrentHits];
        }

        else if (_isEnemyShield)
        {
            Debug.Log("Enemy Shield");
        }
    }

    public void IncrementShieldHits()
    {
        _shieldCurrentHits++;
    }

    public void ResetShieldHitCounter()
    {
        if (!_isEnemyShield)
        {
            _spriteRenderer.color = _baseColor;
            _shieldCurrentHits = 0;
        }
    }

    public bool IsShieldBroken()
    {
        return _shieldCurrentHits <= _maxShieldHits;
    }

}
