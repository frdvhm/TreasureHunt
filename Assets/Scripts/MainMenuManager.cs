using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    public AudioClip mainMenuMusic;
    public AudioClip startButtonSound;
    public AudioClip tutorialButtonSound; // Suara untuk tombol Tutorial (opsional)
    private AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (mainMenuMusic != null)
        {
            audioSource.clip = mainMenuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void Update()
    {
        // Deteksi tombol spasi untuk memulai game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        StartCoroutine(PlayStartSoundAndTransition());
    }

    private IEnumerator PlayStartSoundAndTransition()
    {
        if (startButtonSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(startButtonSound);
            yield return new WaitForSeconds(startButtonSound.length);
        }

        // Reset status game melalui GameManager sebelum memulai game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NewGame(); // Panggil NewGame() untuk mereset game
        }

        // Muat scene Preload atau level pertama
        SceneManager.LoadScene("Preload");
    }

    // Fungsi untuk keluar dari game
    public void QuitGame()
    {
        Debug.Log("Keluar dari game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Fungsi untuk pindah ke scene Tutorial
    public void OpenTutorial()
    {
        StartCoroutine(PlayTutorialSoundAndTransition());
    }

    private IEnumerator PlayTutorialSoundAndTransition()
    {
        if (tutorialButtonSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(tutorialButtonSound);
            yield return new WaitForSeconds(tutorialButtonSound.length);
        }

        // Muat scene "Tutorial"
        SceneManager.LoadScene("Tutorial");
    }
}
