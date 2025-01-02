using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Debug.Log("Menavigasi ke Main Menu...");
        
        // Panggil ResetGameManager() untuk mereset status game
        ResetGame();
        
        // Muat scene Main Menu
        SceneManager.LoadScene("MainMenu"); // Ganti "MainMenu" dengan nama scene Anda
    }

    private void ResetGame()
    {
        // Reset nilai-nilai yang ada di GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameManager(); // Reset semua variabel yang ada di GameManager
            
            // Opsional: Reset data yang disimpan di PlayerPrefs
            PlayerPrefs.DeleteAll();
            
            Debug.Log("Game direset ke kondisi awal.");
        }
    }
}
