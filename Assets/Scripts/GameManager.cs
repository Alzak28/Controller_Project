using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnGameRestarted;

    [Header("UI dan Button")]
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public TMP_Text FinalScore;
    public GameObject buttonRestart;

    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        IsGameOver = false;
        Score = 0;
        UpdateScoreUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
        OnGameRestarted?.Invoke();
    }

    public void AddScore(int amount = 1)
    {
        if (IsGameOver) return;
        Score += amount;
        UpdateScoreUI();
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        // Freeze gameplay
        Time.timeScale = 0f;

        // Show panel
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (FinalScore) FinalScore.text = "Final Score: " + Score.ToString();

        Debug.Log("GAME OVER");
    }

    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = "Score: " + Score.ToString();
    }

    // Optional: quick restart
    void Update()
    {
        if (IsGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    public void RestartGame()
    {
        Debug.Log("Restart");
        //Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
