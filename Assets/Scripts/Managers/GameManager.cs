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
    // AudioSource itu speaker-nya, AudioClip itu kaset/file suaranya
    [SerializeField] private AudioSource sfxSource; 
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    private bool _allWavesSpawned = false;

    // --- GRUP WA: Daftarin GameManager biar dapet notif dari script lain ---
    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += CheckWinCondition;
    }   
    
    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= CheckWinCondition;
    }

    private void Start()
    {
        GameIsOver = false;
        Time.timeScale = 1f; // Biar waktu jalan normal lagi pas restart

        // Sembunyiin UI menang/kalah pas baru mulai
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        // Kasih tau UI lives buat nampilin angka darah awal
        OnLivesChanged?.Invoke(PlayerStats.Lives);
    }

    // --- KALO TIKUS TEMBUS: Darah lu dikurangin di sini ---
    private void HandleEnemyReachedEnd(EnemyData data)
    {
        if (GameIsOver) return;

        // Kurangi nyawa player sesuai damage si tikus
        PlayerStats.Lives = Mathf.Max(0, PlayerStats.Lives - data.damage);
        OnLivesChanged?.Invoke(PlayerStats.Lives);

        if (PlayerStats.Lives <= 0)
        {
            LoseGame();
        }
    }

    // --- CEK MENANG: Tiap ada musuh mati, script ini muter-muter map buat ngitung ---
    private void CheckWinCondition(Enemy enemy)
    {
        if (GameIsOver || !_allWavesSpawned) return;

        // Cari semua musuh yang lagi nempel di game
        Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
        
        int activeCount = 0;
        foreach (Enemy e in remainingEnemies)
        {
            // Cek beneran aktif/masih idup gak?
            if (e.gameObject.activeSelf) activeCount++;
        }

        // Kalau sisa 1 (si pelapor) atau 0, berarti BERSIH!
        if (activeCount <= 1)
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        if (GameIsOver) return;
        GameIsOver = true;
        
        Debug.Log("GG!");
        if (winPanel != null) winPanel.SetActive(true);

        // Putar suara menang sekali (PlayOneShot)
        if (sfxSource != null && winSound != null) sfxSource.PlayOneShot(winSound);
    }

    public void LoseGame()
    {
        if (GameIsOver) return;
        GameIsOver = true;
        
        Debug.Log("KALAH!");
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f; // Freeze game biar dramatis

        // Putar suara kalah
        if (sfxSource != null && loseSound != null) sfxSource.PlayOneShot(loseSound);
    }

    public void SetAllWavesSpawned()
    {
        _allWavesSpawned = true;
        CheckWinCondition(null);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}