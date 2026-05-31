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

        // ==================================================================
        // --- STEP 1: TELEPORTASI AWAL (WAJIB PALING ATAS!) ---
        // Pindahkan posisi fisik objek dari pool ke koordinat Spawner detik ini juga,
        // sebelum algoritma SetPath mengunci koordinat Waypoint pertama!
        // ==================================================================
        obj.transform.position = transform.position;

        // --- HARD MODE EXPLANATION (MENDALAM TIAP BARIS) ---
        // 2. Ambil komponen pergerakan tikus secara presisi dari pool
        EnemyMovement movement = obj.GetComponent<EnemyMovement>();

        if (movement != null)
        {
            // OPSI A: LOGIKA DUAL PATH (LEVEL 2)
            // Jika slot Inspector pathAtas dan pathBawah terisi, jalankan pembagian kasta jalur acak 50:50
            if (pathAtas != null && pathBawah != null)
            {
                if (UnityEngine.Random.value > 0.5f)
                {
                    // Kirim komponen Transform dari kelas PathAtas ke script pergerakan tikus
                    movement.SetPath(pathAtas.transform); 
                }
                else
                {
                    // Kirim komponen Transform dari kelas PathBawah ke script pergerakan tikus
                    movement.SetPath(pathBawah.transform); 
                }
            }
            // OPSI B: LOGIKA FALLBACK SINGLE PATH (LEVEL 1)
            // Jika slot di Inspector kosong, otomatis cari objek tunggal bernama "Path1" di dalam runtime scene
            else
            {
                GameObject pathObj = GameObject.Find("Path1");
                if (pathObj != null)
                {
                    // Ambil komponen Transform induk utama milik Path1 untuk dibedah anaknya
                    movement.SetPath(pathObj.transform);
                }
                else
                {
                    Debug.LogError("Gagal Spawn! Jalur Level 2 kosong, dan objek 'Path1' Level 1 tidak ditemukan di Scene!");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Objek {obj.name} tidak memiliki komponen EnemyMovement!");
        }

        // 3. Set tingkat kekerasan (scaling darah/speed) tikus berdasarkan indeks Wave saat ini
        Enemy e = obj.GetComponent<Enemy>();
        if (e != null) e.SetDifficultyScale(waveIndex);

        // 4. AKTIFKAN OBJEK: Tikus keluar ke world map dalam kondisi posisi, jalur, dan skala data yang sudah final dan legal!
        obj.SetActive(true);

        Debug.Log($"<color=green>SPAWN: {type} | Wave {waveIndex + 1}</color>");
    }
}