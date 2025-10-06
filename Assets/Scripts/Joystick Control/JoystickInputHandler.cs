/*using UnityEngine;
using UnityEngine.InputSystem; // Tambahkan ini

public class JoystickInputHandler : MonoBehaviour
{
    // Referensi ke script Player dan PlayerPoseController
    public PlayerJ playerScript;
    public PlayerPoseControllerJ poseControllerScript;
    public GameManagerJ gameManagerScript;

    // Referensi ke Input Actions Asset
    private InputSystem_Actions inputActions;

    // Variabel untuk menyimpan nilai input
    private float moveXInput;
    private bool changePosePressed;
    private bool startGamePressed;
    private bool restartGamePressed;

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        // Mengatur callback untuk Action "MoveX"
        inputActions.Player.MoveX.performed += ctx => moveXInput = ctx.ReadValue<float>();
        inputActions.Player.MoveX.canceled += ctx => moveXInput = 0f;

        // Mengatur callback untuk Action "ChangePose"
        inputActions.Player.ChangePose.performed += ctx => changePosePressed = true;
        inputActions.Player.ChangePose.canceled += ctx => changePosePressed = false;

        // Mengatur callback untuk Action "StartGame"
        inputActions.Player.StartGame.performed += ctx => startGamePressed = true;
        inputActions.Player.StartGame.canceled += ctx => startGamePressed = false;

        // Mengatur callback untuk Action "RestartGame"
        inputActions.Player.RestartGame.performed += ctx => restartGamePressed = true;
        inputActions.Player.RestartGame.canceled += ctx => restartGamePressed = false;
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        // === Input untuk GameManager (Start/Restart) ===
        if (gameManagerScript != null)
        {
            // Main menu: tekan tombol StartGame untuk mulai
            if (!gameManagerScript.IsGameStarted && startGamePressed)
            {
                gameManagerScript.StartGame();
                startGamePressed = false; // Reset agar tidak terus-menerus memanggil
            }

            // Restart game jika game over
            if (gameManagerScript.IsGameOver && restartGamePressed)
            {
                gameManagerScript.RestartGame();
                restartGamePressed = false; // Reset agar tidak terus-menerus memanggil
            }
        }

        // Pastikan game sudah dimulai dan tidak game over untuk input player
        if (gameManagerScript != null && (!gameManagerScript.IsGameStarted || gameManagerScript.IsGameOver))
        {
            return;
        }

        // === Input untuk PlayerPoseController (Ganti Pose) ===
        if (poseControllerScript != null && changePosePressed)
        {
            poseControllerScript.CyclePose();
            changePosePressed = false; // Reset setelah diproses
        }
    }

    // Fungsi untuk mendapatkan input gerak horizontal (akan dipanggil dari Player.cs)
    public float GetMoveXInput()
    {
        return moveXInput;
    }
}*/
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickInputHandler : MonoBehaviour
{
    // Hapus event ini karena kita akan memanggil method langsung di PlayerJ
    // public static event Action OnMoveLeft;
    // public static event Action OnMoveRight;

    public PlayerJ playerScript; // <<< TAMBAHKAN INI: Referensi ke script PlayerJ
    public PlayerPoseControllerJ poseControllerScript;
    public GameManagerJ gameManagerScript;

    private InputSystem_Actions inputActions;

    // Variabel ini tidak lagi diperlukan jika langsung memanggil method
    // private bool changePosePressed;
    // private bool startGamePressed;
    // private bool restartGamePressed;

    // isMoveAxisInUse tidak lagi relevan untuk pergerakan jalur diskrit
    // private bool isMoveAxisInUse = false;

    void Awake()
    {
        Debug.Log("JoystickInputHandler: Awake dipanggil.");

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
                return; // Berhenti jika inisialisasi gagal
            }
        }
        else
        {
            Debug.Log("JoystickInputHandler: inputActions sudah diinisialisasi (tidak null).");
        }

        if (inputActions == null)
        {
            Debug.LogError("JoystickInputHandler: inputActions MASIH NULL setelah inisialisasi di Awake!");
            return;
        }

        // --- Mengatur Callback untuk Action Gerakan "MoveX" (Untuk Pergerakan Jalur) ---
        // Kita akan menggunakan ini untuk mendeteksi input horizontal dan memanggil PlayerJ.MoveLane
        inputActions.Player.MoveX.performed += ctx =>
        {
            if (playerScript == null) // Pastikan playerScript sudah di-assign
            {
                Debug.LogWarning("PlayerJ script reference is missing in JoystickInputHandler for MoveX!");
                return;
            }

            float moveValue = ctx.ReadValue<float>();

            // Gunakan ambang batas untuk mendeteksi gerakan ke kiri atau kanan
            // Dan panggil PlayerJ.MoveLane()
            if (moveValue > 0.7f) // Jika joystick bergerak ke kanan
            {
                playerScript.MoveLane(1); // Panggil method untuk bergerak ke kanan
                Debug.Log("Move Right triggered (lane transition).");
            }
            else if (moveValue < -0.7f) // Jika joystick bergerak ke kiri
            {
                playerScript.MoveLane(-1); // Panggil method untuk bergerak ke kiri
                Debug.Log("Move Left triggered (lane transition).");
            }
            // Catatan: Jika MoveX adalah Axis dan `performed` event terus-menerus
            // dipicu saat joystick miring, pastikan logika `isMoving` di PlayerJ
            // sudah cukup untuk mencegah pergerakan ganda.
            // Alternatifnya, Anda bisa membuat Actions terpisah (MoveLeft, MoveRight)
            // di Input Actions Asset jika ingin trigger yang lebih diskrit per "ketukan".
        };

        // Callback 'canceled' untuk MoveX tidak lagi digunakan untuk mereset isMoveAxisInUse
        // Karena pergerakan ditangani secara diskrit oleh MoveLane.
        // Anda bisa menghapusnya jika tidak ada kebutuhan lain.
        // inputActions.Player.MoveX.canceled += ctx => {
        //     Debug.Log("MoveX canceled (input released).");
        // };

        // --- Mengatur Callback untuk Action Jump (Contoh) ---
        // Jika Anda memiliki action "Jump" di Input Actions Asset, tambahkan di sini
        // Misalnya:
