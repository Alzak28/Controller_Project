using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO.Ports; // <-- TAMBAHKAN INI

public class JoystickInputHandler : MonoBehaviour
{
    // --- Referensi Script (Sudah ada) ---
    public PlayerJ playerScript;
    public PlayerPoseControllerJ poseControllerScript;
    public GameManagerJ gameManagerScript;

    private InputSystem_Actions inputActions;

    // --- Variabel Logika ARDUINO (BARU) ---
    [Header("Pengaturan Arduino")]
    public string arduinoPortName = "COM3"; // <<< GANTI INI SESUAI PORT ANDA
    public int arduinoBaudRate = 9600;
    public float arduinoMoveThreshold = 5.0f; // Sensitivitas (seberapa miring)

    private SerialPort arduinoSerialPort;
    private bool isArduinoConnected = false;
    private bool isArduinoAxisInUse = false; // Cooldown untuk Arduino


    void Awake()
    {
        Debug.Log("JoystickInputHandler: Awake dipanggil.");

        // --- Bagian Input System (Sudah ada) ---
        if (inputActions == null)
        {
            try
            {
                inputActions = new InputSystem_Actions();
                Debug.Log("JoystickInputHandler: inputActions berhasil diinisialisasi.");
            }
            catch (Exception ex)
            {
                Debug.LogError("JoystickInputHandler: Gagal menginisialisasi InputSystem_Actions: " + ex.Message);
                return;
            }
        }

        // --- Callback Input System (Sudah ada) ---
        inputActions.Player.MoveX.performed += ctx =>
        {
            if (playerScript == null) return;

            float moveValue = ctx.ReadValue<float>();

            if (moveValue > 0.7f)
            {
                playerScript.MoveLane(1);
                Debug.Log("Input System: Move Right");
            }
            else if (moveValue < -0.7f)
            {
                playerScript.MoveLane(-1);
                Debug.Log("Input System: Move Left");
            }
        };

        inputActions.Player.ChangePose.performed += ctx =>
        {
            if (poseControllerScript != null)
            {
                poseControllerScript.CyclePose();
                Debug.Log("Input System: ChangePose performed.");
            }
        };

        // ... (Callback Input System lainnya: StartGame, RestartGame, dll. tetap sama) ...
        inputActions.Player.StartGame.performed += ctx => { /* ... */ };
        inputActions.Player.RestartGame.performed += ctx => { /* ... */ };
    }

    // --- Method Start (BARU) ---
    // Kita gunakan Start() untuk koneksi Arduino
    void Start()
    {
        // Pastikan setting .NET 4.x sudah diatur di Project Settings > Player
        try
        {
            arduinoSerialPort = new SerialPort(arduinoPortName, arduinoBaudRate);
            arduinoSerialPort.ReadTimeout = 100; // Timeout agar tidak 'freeze'
            arduinoSerialPort.Open();
            isArduinoConnected = true;
            Debug.Log("Koneksi Arduino Berhasil di port " + arduinoPortName);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Koneksi Arduino Gagal: " + e.Message);
            isArduinoConnected = false;
        }
    }

    // --- Method Update (BARU) ---
    // Kita gunakan Update() untuk MEMBACA Arduino di setiap frame
    void Update()
    {
        // Jangan lakukan apa-apa jika Arduino tidak terhubung
        if (!isArduinoConnected || arduinoSerialPort == null || !arduinoSerialPort.IsOpen) return;

        try
        {
            // 1. Baca data dari Arduino (misal: "2.50,1.10,9.80")
            string dataString = arduinoSerialPort.ReadLine();
            string[] values = dataString.Split(',');

            if (values.Length == 3)
            {
                // 2. Ubah data string menjadi angka
                float x = float.Parse(values[0]);
                float y = float.Parse(values[1]);
                float z = float.Parse(values[2]);

                // 3. TERJEMAHKAN DATA MENJADI AKSI
                // Ini adalah logika "penerjemah" dari sensor miring ke aksi game

                // --- Logika Gerak Kiri/Kanan (MoveX) ---
                if (x > arduinoMoveThreshold) // Miring Kanan
                {
                    if (!isArduinoAxisInUse && playerScript != null)
                    {
                        playerScript.MoveLane(1); // Panggil method yang SAMA
                        isArduinoAxisInUse = true; // Set cooldown
                        Debug.Log("ARDUINO: Move Right");
                    }
                }
                else if (x < -arduinoMoveThreshold) // Miring Kiri
                {
                    if (!isArduinoAxisInUse && playerScript != null)
                    {
                        playerScript.MoveLane(-1); // Panggil method yang SAMA
                        isArduinoAxisInUse = true; // Set cooldown
                        Debug.Log("ARDUINO: Move Left");
                    }
                }
                else // Posisi tengah
                {
                    isArduinoAxisInUse = false; // Reset cooldown
                }

                // --- (Contoh) Logika Ganti Pose (misal: dari sumbu Z) ---
                // if (z < -5.0f) // Misalnya, jika sensor dibalik
                // {
                //     poseControllerScript.CyclePose();
                // }
            }
        }
        catch (System.TimeoutException) { /* Ini wajar, abaikan */ }
        catch (Exception e) { Debug.LogWarning("Error membaca data Arduino: " + e.Message); }
    }


    void OnEnable()
    {
        Debug.Log("JoystickInputHandler: OnEnable dipanggil.");
        if (inputActions != null)
        {
            inputActions.Enable();
            Debug.Log("JoystickInputHandler: inputActions di-enable.");
        }
    }

    void OnDisable()
    {
        Debug.Log("JoystickInputHandler: OnDisable dipanggil.");
        if (inputActions != null)
        {
            inputActions.Disable();
            Debug.Log("JoystickInputHandler: inputActions di-disable.");
        }
    }

    // --- Method Penutup Port (BARU) ---
    void OnApplicationQuit()
    {
        if (arduinoSerialPort != null && arduinoSerialPort.IsOpen)
        {
            arduinoSerialPort.Close();
            Debug.Log("Port Arduino ditutup.");
        }
    }
}