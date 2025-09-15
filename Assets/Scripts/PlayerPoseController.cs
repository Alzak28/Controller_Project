using System;
using UnityEngine;

public class PlayerPoseController : MonoBehaviour
{
    public GameObject[] posePrefabs;  // Array untuk 4 pose prefab
    private GameObject currentPoseInstance;
    private int currentPoseIndex = 0; // Menggunakan index untuk pose
    public KeyCode switchPoseKey = KeyCode.E;   // Tombol untuk ganti pose

    void Start()
    {
        // Set Pose awal (misalnya PoseA)
        if (posePrefabs.Length > 0)
        {
            SetPose(currentPoseIndex); 
        }
        else
        {
            Debug.LogError("No Pose Prefabs assigned!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(switchPoseKey))
        {
            CyclePose();
        }
    }

    // Fungsi untuk mengganti pose
    public void CyclePose()
    {
        Debug.Log("Switching Pose");
        currentPoseIndex = (currentPoseIndex + 1) % posePrefabs.Length; // Update pose index dengan modulo
        Debug.Log("Next Pose Index: " + currentPoseIndex);
        SetPose(currentPoseIndex); // Ganti pose sesuai index
    }

    // Fungsi untuk set pose sesuai index
    void SetPose(int poseIndex)
    {
        // Hapus pose sebelumnya
        if (currentPoseInstance != null)
        {
            Destroy(currentPoseInstance);
        }

        // Instantiate prefab pose yang baru
        if (poseIndex >= 0 && poseIndex < posePrefabs.Length)
        {
            currentPoseInstance = Instantiate(posePrefabs[poseIndex], transform.position, transform.rotation);
            currentPoseInstance.transform.SetParent(transform); // Set parent untuk sinkronisasi posisi
        }
        else
        {
            Debug.LogError("Invalid pose index!");
        }
    }
}
