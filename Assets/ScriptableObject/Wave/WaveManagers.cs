using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagers : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private WaveData[] waves; 
    [SerializeField] private float timeBetweenWaves = 5f; // Jeda 5 detik antar wave
    
    private int _currentWaveIndex = 0;
    private bool _isSpawning = false;
    private float _countdown = 2f; // Timer buat wave pertama

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

        // [LOGIKA OTOMATIS]: Ngitung mundur buat wave berikutnya
        if (_countdown <= 0f)
        {
            if (_currentWaveIndex < waves.Length)
            {
                StartCoroutine(SpawnWave());
                _countdown = timeBetweenWaves; // Reset timer buat wave selanjutnya
            }
        }

        _countdown -= Time.deltaTime;
    }

    IEnumerator SpawnWave()
    {
        _isSpawning = true;
        WaveData currentWave = waves[_currentWaveIndex];

        // Lapor ke UI
        Spawner.OnWaveChanged?.Invoke(_currentWaveIndex + 1, waves.Length);
        Debug.Log($"<color=red>WAVE {_currentWaveIndex + 1} DIMULAI!</color>");

        foreach (var group in currentWave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                if (_spawner != null) _spawner.ActivateFromPool(group.enemyType);
                yield return new WaitForSeconds(group.spawnInterval);
            }
            yield return new WaitForSeconds(1f); // Jeda antar grup musuh
        }

        _isSpawning = false;
        _currentWaveIndex++;

        if (_currentWaveIndex >= waves.Length)
        {
            if(_gameManager != null) _gameManager.SetAllWavesSpawned();
        }
    }
}