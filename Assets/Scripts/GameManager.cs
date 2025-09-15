using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public TMP_Text scoreText;      
    public GameObject gameOverPanel; 

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
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        IsGameOver = false;
        Score = 0;
        UpdateScoreUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
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

        Debug.Log("GAME OVER");
    }

    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = Score.ToString();
    }

    // Optional: quick restart
    void Update()
    {
        if (IsGameOver && Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
