using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text comboText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;

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

        UpdateHighScore();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (comboText != null) comboText.text = "";
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
            comboText.text = combo > 1 ? $"Combo x{combo}!" : "";
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
}
