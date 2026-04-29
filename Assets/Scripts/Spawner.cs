using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private WaveData[] waves;
    [SerializeField] private ObjectPooler biasaPool, lincahPool, rajaPool;

    private Dictionary<EnemyType, ObjectPooler> _pools;
    private int _currentWaveIndex = 0;

    void Awake() {
        _pools = new Dictionary<EnemyType, ObjectPooler> {
            { EnemyType.TikusBiasa, biasaPool },
            { EnemyType.TikusLincah, lincahPool },
            { EnemyType.RajaTikus, rajaPool }
        };
    }

    // [FUNGSI PUBLIC]: Dibuka gemboknya biar bisa dipanggil WaveManager
    public void ActivateFromPool(EnemyType type)
    {
        if (_pools.TryGetValue(type, out var pool)) {
            GameObject obj = pool.GetPooledObject();
            if (obj != null) {
                obj.transform.position = transform.position;
                
                // Suntik Scaling Wave
                Enemy e = obj.GetComponent<Enemy>();
                if (e != null) e.SetDifficultyScale(_currentWaveIndex);

                obj.SetActive(true);
            }
        }
    }
}