using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GridManager gridManager;
    [SerializeField] private List<CardData> cardDataList;
    [SerializeField] private Sprite cardBack;
    
    [Header("Game Config")]
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 4;
    
    private List<Card> allCards = new List<Card>();
    private List<Card> pendingComparison = new List<Card>();
    
    private int score = 0;
    private int combo = 0;
    private int matchesFound = 0;
    private bool isGameOver = false;

    public event Action<int, int> OnScoreChanged; // Score, Combo
    public event Action OnGameOver;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Load settings or layout if saved
        LoadProgress();
        // Game will be started via Main Menu Play button
    }

    public void StartNewGame()
    {
        StopAllCoroutines();
        isGameOver = false;
        score = 0;
        combo = 0;
        matchesFound = 0;
        AudioManager.Instance?.sfxSource.Stop();
        pendingComparison.Clear();
        
        OnScoreChanged?.Invoke(score, combo);

        allCards = gridManager.SpawnCards(rows, columns, cardDataList, cardBack);
        
        foreach (var card in allCards)
        {
            card.OnCardClicked += HandleCardClicked;
        }
    }

    private void HandleCardClicked(Card card)
    {
        if (isGameOver) return;
        
        // Continuous flipping: flip immediately
        card.Flip(true);
        AudioManager.Instance?.PlayFlip();
        
        pendingComparison.Add(card);

        if (pendingComparison.Count >= 2)
        {
            Card c1 = pendingComparison[0];
            Card c2 = pendingComparison[1];
            pendingComparison.RemoveAt(0);
            pendingComparison.RemoveAt(0);
            
            StartCoroutine(CompareCards(c1, c2));
        }
    }

    private IEnumerator CompareCards(Card c1, Card c2)
    {
        // Short delay to let the second card finish its initial flip phase if needed
        // but not strictly necessary for "continuous" feel since we already started the flip.
        
        if (c1.Id == c2.Id)
        {
            // Match
            yield return new WaitForSeconds(0.3f);
            c1.SetMatched();
            c2.SetMatched();
            matchesFound++;
            combo++;
            score += 100 * combo;
            
            AudioManager.Instance?.PlayMatch(combo);
            OnScoreChanged?.Invoke(score, combo);
            
            if (CheckGameOver())
            {
                EndGame();
            }
        }
        else
        {
            // Mismatch
            yield return new WaitForSeconds(1.0f);
            c1.Flip(false);
            c2.Flip(false);
            combo = 0;
            
            AudioManager.Instance?.PlayMismatch();
            OnScoreChanged?.Invoke(score, combo);
        }
    }

    private bool CheckGameOver()
    {
        return matchesFound >= (rows * columns) / 2;
    }

    private void EndGame()
    {
        isGameOver = true;
        AudioManager.Instance?.PlayGameOver();
        SaveProgress();
        OnGameOver?.Invoke();
    }

    private void SaveProgress()
    {
        SaveSystem.SaveScore(score);
    }

    private void LoadProgress()
    {
        // Could load high score or last layout
    }

    // Public methods for UI to change layout
    public void SetLayout(int r, int c)
    {
        rows = r;
        columns = c;
    }

    public void ReturnToMenu()
    {
        StopAllCoroutines();
        isGameOver = false;
        score = 0;
        combo = 0;
        matchesFound = 0;
        OnScoreChanged?.Invoke(score, combo);
        gridManager.ClearGrid();
        AudioManager.Instance?.sfxSource.Stop();
    }
}
