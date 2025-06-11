using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour
{
    Slider _slider;

    [SerializeField] private float _chargeBarValue;
    [SerializeField] private float _incrementSpeed = 1f;
    [SerializeField] private float _decrementSpeed = 1f;
    [SerializeField] private float _coolDownWaitSeconds = 2f;

    private bool _isCooldownComplete = true;
    private float _timeElapsed = 0f;
    private bool _cooldownStarted = false;

    void Start()
    {
        _slider = GetComponent<Slider>();
    }

    void Update()
    {
        _chargeBarValue = Mathf.Clamp(_chargeBarValue, _slider.minValue, _slider.maxValue);
    }

    public void IncrementValueBar()
    {
        if (CanCharge())
        {
            _chargeBarValue += Time.deltaTime * _incrementSpeed;
            _slider.value = _chargeBarValue;

            // Reset cooldown (while running)
            _cooldownStarted = false;
            _timeElapsed = 0f;
        }

        // if value reaches max the we cannot charge
        if (_chargeBarValue >= _slider.maxValue)
        {
            _isCooldownComplete = false;
        }
    }

    public void DecrementValueBar()
    {
        // Start cooldown
        if (!_cooldownStarted)
        {
            _isCooldownComplete = false; //can't reun until cooldown is not complete
            _cooldownStarted = true;
            _timeElapsed = 0f;
        }

        // Wait for Cooldown
        if (!_isCooldownComplete)
        {
            _timeElapsed += Time.deltaTime;

            if (_timeElapsed >= _coolDownWaitSeconds)
            {
                _isCooldownComplete = true;
            }
        }

        // Start decreasing the bar after cooldown is complete
        if (_isCooldownComplete && _chargeBarValue > 0f)
        {
            _chargeBarValue -= Time.deltaTime * _decrementSpeed;
            _slider.value = _chargeBarValue;
        }
    }

    public bool CanCharge()
    {
        return _chargeBarValue < _slider.maxValue && _isCooldownComplete;
    }
}
