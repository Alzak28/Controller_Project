using UnityEngine;
using System; // Untuk Action

public class ObstacleMove : MonoBehaviour
{
    public float speed = 5f;
    public float resetZ = 20f;
    public float endZ = -20f;

    // Event yang akan dipanggil ketika obstacle mencapai endZ
    public static event Action OnObstacleReachedEnd;

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        if (transform.position.z < endZ)
        {
            // Panggil event sebelum reset/destroy
            if (OnObstacleReachedEnd != null)
            {
                OnObstacleReachedEnd();
            }

            // Untuk game endless runner, biasanya dihancurkan atau di-pool
            Destroy(gameObject); // Atau kembalikan ke object pool
            // Atau seperti sebelumnya:
            // Vector3 newPos = transform.position;
            // newPos.z = resetZ;
            // transform.position = newPos;
        }
    }
}