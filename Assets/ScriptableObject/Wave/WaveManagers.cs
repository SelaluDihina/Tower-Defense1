using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagers : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private WaveData[] waves; 
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float timeBetweenGroups = 2f; // Jeda antar pleton musuh

    private int _currentWaveIndex = 0;
    private bool _isSpawning = false;

    private Spawner _spawner;
    private GameManager _gameManager;

    void Start()
    {
        _spawner = GetComponent<Spawner>();
        _gameManager = FindObjectOfType<GameManager>(); // Buat lapor pas wave 15 kelar
    }

    void Update()
    {
        // Tombol Space buat start wave. 
        // Riz, nanti kalo udah stabil, lu bisa bikin ini otomatis pake timer.
        if (!_isSpawning && Input.GetKeyDown(KeyCode.Space)) 
        {
            if (_currentWaveIndex < waves.Length) 
            {
                StartCoroutine(SpawnWave());
            }
        }
    }

    IEnumerator SpawnWave()
    {
        _isSpawning = true;
        WaveData currentWave = waves[_currentWaveIndex];

        // [LOGIKA 1]: Update UI 1/15
        // Kita panggil event di Spawner biar UI tau sekarang wave berapa
        Spawner.OnWaveChanged?.Invoke(_currentWaveIndex + 1, waves.Length);

        Debug.Log($"<color=red>WAVE {_currentWaveIndex + 1} DIMULAI!</color>");

        // [LOGIKA 2]: Nested Loop buat Gerombolan (Swarm)
        foreach (var group in currentWave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemyByType(group.enemyType); 
                
                // Kalo lu set spawnInterval 0.1 di Inspector, mereka keluar kyk tawuran!
                yield return new WaitForSeconds(group.spawnInterval);
            }

            // Jeda antar grup biar kaga numpuk kyk sarden (Pacing)
            yield return new WaitForSeconds(timeBetweenGroups);
        }

        _isSpawning = false;
        _currentWaveIndex++;

        // [LOGIKA 3]: Cek Menang Final
        if (_currentWaveIndex >= waves.Length)
        {
            _gameManager.SetAllWavesSpawned(); // Lapor ke GameManager: "Musuh abis, cek sisa di map!"
        }
    }

    void SpawnEnemyByType(EnemyType type)
    {
        if (_spawner != null)
        {
            _spawner.ActivateFromPool(type); 
        }
    }
}