using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Pastikan Anda mengimpor UI

public class GameOverManager : MonoBehaviour
{
    // Tombol Restart dan Main Menu yang harus di-assign di Inspector
    public Button restartButton;
    public Button mainMenuButton; // Tambahan tombol Main Menu

    private void Start()
    {
        // Pastikan tombol Restart sudah di-assign
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogWarning("Tombol Restart belum di-assign di inspector.");
        }

        // Pastikan tombol Main Menu sudah di-assign
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        else
        {
            Debug.LogWarning("Tombol Main Menu belum di-assign di inspector.");
        }
    }

    // Fungsi untuk restart game
    private void RestartGame()
    {
        ResetGame();
        SceneManager.LoadScene("Preload"); // Kembali ke scene Preload
    }

    // Fungsi untuk pindah ke Main Menu
    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Ganti "MainMenu" dengan nama scene Main Menu Anda
    }

    // Fungsi untuk mengatur ulang game
    private void ResetGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NewGame(); // Reset game melalui GameManager
        }
        else
        {
            Debug.LogWarning("GameManager belum diinisialisasi.");
        }
    }
}
