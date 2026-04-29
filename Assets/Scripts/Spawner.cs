using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // [PENTING]: Ini kabel yang dicari UIController lu!
    public static Action<int, int> OnWaveChanged;

    [SerializeField] private WaveData[] waves;
    [SerializeField] private ObjectPooler biasaPool, lincahPool, rajaPool;

    private Dictionary<EnemyType, ObjectPooler> _pools;
    public int _currentWaveIndex = 0; // Biar bisa dibaca script lain

    void Awake() {
        _pools = new Dictionary<EnemyType, ObjectPooler> {
            { EnemyType.TikusBiasa, biasaPool },
            { EnemyType.TikusLincah, lincahPool },
            { EnemyType.RajaTikus, rajaPool }
        };
    }

    public void ActivateFromPool(EnemyType type)
    {
        // Pas ganti wave, teriak ke UI!
        OnWaveChanged?.Invoke(_currentWaveIndex + 1, waves.Length);

        if (_pools.TryGetValue(type, out var pool)) {
            GameObject obj = pool.GetPooledObject();
            if (obj != null) {
                obj.transform.position = transform.position;
                Enemy e = obj.GetComponent<Enemy>();
                if (e != null) e.SetDifficultyScale(_currentWaveIndex);
                obj.SetActive(true);
            }
        }
    }
}