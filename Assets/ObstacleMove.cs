using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    public float speed = 5f;     // kecepatan maju
    public float resetZ = 20f;   // posisi awal Z
    public float endZ = -20f;    // posisi batas akhir Z

    void Update()
    {
        // Gerakkan obstacle ke arah -Z
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        // Kalau sudah melewati batas endZ, kembalikan ke posisi awal
        if (transform.position.z < endZ)
        {
            Vector3 newPos = transform.position;
            newPos.z = resetZ;
            transform.position = newPos;
        }
    }
}
