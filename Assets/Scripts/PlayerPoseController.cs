using UnityEngine;

public class PlayerPoseController : MonoBehaviour
{
    public GameObject[] posePrefabs;
    private GameObject currentPoseInstance;
    private int currentPoseIndex = 0;

    [Header("Sensor")]
    public ArduinoController sensor; // drag ArduinoController
    public float poseTriggerY = 0.6f; // ambang Y untuk trigger pose (g)
    public float poseCooldown = 0.8f;
    float lastPoseTime = -10f;

    void Start()
    {
        if (posePrefabs.Length > 0) SetPose(currentPoseIndex);
        else Debug.LogError("No Pose Prefabs assigned!");
    }

    void Update()
    {
        // input sensor
        if (sensor != null)
        {
            if (Time.time - lastPoseTime >= poseCooldown)
            {
                if (sensor.Y > poseTriggerY)
                {
                    CyclePose();
                    lastPoseTime = Time.time;
                }
            }
        }

        // keyboard fallback (optional)
        if (Input.GetKeyDown(KeyCode.E))
        {
            CyclePose();
        }
    }

    public void CyclePose()
    {
        currentPoseIndex = (currentPoseIndex + 1) % posePrefabs.Length;
        SetPose(currentPoseIndex);
    }

    void SetPose(int poseIndex)
    {
        if (currentPoseInstance != null) Destroy(currentPoseInstance);
        if (poseIndex >= 0 && poseIndex < posePrefabs.Length)
        {
            currentPoseInstance = Instantiate(posePrefabs[poseIndex], transform.position, transform.rotation);
            currentPoseInstance.transform.SetParent(transform);
        }
    }
}
