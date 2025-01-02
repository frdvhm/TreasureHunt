using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab; // Objek yang akan di-spawn
    public float spawnInterval = 3f; // Interval spawn default (Level 1)
    public AudioClip spawnSound; // Audio clip untuk efek suara
    private AudioSource audioSource; // Referensi ke Audio Source

    private Coroutine spawnCoroutine; // Referensi ke Coroutine spawn
    private Transform spawnedObjectsContainer; // Kontainer untuk menyimpan semua objek spawn

    private void Awake()
    {
        // Pastikan AudioSource ada di GameObject ini
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Buat kontainer untuk objek spawn jika belum ada
        if (spawnedObjectsContainer == null)
        {
            spawnedObjectsContainer = new GameObject("SpawnedObjects").transform;
        }
    }

    private void OnEnable()
    {
        StartSpawning();
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    // Mulai proses spawning
    public void StartSpawning()
    {
        // Jika Coroutine sudah berjalan, hentikan terlebih dahulu
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    // Hentikan proses spawning
    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    // Coroutine yang mengatur spawning berdasarkan spawnInterval
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Fungsi untuk memunculkan prefab
    private void Spawn()
    {
        // Spawn objek dan simpan di dalam kontainer
        GameObject spawnedObject = Instantiate(prefab, transform.position, Quaternion.identity);
        spawnedObject.transform.parent = spawnedObjectsContainer;

        // Putar suara spawn menggunakan PlayOneShot
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }

    // Fungsi untuk mengatur interval spawn
    public void SetSpawnInterval(float interval)
    {
        spawnInterval = Mathf.Max(0.1f, interval); // Pastikan minimal 0.1 detik untuk performa
        StartSpawning(); // Restart Coroutine dengan interval baru
    }

    // Fungsi untuk membersihkan semua objek yang di-spawn
    public void ClearAllSpawnedObjects()
    {
        if (spawnedObjectsContainer != null)
        {
            foreach (Transform child in spawnedObjectsContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    // Fungsi untuk reset Spawner ke kondisi awal
    public void ResetSpawner()
    {
        StopSpawning(); // Hentikan proses spawning
        ClearAllSpawnedObjects(); // Hapus semua objek spawn
        SetSpawnInterval(3f); // Atur ulang interval ke nilai default
        Debug.Log("Spawner di-reset.");
    }
}