/*        inputActions.Player.Jump.performed += ctx =>
        {
            if (playerScript != null)
            {
                playerScript.Jump(); // Panggil method Jump di PlayerJ
                Debug.Log("Jump performed.");
            }
            else
            {
                Debug.LogWarning("PlayerJ script reference is missing in JoystickInputHandler for Jump!");
            }
        };*/


        // --- Mengatur Callback untuk Action Lainnya (tetap gunakan performed) ---
        inputActions.Player.ChangePose.performed += ctx =>
        {
            if (poseControllerScript != null)
            {
                poseControllerScript.CyclePose();
                Debug.Log("ChangePose performed.");
            }
            else
            {
                Debug.LogWarning("PlayerPoseControllerJ script reference is missing in JoystickInputHandler for ChangePose!");
            }
        };

        inputActions.Player.StartGame.performed += ctx =>
        {
            if (gameManagerScript != null)
            {
                // Logika: Hanya mulai game jika belum dimulai
                if (!gameManagerScript.IsGameStarted)
                {
                    gameManagerScript.StartGame();
                    Debug.Log("StartGame performed.");
                }
                else
                {
                    Debug.Log("StartGame performed, but game is already running.");
                }
            }
            else
            {
                Debug.LogWarning("GameManagerJ script reference is missing in JoystickInputHandler for StartGame!");
            }
        };

        inputActions.Player.RestartGame.performed += ctx =>
        {
            if (gameManagerScript != null)
            {
                // Logika: Hanya restart game jika game over
                if (gameManagerScript.IsGameOver)
                {
                    gameManagerScript.RestartGame();
                    Debug.Log("RestartGame performed.");
                }
                else
                {
                    Debug.Log("RestartGame performed, but game is not in Game Over state.");
                }
            }
            else
            {
                Debug.LogWarning("GameManagerJ script reference is missing in JoystickInputHandler for RestartGame!");
            }
        };
    }

    void OnEnable()
    {
        Debug.Log("JoystickInputHandler: OnEnable dipanggil.");
        if (inputActions != null)
        {
            inputActions.Enable();
            Debug.Log("JoystickInputHandler: inputActions di-enable.");
        }
        else
        {
            Debug.LogError("JoystickInputHandler: inputActions NULL di OnEnable! Tidak dapat mengaktifkan.");
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
        else
        {
            Debug.LogWarning("JoystickInputHandler: inputActions NULL di OnDisable! Tidak perlu dinonaktifkan.");
        }
    }

    // Fungsi Update() di JoystickInputHandler bisa dihapus atau dikosongkan karena semua input
    // sekarang ditangani melalui event callback Input System.
    // void Update()
    // {
    //
    // }
}