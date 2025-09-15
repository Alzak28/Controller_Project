using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Score : MonoBehaviour
{
    private bool consumed;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (consumed) return;
        if (!other.CompareTag("Player")) return;

        // Jika sudah Game Over, abaikan (waktu bisa saja ter-pause)
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        GameManager.Instance?.AddScore(1);
        consumed = true;
        gameObject.SetActive(false);
    }
}
