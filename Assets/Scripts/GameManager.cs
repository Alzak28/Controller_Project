/*using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnGameRestarted;
        public GameObject[] posePrefabs;  // Array untuk 4 pose prefab

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
*/
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnGameRestarted;
    public GameObject[] posePrefabs;

    [Header("UI dan Button")]
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public TMP_Text FinalScore;
    public GameObject buttonRestart;

    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public GameObject startButton;

    [Header("Music Manager")]
    public AudioClip[] bgMusics; // Array untuk background music
    public string[] bgMusicNames; // Nama-nama bg music, urutannya sama dengan array di atas
    public AudioClip[] fxMusics; // Array untuk sound effect
    public string[] fxMusicNames; // Nama-nama fx music

    // Di dalam GameManager
    public bool IsGameStarted => isGameStarted;


    private AudioSource audioSource;

    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }
    private bool isGameStarted = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
    }

    void Start()
    {
        Time.timeScale = 1f;
        IsGameOver = false;
        Score = 0;
        UpdateScoreUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // Tampilkan main menu di awal
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (startButton) startButton.SetActive(true);

        if (gameOverPanel) gameOverPanel.SetActive(false);
        OnGameRestarted?.Invoke();
    }

    void Update()
    {
        // Main menu: tekan W atau panah atas untuk mulai
        if (!isGameStarted && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            StartGame();
        }

        // Restart game jika game over
        if (IsGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void StartGame()
    {
        isGameStarted = true;
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (startButton) startButton.SetActive(false);

        PlayBGMusic("musikgamebg");
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

        // Ganti musik ke kalah
        PlayBGMusic("kalahbg");

        Debug.Log("GAME OVER");
    }

    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = "Score: " + Score.ToString();
    }

    public void RestartGame()
    {
        Debug.Log("Restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // MUSIC MANAGER SECTION
    public void PlayBGMusic(string musicName)
    {
        int idx = Array.IndexOf(bgMusicNames, musicName);
        if (idx >= 0 && idx < bgMusics.Length)
        {
            audioSource.Stop();
            audioSource.clip = bgMusics[idx];
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void PlayFX(string fxName)
    {
        int idx = Array.IndexOf(fxMusicNames, fxName);
        if (idx >= 0 && idx < fxMusics.Length)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = fxMusics[idx];
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
