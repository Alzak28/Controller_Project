/*using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerJ : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpforce = 8f;

    private Rigidbody rb;
    private bool isGrounded;
    private JoystickInputHandler joystickInputHandler; // Tambahkan referensi ini

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // pastikan Player bertag "Player"
        gameObject.tag = "Player";

        // Cari JoystickInputHandler di scene
        joystickInputHandler = FindObjectOfType<JoystickInputHandler>();
        if (joystickInputHandler == null)
        {
            Debug.LogError("JoystickInputHandler not found in the scene! Please add it to a GameObject.");
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        // Jika kamu ingin menambahkan lompat dengan tombol joystick, tambahkan di sini juga
        // Misalnya: inputActions.Player.Jump.performed += ctx => Jump();
        // Untuk saat ini, kita biarkan Spacebar saja atau tambahkan ke Input Actions
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        float moveX = 0f;
        if (joystickInputHandler != null)
        {
            moveX = joystickInputHandler.GetMoveXInput(); // Ambil input dari joystick
        }
        else
        {
            moveX = Input.GetAxis("Horizontal"); // Fallback ke keyboard jika joystick handler tidak ditemukan
        }

        // Gerak hanya di X; pertahankan Y dan Z rigidbody
        rb.linearVelocity = new Vector3(moveX * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
    }

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Die();
        }

        if (other.CompareTag("ScoreTrigger"))
        {
            // Panggil method AddScore dari GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore();
            }
            Debug.Log("Score nambah 1");
            // Optional: Hancurkan atau nonaktifkan trigger agar tidak double score
            Destroy(other.gameObject);
        }
    }

    void Die()
    {
        Debug.Log("Player Dead!");
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}*/

using UnityEngine;
using System.Collections; // <-- Tambahkan ini
// using UnityEngine.InputSystem; // Tidak perlu di sini jika input di handle JoystickInputHandler

[RequireComponent(typeof(Rigidbody))]
public class PlayerJ : MonoBehaviour
{
    public Transform[] lanes;       // Array untuk menyimpan posisi jalur
    public int currentLane = 1;     // Jalur saat ini (sesuaikan dengan jumlah jalur Anda, biasanya dimulai dari 0 atau 1)
    public float transitionSpeed = 5f; // Kecepatan transisi antar jalur

    // public float moveSpeed = 5f; // Hapus atau biarkan jika masih digunakan untuk kecepatan maju
    public float jumpforce = 8f;

    private Rigidbody rb;
    private bool isGrounded;
    private JoystickInputHandler joystickInputHandler; // Referensi ke JoystickInputHandler

    private Vector3 targetPosition; // Posisi target saat bergerak antar jalur
    private bool isMoving = false;  // Flag untuk mencegah input ganda saat bergerak

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.tag = "Player";

        joystickInputHandler = FindObjectOfType<JoystickInputHandler>();
        if (joystickInputHandler == null)
        {
            Debug.LogError("JoystickInputHandler not found in the scene! Please add it to a GameObject.");
        }

        // --- Inisialisasi posisi awal di jalur ---
        if (lanes.Length > 0 && currentLane >= 0 && currentLane < lanes.Length)
        {
            targetPosition = lanes[currentLane].position;
            transform.position = targetPosition; // Langsung set posisi awal pemain
        }
        else
        {
            Debug.LogError("Lanes array is not set up correctly or currentLane is out of bounds!");
            enabled = false; // Nonaktifkan script jika ada masalah
        }
    }
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        // Input Lompat (biarkan atau integrasikan ke Input System)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Pergerakan antar jalur akan dipanggil dari JoystickInputHandler
        // Tidak ada lagi Input.GetAxis("Horizontal") di sini
    }

    void FixedUpdate()
    {
        // Tetap terapkan gravitasi atau kecepatan maju jika ada, tapi jangan gerakkan horizontal di sini
        // Jika Anda ingin pemain bergerak maju secara otomatis, lakukan di sini
        // rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, GameManager.Instance.forwardSpeed);
    }
    // Method ini akan dipanggil dari JoystickInputHandler
    public void MoveLane(int direction) // direction bisa +1 (kanan) atau -1 (kiri)
    {
        if (isMoving) return; // Jangan bergerak jika sudah dalam transisi

        int newLane = currentLane + direction;

        if (newLane >= 0 && newLane < lanes.Length)
        {
            StartCoroutine(MoveToLaneCoroutine(newLane));
        }
        else
        {
            Debug.Log("Cannot move further in that direction. Current lane: " + currentLane + ", Attempted new lane: " + newLane);
        }
    }

    private IEnumerator MoveToLaneCoroutine(int newLane)
    {
        isMoving = true;
        currentLane = newLane;
        targetPosition = lanes[currentLane].position;

        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;

        while (timeElapsed < 1f)
        {
            // Hanya lerp posisi X, biarkan Y dan Z tidak berubah relatif terhadap jalur
            Vector3 currentLerpPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed);
            transform.position = new Vector3(currentLerpPosition.x, startPosition.y, startPosition.z); // Pertahankan Y dan Z awal

            timeElapsed += Time.deltaTime * transitionSpeed;
            yield return null;
        }

        transform.position = new Vector3(targetPosition.x, startPosition.y, startPosition.z); // Pastikan X tepat di target, Y dan Z tetap
        isMoving = false;
    }
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Die();
        }

        if (other.CompareTag("ScoreTrigger"))
        {
            // Panggil method AddScore dari GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore();
            }
            Debug.Log("Score nambah 1");
            // Optional: Hancurkan atau nonaktifkan trigger agar tidak double score
            Destroy(other.gameObject);
        }
    }

    void Die()
    {
        Debug.Log("Player Dead!");
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}