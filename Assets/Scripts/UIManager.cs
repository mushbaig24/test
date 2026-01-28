using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button playButton;

    [Header("Gameplay UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateUI;
            GameManager.Instance.OnGameOver += ShowGameOver;
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnBackToMainMenuClicked);
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        UpdateHighScore();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        ResetMenuUI();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateUI;
            GameManager.Instance.OnGameOver -= ShowGameOver;
        }
    }

    private void UpdateUI(int score, int combo)
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (comboText != null) 
        {
            comboText.text = combo > -1 ? $"Combo x{combo}!" : "";
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        UpdateHighScore();
    }

    private void UpdateHighScore()
    {
        if (highScoreText != null) 
            highScoreText.text = $"High Score: {SaveSystem.GetHighScore()}";
    }

    private void OnRestartClicked()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        GameManager.Instance.StartNewGame();
    }

    private void OnPlayClicked()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        GameManager.Instance.StartNewGame();
    }

    private void OnBackToMainMenuClicked()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        GameManager.Instance.ReturnToMenu();
        ResetMenuUI();
    }

    private void ResetMenuUI()
    {
        if (scoreText != null) scoreText.text = "Score";
        if (comboText != null) comboText.text = "Combo meter";
    }
}
