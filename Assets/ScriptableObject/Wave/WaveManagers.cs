using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManagers : MonoBehaviour
{
    [Header("Wave Settings")]
    // Menyimpan daftar data wave (jenis musuh, jumlah, dll) yang bisa diatur dari Inspector
    [SerializeField] private WaveData[] waves; 
    // Jeda waktu (detik) antar wave setelah wave sebelumnya selesai spawn
    [SerializeField] private float timeBetweenWaves = 5f; 
    
    // Index untuk melacak wave keberapa yang sedang atau akan berjalan
    private int _currentWaveIndex = 0;
    // Flag/Saklar untuk mencegah timer berjalan saat proses spawning musuh sedang berlangsung
    private bool _isSpawning = false;
    // Timer mundur yang akan dieksekusi setiap frame
    private float _countdown = 5f; 
    // Menyimpan detik terakhir yang di-log ke console agar tidak terjadi spam log setiap frame
    private int _lastLoggedTime = -1; 

    // Referensi ke script Spawner untuk memanggil fungsi pooling musuh
    private Spawner _spawner;
    // Referensi ke GameManager untuk melaporkan status akhir permainan
    private GameManager _gameManager;

    void Start()
    {
        // Mencari komponen Spawner pada object yang sama untuk efisiensi pemanggilan fungsi
        _spawner = GetComponent<Spawner>();
        // Mencari object GameManager di scene untuk koordinasi status wave terakhir
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        // [LOGIKA GERBANG]: Jika musuh sedang keluar (spawning), hentikan semua logika di bawahnya
        if (_isSpawning) return;

        // [LOGIKA COUNTDOWN CONSOLE]: Menampilkan hitung mundur di Console secara rapi
        if (_countdown > 0f && _currentWaveIndex < waves.Length)
        {
            // Mathf.CeilToInt membulatkan angka ke atas (misal 4.2 jadi 5) agar visual timer lebih natural
            int currentSecond = Mathf.CeilToInt(_countdown);

            // Cek apakah detiknya sudah berganti dari detik sebelumnya yang dicatat
            if (currentSecond != _lastLoggedTime)
            {
                Debug.Log($"<color=yellow>[COOLDOWN]</color> Wave {_currentWaveIndex + 1} dalam: {currentSecond}...");
                // Kunci detik sekarang agar tidak dipanggil lagi di frame selanjutnya (ngerem spam log)
                _lastLoggedTime = currentSecond;
            }
        }

        // [LOGIKA EKSEKUSI]: Jika waktu habis, jalankan wave baru
        if (_countdown <= 0f)
        {
            // Pastikan index wave tidak melebihi jumlah data wave yang ada
            if (_currentWaveIndex < waves.Length)
            {
                // Menjalankan Coroutine (fungsi yang bisa berjalan secara async/paralel dengan waktu)
                StartCoroutine(SpawnWave());
                // Reset timer kembali ke waktu jeda antar wave
                _countdown = timeBetweenWaves; 
                // Reset pengunci log agar hitung mundur wave berikutnya bisa muncul
                _lastLoggedTime = -1;
            }
        }

        // Mengurangi nilai timer berdasarkan waktu nyata (detik), bukan berdasarkan frame rate
        _countdown -= Time.deltaTime;
    }

    // Fungsi IEnumerator memungkinkan penggunaan 'yield return' untuk memberikan jeda waktu di tengah loop
    IEnumerator SpawnWave()
    {
        // Kunci status spawning agar Update() tidak menjalankan timer baru
        _isSpawning = true;
        
        // Ambil data wave saat ini dari array berdasarkan index
        WaveData currentWave = waves[_currentWaveIndex];

        // Memicu Event (jika ada script lain yang subscribe) untuk update UI nomor wave
        Spawner.OnWaveChanged?.Invoke(_currentWaveIndex + 1, waves.Length); 

        Debug.Log($"<color=red>WAVE {_currentWaveIndex + 1} DIMULAI!</color>");

        // Loop melalui setiap kelompok musuh (misal: Kelompok A, lalu Kelompok B)
        foreach (var group in currentWave.enemyGroups)
        {
            // Loop sebanyak jumlah musuh dalam satu kelompok tersebut
            for (int i = 0; i < group.count; i++)
            {
                // Memanggil musuh dari Pool memori (pooling) agar tidak membebani CPU/RAM secara berlebih
                if (_spawner != null) _spawner.ActivateFromPool(group.enemyType, _currentWaveIndex);
                
                // Memberikan jeda antar individu musuh saat keluar agar tidak menumpuk di satu titik
                yield return new WaitForSeconds(group.spawnInterval);
            }
            // Jeda tambahan 1 detik antar kelompok musuh yang berbeda dalam satu wave
            yield return new WaitForSeconds(1f); 
        }

        // Melepas kunci status spawning setelah semua musuh dalam wave tersebut keluar
        _isSpawning = false;
        // Menaikkan index untuk menunjuk ke wave berikutnya
        _currentWaveIndex++;

        // [LOGIKA HARD MODE]: Cek apakah ini adalah wave terakhir yang dikirim
        if (_currentWaveIndex >= waves.Length) 
        {
            if (_gameManager != null) 
            {
                // Lapor ke GameManager bahwa semua musuh sudah di-spawn, tinggal tunggu mereka mati semua
                _gameManager.SetAllWavesSpawned(); 
                Debug.Log("<color=orange>[WAVE] Laporan: Semua wave sudah keluar! Fokus bantai sisa musuh!</color>"); 
            }
        }
    }
}