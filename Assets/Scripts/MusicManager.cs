using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip bgmLevel1; // Assign bgm1
    public AudioClip bgmLevel2; // Assign bgm2
    public AudioClip bgmLevel3; // Assign bgm3
    private AudioSource audioSource;

    private void Awake()
    {
        // Make this MusicManager persist across scenes
        DontDestroyOnLoad(gameObject);

        // Add AudioSource component if not already present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true; // Loop background music
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Level1":
                PlayMusic(bgmLevel1);
                break;
            case "Level2":
                PlayMusic(bgmLevel2);
                break;
            case "Level3":
                PlayMusic(bgmLevel3);
                break;
            default:
                audioSource.Stop(); // Stop music for other scenes
                break;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip) return; // Don't restart the same music
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
