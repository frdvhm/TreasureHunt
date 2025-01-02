using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const int NUM_LEVELS = 3; // Jumlah level dalam game
    public int level { get; private set; } = 0;
    public int lives { get; private set; } = 3;
    public int score { get; private set; } = 0;

    public Spawner spawner; // Referensi Spawner di Inspector
    private GameObject livesContainer; // LivesContainer di setiap scene

    public AudioClip endSceneSound;
    public AudioClip gameOverSound;
    private AudioSource audioSource; // AudioSource to play the sound

    public static event System.Action<int> OnLivesChanged; // Event untuk perubahan nyawa

    private void Awake()
    {
        // Pastikan hanya ada satu instance GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Jangan hancurkan GameManager saat pindah scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (spawner == null)
        {
            spawner = FindObjectOfType<Spawner>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Listener untuk sceneLoaded
        NewGame();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    public void NewGame()
    {
        // Reset nilai game seperti nyawa dan skor
        lives = 3;
        score = 0;
        level = 1;
        LoadLevel(level); // Mulai level pertama
    }

    private void LoadLevel(int index)
    {
        level = index;

        if (spawner != null)
        {
            switch (level)
            {
                case 1: spawner.SetSpawnInterval(3f); break;
                case 2: spawner.SetSpawnInterval(2f); break;
                case 3: spawner.SetSpawnInterval(1f); break;
                default: spawner.SetSpawnInterval(3f); break;
            }
        }

        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.cullingMask = 0;
        }

        Invoke(nameof(LoadScene), 1f); // Tunggu 1 detik untuk transisi
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(level > 0 ? $"Level{level}" : "Preload");
    }

    public void LevelComplete()
    {
        score += 1000;

        if (level + 1 <= NUM_LEVELS)
        {
            LoadLevel(level + 1);
        }
        else
        {
            StartCoroutine(PlaySoundAndTransition(endSceneSound, "EndScene"));
        }
    }

    public void LevelFailed()
    {
        lives--;
        OnLivesChanged?.Invoke(lives);

        if (lives <= 0)
        {
            StartCoroutine(PlaySoundAndTransition(gameOverSound, "GameOver"));
        }
        else
        {
            LoadLevel(level);
        }
    }

    private IEnumerator PlaySoundAndTransition(AudioClip clip, string sceneName)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
            DontDestroyOnLoad(audioSource.gameObject);
            yield return new WaitForSeconds(clip.length);
        }

        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindLivesContainer();
        SetLivesVisibility(scene.name != "Preload");
    }

    private void FindLivesContainer()
    {
        GameObject canvas = GameObject.Find("Canvas");

        if (canvas != null)
        {
            livesContainer = canvas.transform.Find("LivesContainer")?.gameObject;
        }
        else
        {
            livesContainer = null;
        }
    }

    private void SetLivesVisibility(bool visible)
    {
        if (livesContainer != null)
        {
            livesContainer.SetActive(visible);
        }
    }

    public void ResetGameManager()
    {
        lives = 3;
        score = 0;
        level = 0;

        if (spawner != null)
        {
            spawner.ResetSpawner();
        }

        Debug.Log("GameManager di-reset.");
    }
    public void ResetGame()
    {
        score = 0;
        lives = 3;
        Debug.Log("Game direset ke kondisi awal");
    }
}
