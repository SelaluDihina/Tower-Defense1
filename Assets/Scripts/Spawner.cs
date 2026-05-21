using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Action<int, int> OnWaveChanged;

    [SerializeField] private ObjectPooler biasaPool;
    [SerializeField] private ObjectPooler lincahPool;
    [SerializeField] private ObjectPooler rajaPool;

    // --- SUNTIKAN SAKTI DUAL PATH (TIDAK MENGHAPUS KODE LAMA) ---
    [Header("Dual Path Settings")]
    [SerializeField] private Path pathAtas;  // Slot buat masukin objek PathAtas lu
    [SerializeField] private Path pathBawah; // Slot buat masukin objek Pathbawah lu
    // ------------------------------------------------------------

    private Dictionary<EnemyType, ObjectPooler> _pools;

    void Awake()
    {
        _pools = new Dictionary<EnemyType, ObjectPooler>
        {
            { EnemyType.TikusBiasa,  biasaPool  },
            { EnemyType.TikusLincah, lincahPool },
            { EnemyType.RajaTikus,   rajaPool    }
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

        Enemy e = obj.GetComponent<Enemy>();
        if (e != null) e.SetDifficultyScale(waveIndex);

        // --- ARSITEKTUR PENENTUAN JALUR ACAK TIKUS (SUNTIKAN BARU) ---
        // Kita cek apakah kedua script bapak jalan ini udah lu colok di Inspector
        if (pathAtas != null && pathBawah != null)
        {
            // Ambil komponen pergerakan tikus (biasanya nempel bareng script Enemy atau script terpisah)
            // Sesuai kodingan lu, kita coba ambil script EnemyMovement (sesuaikan nama class movement lu jika berbeda)
            EnemyMovement movement = obj.GetComponent<EnemyMovement>();
            
            if (movement != null)
            {
                // Menggunakan fungsi acak bawaan Unity (0.0 sampai 1.0)
                // Jika angka keluar di atas 0.5f (50% peluang), tikus dipaksa lewat Kasta Atas
                if (UnityEngine.Random.value > 0.5f)
                {
                    e.SetPath(pathAtas);
                }
                else // 50% peluang sisanya, tikus dipaksa lewat Kasta Bawah
                {
                    e.SetPath(pathBawah);
                }
            }
        }
        // ------------------------------------------------------------------

        obj.SetActive(true);

        Debug.Log($"<color=green>SPAWN: {type} | Wave {waveIndex + 1}</color>");
    }
}