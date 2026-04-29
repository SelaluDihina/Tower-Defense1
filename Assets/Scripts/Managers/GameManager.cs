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
        // [LOGIKA]: Dengerin pengumuman dari script Enemy
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed; // Kita pisah fungsinya biar rapi
    }   
    
    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Start()
    {
        GameIsOver = false;
        Time.timeScale = 1f; 

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        OnLivesChanged?.Invoke(PlayerStats.Lives);
    }

    // --- [FIX]: Sekarang nerima data (Enemy enemy) bukan cuma data mentah ---
    private void HandleEnemyReachedEnd(Enemy enemy)
    {
        if (GameIsOver) return;

        // Ambil damage dari komponen Enemy-nya
        // Pastiin di script Enemy lu ada variabel buat nyimpen damage (biasanya di data)
        int damage = 1; // Default 1 kalo lu belum set di EnemyData
        
        PlayerStats.Lives = Mathf.Max(0, PlayerStats.Lives - damage);
        OnLivesChanged?.Invoke(PlayerStats.Lives);

        if (PlayerStats.Lives <= 0)
        {
            LoseGame();
        }
    }

    // --- [FIX]: Fungsi baru buat ngurusin duit + cek menang ---
    private void HandleEnemyDestroyed(Enemy enemy)
    {
        if (GameIsOver) return;

        // 1. TAMBAH DUIT: Biar lu kaga miskin pas mau bangun tower
        // Asumsi: Tiap tikus mati dapet 10 gold. Nanti lu bisa bikin variasi di EnemyData.
        PlayerStats.Money += 10; 
        
        // 2. CEK KONDISI MENANG
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (GameIsOver || !_allWavesSpawned) return;

        // [LOGIKA]: Cari semua musuh yang aktif di Hierarchy
        Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
        
        int activeCount = 0;
        foreach (Enemy e in remainingEnemies)
        {
            if (e.gameObject.activeInHierarchy) activeCount++;
        }

        // Kalau kaga ada musuh tersisa, berarti lu MENANG!
        if (activeCount <= 0)
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        if (GameIsOver) return;
        GameIsOver = true;
        
        if (winPanel != null) winPanel.SetActive(true);
        if (sfxSource != null && winSound != null) sfxSource.PlayOneShot(winSound);
    }

    public void LoseGame()
    {
        if (GameIsOver) return;
        GameIsOver = true;
        
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f; 

        if (sfxSource != null && loseSound != null) sfxSource.PlayOneShot(loseSound);
    }

    public void SetAllWavesSpawned()
    {
        _allWavesSpawned = true;
        CheckWinCondition();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}