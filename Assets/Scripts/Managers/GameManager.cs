using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action<int> OnLivesChanged;
    public static bool GameIsOver = false;

    [Header("UI Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Audio (Win/Lose)")]
    [SerializeField] private AudioSource sfxSource; 
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    private bool _allWavesSpawned = false; 

    private void OnEnable()
    {
        // Berlangganan event saat musuh lolos atau hancur
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed; 
    }   
    
    private void OnDisable()
    {
        // Berhenti berlangganan agar tidak terjadi memory leak
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Start()
    {
        // Reset status game dan pastikan waktu berjalan normal (1f)
        PlayerStats.Lives = 10;
        GameIsOver = false;
        Time.timeScale = 1f; 

        // Sembunyikan semua panel endgame saat baru mulai
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        // Update UI nyawa di awal game
        OnLivesChanged?.Invoke(PlayerStats.Lives);
    }

    private void HandleEnemyReachedEnd(Enemy enemy)
    {
        if (GameIsOver) return;

        // Kurangi nyawa player jika tikus masuk gudang
        int damage = 1; 
        PlayerStats.Lives = Mathf.Max(0, PlayerStats.Lives - damage);
        OnLivesChanged?.Invoke(PlayerStats.Lives);

        if (PlayerStats.Lives <= 0)
        {
            LoseGame();
        }
        else 
        {
            // Cek kemenangan jika musuh terakhir lewat tapi nyawa masih ada
            CheckWinCondition();
        }
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        if (GameIsOver) return;

        // Tambah uang dan cek apakah ini musuh terakhir untuk menang
        PlayerStats.Money += 10; 
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        // Syarat menang: Semua wave sudah spawn DAN tidak ada tikus tersisa di arena
        if (GameIsOver || !_allWavesSpawned) return;

        Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
        
        int activeCount = 0;
        foreach (Enemy e in remainingEnemies)
        {
            if (e.gameObject.activeInHierarchy) activeCount++;
        }

        if (activeCount <= 1)
        {
            Invoke(nameof(WinGame), 0.1f);        }
    }

    public void WinGame()
    {
        if (GameIsOver) return;
        GameIsOver = true;
        
        if (winPanel != null) winPanel.SetActive(true); 
        Time.timeScale = 0f; // Hentikan waktu agar tower/tikus tidak bergerak lagi

        if (sfxSource != null && winSound != null) 
            sfxSource.PlayOneShot(winSound);

        Debug.Log("<color=green>[SYSTEM] MENANG!</color>");
    }

    public void LoseGame()
    {
        if (GameIsOver) return;
        GameIsOver = true;
        
        // Aktifkan LosePanel sesuai desain di LoseFix.jpg
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f; // Freeze game agar player fokus ke panel Game Over

        if (sfxSource != null && loseSound != null) 
            sfxSource.PlayOneShot(loseSound);
    }

    public void SetAllWavesSpawned()
    {
        _allWavesSpawned = true;
        Debug.Log("<color=orange>[SYSTEM] Wave terakhir sudah keluar semua!</color>");
        CheckWinCondition(); 
    }

    // --- [LOGIKA TOMBOL PADA LOSE PANEL] ---

    // Dipasang pada tombol "Coba Lagi" (LoseFix.jpg)
    public void RestartGame()
    {
        // Reset Time.timeScale ke 1f WAJIB sebelum reload scene agar game tidak freeze di awal
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Dipasang pada tombol "Kembali" (LoseFix.jpg)
    public void ToMainMenu()
    {
        // Memuat scene menu utama (pastikan MainMenu ada di index 0 pada Build Settings)
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}