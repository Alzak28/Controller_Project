using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public List<GameObject> obstaclePrefabs;
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public float spawnRangeXOffset = 0f;

    private float timer;
    private bool readyToSpawn = true; // Menggantikan canSpawn, lebih jelas namanya

    void OnEnable()
    {
        // Berlangganan event saat spawner aktif
        ObstacleMove.OnObstacleReachedEnd += OnPreviousObstacleCleared;
    }

    void OnDisable()
    {
        // Berhenti berlangganan event saat spawner tidak aktif
        ObstacleMove.OnObstacleReachedEnd -= OnPreviousObstacleCleared;
    }

    void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point is not assigned! Please assign a Transform to 'spawnPoint'.");
            enabled = false;
            return;
        }
        if (obstaclePrefabs == null || obstaclePrefabs.Count == 0)
        {
            Debug.LogWarning("Obstacle Prefabs list is empty! Please assign some prefabs.");
            enabled = false;
            return;
        }

        timer = spawnInterval;
    }

    void Update()
    {
        // Hanya hitung mundur timer jika siap dan belum ada obstacle yang di-spawn
        if (readyToSpawn)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                SpawnObstacle();
                timer = spawnInterval;
                readyToSpawn = false; // Set menjadi false setelah spawn
            }
        }
    }


    // void SpawnObstacle()
    // {
    //     int randomIndex = UnityEngine.Random.Range(0, obstaclePrefabs.Count);
    //     GameObject selectedPrefab = obstaclePrefabs[randomIndex];

    //     float finalSpawnX = spawnPoint.position.x +
    //                         UnityEngine.Random.Range(-spawnRangeXOffset, spawnRangeXOffset);
    //     Vector3 spawnPosition = new Vector3(finalSpawnX, spawnPoint.position.y, spawnPoint.position.z);

    //     Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    // }

    void SpawnObstacle()
    {
        int randomIndex = Random.Range(0, obstaclePrefabs.Count);
        GameObject selectedPrefab = obstaclePrefabs[randomIndex];

        float finalSpawnX = spawnPoint.position.x + Random.Range(-spawnRangeXOffset, spawnRangeXOffset);
        Vector3 spawnPosition = new Vector3(finalSpawnX, spawnPoint.position.y, spawnPoint.position.z);

        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }

    // Method ini akan dipanggil ketika obstacle sebelumnya mencapai endZ
    void OnPreviousObstacleCleared()
    {
        readyToSpawn = true; // Izinkan spawner untuk spawn obstacle berikutnya
    }
}