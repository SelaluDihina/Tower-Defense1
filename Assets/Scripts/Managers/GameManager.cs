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
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed; 
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

    private void HandleEnemyReachedEnd(Enemy enemy)
    {
        if (GameIsOver) return;

        int damage = 1; 
        PlayerStats.Lives = Mathf.Max(0, PlayerStats.Lives - damage);
        OnLivesChanged?.Invoke(PlayerStats.Lives);

        if (PlayerStats.Lives <= 0)
        {
            LoseGame();
        }
        else 
        {
            // [FIX]: Kalau musuh lolos tapi lu masih hidup, tetep cek menang!
            // Takutnya ini musuh terakhir yang lewat.
            CheckWinCondition();
        }
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        if (GameIsOver) return;

        PlayerStats.Money += 10; 
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (GameIsOver || !_allWavesSpawned) return;

        Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
        
        int activeCount = 0;
        foreach (Enemy e in remainingEnemies)
        {
            if (e.gameObject.activeInHierarchy) activeCount++;
        }

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
        Time.timeScale = 0f; // [FIX]: Biar game beneran berhenti pas menang

        if (sfxSource != null && winSound != null) 
            sfxSource.PlayOneShot(winSound);

        Debug.Log("<color=green>[SYSTEM] SADIS RIZ! LU MENANG!</color>");
    }

    public void LoseGame()
    {
        if (GameIsOver) return;
        GameIsOver = true;
        
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f; 

        if (sfxSource != null && loseSound != null) 
            sfxSource.PlayOneShot(loseSound);
    }

    public void SetAllWavesSpawned()
    {
        _allWavesSpawned = true;
        Debug.Log("<color=orange>[SYSTEM] Wave terakhir sudah keluar semua!</color>");
        CheckWinCondition(); 
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}