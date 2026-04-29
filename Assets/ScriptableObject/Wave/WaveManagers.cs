using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagers : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private WaveData[] waves; 
    [SerializeField] private Transform spawnPoint;
    
    private int _currentWaveIndex = 0;
    private bool _isSpawning = false;

    // [PENTING]: Referensi ke script Spawner lu buat ambil fungsi pooler-nya
    private Spawner _spawner; 

    void Start()
    {
        _spawner = GetComponent<Spawner>(); // Cari script spawner di objek yang sama
    }

    void Update()
    {
        // Pake Space buat ngetes wave
        if (!_isSpawning && Input.GetKeyDown(KeyCode.Space)) 
        {
            if (_currentWaveIndex < waves.Length) StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        _isSpawning = true;
        WaveData currentWave = waves[_currentWaveIndex];

        foreach (var group in currentWave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                // [FIX BARIS 36]: Ganti 'enemyPrefab' jadi 'enemyType' sesuai WaveData.cs baru lu!
                SpawnEnemyByType(group.enemyType); 
                
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }

        _isSpawning = false;
        _currentWaveIndex++;
    }

    // [LOGIKA BARU]: Biar sinkron ama Object Pooler
    void SpawnEnemyByType(EnemyType type)
    {
        // Di sini lu panggil fungsi ActivateFromPool yang ada di script Spawner lu
        if (_spawner != null)
        {
            _spawner.ActivateFromPool(type); // [PENS STYLE]: Spawn pake Enum, bukan Instantiate GameObject!
        }
    }
}