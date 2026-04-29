using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Action<int, int> OnWaveChanged;

    [SerializeField] private ObjectPooler biasaPool;
    [SerializeField] private ObjectPooler lincahPool;
    [SerializeField] private ObjectPooler rajaPool;

    private Dictionary<EnemyType, ObjectPooler> _pools;

    void Awake()
    {
        _pools = new Dictionary<EnemyType, ObjectPooler>
        {
            { EnemyType.TikusBiasa,  biasaPool  },
            { EnemyType.TikusLincah, lincahPool },
            { EnemyType.RajaTikus,   rajaPool   }
        };
    }

    public void ActivateFromPool(EnemyType type, int waveIndex)
    {
        if (!_pools.TryGetValue(type, out var pool)) return;

        GameObject obj = pool.GetPooledObject();
        if (obj == null)
        {
            Debug.LogError($"Pool kosong buat: {type}!");
            return;
        }

        obj.transform.position = transform.position;

        // [FIX MJ UTAMA]: SetDifficultyScale DULU sebelum SetActive
        // SetActive → OnEnable → EnemyMovement mulai gerak pakai MoveSpeed
        // Kalau SetDifficultyScale belum dipanggil, MoveSpeed masih 0 dari Die()
        // Enemy ga gerak tapi facing-nya update → keliatan MJ
        Enemy e = obj.GetComponent<Enemy>();
        if (e != null) e.SetDifficultyScale(waveIndex);

        obj.SetActive(true);

        Debug.Log($"<color=green>SPAWN: {type} | Wave {waveIndex + 1}</color>");
    }
}