using UnityEngine;

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
}