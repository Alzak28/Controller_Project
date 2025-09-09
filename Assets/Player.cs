using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f; // kecepatan gerak
    public float jumpforce = 8;
    private Rigidbody rb;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) 
        {
            rb.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
            isGrounded = false; 
        }
    }
    private void FixedUpdate()
    {
        // Input kiri (-1) kanan 9(1)
        float moveX = Input.GetAxis("Horizontal"); // default: A/D atau ? ?

        // Buat arah gerak hanya di sumbu X
        Vector3 movement = new Vector3(moveX, 0f, 0f) * moveSpeed;

        // Tetapkan velocity, tetapi biarkan Y & Z dari rb
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Dead!");

    }
}
