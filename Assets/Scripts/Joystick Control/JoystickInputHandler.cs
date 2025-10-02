using UnityEngine;
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
}