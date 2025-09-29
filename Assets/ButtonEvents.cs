using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonEvents : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;   // assign di Inspector
    [SerializeField] private AudioClip clickSound;      // assign di Inspector

    // Fungsi umum untuk play sound + jalankan action
    private IEnumerator PlaySoundAndExecute(System.Action action)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            yield return new WaitForSeconds(clickSound.length);
        }

        action?.Invoke();
    }

    // Retry scene aktif
    public void RetryGame()
    {
        StartCoroutine(PlaySoundAndExecute(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }));
    }

    // Start game (load scene berdasarkan nama)
    public void StartGame(string sceneName)
    {
        StartCoroutine(PlaySoundAndExecute(() =>
        {
            SceneManager.LoadScene(sceneName);
        }));
    }

    // Exit game
    public void ExitGame()
    {
        StartCoroutine(PlaySoundAndExecute(() =>
        {

            Application.Quit();

        }));
    }
}
