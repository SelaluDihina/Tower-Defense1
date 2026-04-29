using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // [PENTING]: Ini kabel yang dicari UIController lu!
    public static Action<int, int> OnWaveChanged;
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

    public void ActivateFromPool(EnemyType type, int waveIndex)
    {
        // Pas ganti wave, teriak ke UI!
        if (_pools.TryGetValue(type, out var pool)) {
            GameObject obj = pool.GetPooledObject();
            if (obj != null) {
                Debug.Log($"<color=green>BERHASIL SPAWN: {type}</color>"); // <--- TAMBAHIN INI RIZ!
                obj.transform.position = transform.position;
                Enemy e = obj.GetComponent<Enemy>();
                if (e != null) e.SetDifficultyScale(waveIndex);
                obj.SetActive(true);
            }
            else {
            Debug.LogError($"<color=yellow>POOL KOSONG BUAT: {type}!</color>");
        }
    }
  }
}