using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagers : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private WaveData[] waves; 
    [SerializeField] private float timeBetweenWaves = 5f; 
    
    private int _currentWaveIndex = 0;
    private bool _isSpawning = false;
    private float _countdown = 2f; 

    private Spawner _spawner;
    private GameManager _gameManager;

    void Start()
    {
        _spawner = GetComponent<Spawner>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (_isSpawning) return;

        if (_countdown <= 0f)
        {
            if (_currentWaveIndex < waves.Length)
            {
                StartCoroutine(SpawnWave());
                _countdown = timeBetweenWaves; 
            }
        }

        _countdown -= Time.deltaTime;
    }

    IEnumerator SpawnWave()
    {
        _isSpawning = true;
        WaveData currentWave = waves[_currentWaveIndex];

        Spawner.OnWaveChanged?.Invoke(_currentWaveIndex + 1, waves.Length); 

        Debug.Log($"<color=red>WAVE {_currentWaveIndex + 1} DIMULAI!</color>");

        foreach (var group in currentWave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                if (_spawner != null) _spawner.ActivateFromPool(group.enemyType, _currentWaveIndex);
                yield return new WaitForSeconds(group.spawnInterval);
            }
            yield return new WaitForSeconds(1f); 
        }

        _isSpawning = false;
        _currentWaveIndex++;

        // =========================================================
        // [LOGIKA HARD MODE PENS]: LAPOR KE GAMEMANAGER
        // =========================================================
        // Cek apakah wave yang baru beres di-spawn adalah wave terakhir
        if (_currentWaveIndex >= waves.Length) 
        {
            if (_gameManager != null) 
            {
                // Cetek saklar di GameManager biar dia mulai ngecek musuh sisa
                _gameManager.SetAllWavesSpawned(); 
                Debug.Log("<color=orange>[WAVE] Laporan: Semua wave sudah keluar! Cek arena...</color>"); 
            }
        }
    }
}