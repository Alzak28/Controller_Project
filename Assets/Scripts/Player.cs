using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public Transform[] lanes;
    public int currentLane = 1;
    public float transitionSpeed = 5f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private Rigidbody rb;
    private bool isGrounded;

    [Header("Sensor Input")]
    public ArduinoController sensor; // drag ArduinoController GameObject
    [Tooltip("Threshold tilt (absolute) untuk pindah lane. X adalah kemiringan kiri/kanan (g).")]
    public float tiltThreshold = 0.35f;
    [Tooltip("Hysteresis untuk mencegah toggle (nilai > 0).")]
    public float tiltHysteresis = 0.08f;
    [Tooltip("Cooldown (detik) antara perpindahan lane")]
    public float laneCooldown = 0.5f;

    float lastMoveTime = -10f;
    int lastRequestedLane = -1;

    void Start()
    {
        targetPosition = lanes[currentLane].position;
        rb = GetComponent<Rigidbody>();
        gameObject.tag = "Player";
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        // sensor-based control (priority terhadap keyboard optional)
        if (!isMoving && sensor != null)
        {
            float sx = sensor.X;
            bool canMove = Time.time - lastMoveTime >= laneCooldown;

            if (canMove)
            {
                if (sx > tiltThreshold && currentLane < lanes.Length - 1)
                {
                    StartCoroutine(MoveToLane(currentLane + 1));
                    lastMoveTime = Time.time;
                    lastRequestedLane = currentLane + 1;
                }
                else if (sx < -tiltThreshold && currentLane > 0)
                {
                    StartCoroutine(MoveToLane(currentLane - 1));
                    lastMoveTime = Time.time;
                    lastRequestedLane = currentLane - 1;
                }
            }
        }

        // fallback: keyboard controls (optional)
        if (!isMoving)
        {
            if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && currentLane > 0)
            {
                StartCoroutine(MoveToLane(currentLane - 1));
            }
            if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && currentLane < lanes.Length - 1)
            {
                StartCoroutine(MoveToLane(currentLane + 1));
            }
        }
    }

    private IEnumerator MoveToLane(int newLane)
    {
        isMoving = true;
        currentLane = newLane;
        targetPosition = lanes[currentLane].position;

        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;

        while (timeElapsed < 1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed);
            timeElapsed += Time.deltaTime * transitionSpeed;
            yield return null;
        }

        transform.position = targetPosition; // Pastikan posisi tepat di target
        isMoving = false;
    }

    // Collider biasa untuk ground & obstacle (OnCollisionEnter tetap dipakai)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    // Trigger untuk score/collectible
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ScoreTrigger"))
        {
            // Tambah score lewat GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1);
                // opsional: panggil efek suara / partikel lewat GameManager
                GameManager.Instance.PlayFX("collect"); // pastikan ada fx bernama "collect" jika dipakai
            }

            // Hapus item agar tidak double-count
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Obstacle")) // kalau obstacle menggunakan trigger
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Dead!");
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}
