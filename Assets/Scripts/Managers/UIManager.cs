using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private Image _livesImage;
    [SerializeField] private GameObject _gameOverTextObject;
    [SerializeField] private GameObject _restartTextObject;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private TMP_Text _waveText;

    void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_scoreText == null)
        {
            Debug.LogError("Score text is null on UI Manager");
        }

        if (_livesImage == null)
        {
            Debug.LogError("LivesImage is NULL on UIManager");
        }

        if (_gameOverTextObject == null)
        {
            Debug.LogError("GameOver object is NULL on UIManager");
        }

        if (_restartTextObject == null)
        {
            Debug.LogError("Restart Text object is NULL on UIManager");
        }

        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL on UI Manager");
        }

        _scoreText.text = "Score: " + 0;
        _livesImage.sprite = _livesSprites[3];
        _gameOverTextObject.SetActive(false);
        _restartTextObject.SetActive(false);

    }


    public void UpdateScoreText(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateAmmoText(int playerAmmo)
    {
        if (playerAmmo > 0)
        {
            _ammoText.text = "Ammo " + playerAmmo;
        }
        else
        {
            _ammoText.text = "No Ammo";
        }
    }

    public void UpdateLivesImage(int playerLives)
    {
        if (playerLives < 0) //Check if we are not damaged by multiple lasers and go out of array
            return;

        _livesImage.sprite = _livesSprites[playerLives];
        if (playerLives == 0)
        {
            GameOverSequence();
        }
    }

    private void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverTextObject.SetActive(true);
        _restartTextObject.SetActive(true);
        StartCoroutine(GameOverFlickeringRoutine());
        StartCoroutine(RestartTextFlickeringRoutine());
    }

    IEnumerator GameOverFlickeringRoutine()
    {
        while (true)
        {
            _gameOverTextObject.SetActive(false);
            yield return new WaitForSeconds(0.15f);
            _gameOverTextObject.SetActive(true);
            yield return new WaitForSeconds(0.15f);
        }

    }

    IEnumerator RestartTextFlickeringRoutine()
    {
        while (true)
        {
            _restartTextObject.SetActive(false);
            yield return new WaitForSeconds(0.15f);
            _restartTextObject.SetActive(true);
            yield return new WaitForSeconds(0.15f);
        }
    }

    public IEnumerator WaveUpdateText(int waveNumber)
    {
        _waveText.text = $"Wave: {waveNumber}";
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
    }

}
