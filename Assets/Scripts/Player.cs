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
    // public float moveSpeed = 5f;
    // public float jumpforce = 8f;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        targetPosition = lanes[currentLane].position;

        rb = GetComponent<Rigidbody>();
        // pastikan Player bertag "Player"
        gameObject.tag = "Player";
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 0)
            {
                StartCoroutine(MoveToLane(currentLane - 1));
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) && currentLane < lanes.Length - 1)
            {
                StartCoroutine(MoveToLane(currentLane + 1));
            }
        }

        // if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        // {
        //     rb.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
        //     isGrounded = false;
        // }

            // MovePlayer();
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

    // void MovePlayer()
    // {
    //     if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 0)
    //     {
    //         currentLane--;
    //     }
    //     else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) && currentLane < lanes.Length - 1)
    //     {
    //         currentLane++;
    //     }

    //     Vector3 targetPosition = lanes[currentLane].position;
    //     transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    //     //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    // }

    // void FixedUpdate()
    // {
    //     if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

    //     float moveX = Input.GetAxis("Horizontal"); // A/D atau ← →
    //     // Gerak hanya di X; pertahankan Y dan Z rigidbody
    //     rb.linearVelocity = new Vector3(moveX * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
    // }

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

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Obstacle"))
    //     {
    //         Die();
    //     }

    //     if (other.CompareTag("ScoreTrigger"))
    //     {
    //         // Panggil method AddScore dari GameManager
    //         if (GameManager.Instance != null)
    //         {
    //             GameManager.Instance.AddScore();
    //         }
    //         Debug.Log("Score nambah 1");
    //         // Optional: Hancurkan atau nonaktifkan trigger agar tidak double score
    //         Destroy(other.gameObject);
    //     }
    // }

    void Die()
    {
        Debug.Log("Player Dead!");
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}
