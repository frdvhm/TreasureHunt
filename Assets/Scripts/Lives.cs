using UnityEngine;
using UnityEngine.UI;

public class Lives : MonoBehaviour
{
    public GameObject heartPrefab; // Prefab gambar hati
    public Transform livesContainer; // Tempat menyimpan hati
    private int currentLives;

    private void Start()
    {
        // Daftarkan listener untuk event lives yang berubah
        GameManager.OnLivesChanged += UpdateLivesDisplay;

        // Tampilkan nyawa saat mulai
        UpdateLivesDisplay(GameManager.Instance.lives);
    }

    private void OnDestroy()
    {
        // Unsubscribe ketika object ini dihancurkan
        GameManager.OnLivesChanged -= UpdateLivesDisplay;
    }

    private void UpdateLivesDisplay(int lives)
    {
        currentLives = lives;
        Debug.Log("Updating lives display: " + lives); // Debug saat memperbarui tampilan

        // Hapus semua hati yang ada
        foreach (Transform child in livesContainer)
        {
            Destroy(child.gameObject);
        }

        // Tambahkan hati sesuai jumlah nyawa
        for (int i = 0; i < currentLives; i++)
        {
            Instantiate(heartPrefab, livesContainer);
        }
    }
}
