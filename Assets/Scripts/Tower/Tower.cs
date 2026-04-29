using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TowerData data;         // Ngambil data tower (Range, Fire Rate, Damage)
    [Range(0.01f, 2f)] 
    [SerializeField] private float projectileScale = 0.05f; // [FUNGSI]: Ngecilin ukuran peluru (biar piso kaga kegedean)

    private List<Enemy> _enemiesInRange = new List<Enemy>(); // List buat nyatet musuh yang masuk jangkauan
    private ObjectPooler _projectitlePool;           // Referensi ke gudang peluru (Object Pooling)
    private float _shootTimer;                       // Timer internal buat ngatur kapan boleh nembak lagi

    private void Start()
    {
        _projectitlePool = GetComponent<ObjectPooler>(); // Inisialisasi pooler peluru
        if (data != null) _shootTimer = data.shootInterval; // Set timer awal sesuai data
    }

    private void Update()
    {
        if (data == null) return;
        _shootTimer -= Time.deltaTime;               // Kurangi timer tiap frame sesuai waktu nyata

        if (_enemiesInRange.Count == 0) CheckForEnemiesManual(); // [BACKUP]: Cek musuh manual kalau Trigger error

        if (_shootTimer <= 0)                        // Kalau timer abis (saatnya nembak)
        {
            _shootTimer = data.shootInterval;        // Reset timer ke interval awal
            Shoot();                                 // Panggil fungsi nembak
        }
    }

    private void CheckForEnemiesManual()
    {
        // [FUNGSI]: Sensor area lingkaran di sekitar tower buat cari tag "Enemy"
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, data.range);
        foreach (var c in colls)
        {
            if (c.CompareTag("Enemy"))
            {
                Enemy enemy = c.GetComponent<Enemy>();
                if (enemy != null && !_enemiesInRange.Contains(enemy))
                    _enemiesInRange.Add(enemy);       // Masukin musuh baru ke list radar
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))           // Kalau musuh masuk garis sensor (Circle Collider)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && !_enemiesInRange.Contains(enemy))
                _enemiesInRange.Add(enemy);           // Daftarkan musuh ke target potensial
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))           // Kalau musuh keluar dari garis sensor
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null) _enemiesInRange.Remove(enemy); // Hapus dari daftar target
        }
    }

    public void Shoot()
    {
        // [CLEANUP]: Bersihkan musuh yang mati, inactive, atau lari kejauhan (Anti-Sniper)
        _enemiesInRange.RemoveAll(e => 
            e == null || 
            !e.gameObject.activeInHierarchy || 
            Vector3.Distance(transform.position, e.transform.position) > data.range
        );

        if (_enemiesInRange.Count == 0) return;      // Kalau kaga ada musuh, batal nembak

        Enemy targetEnemy = _enemiesInRange[0];      // [TARGETING]: Pilih musuh yang paling duluan masuk (Priority)

        if (targetEnemy != null)
        {
            GameObject projGo = _projectitlePool.GetPooledObject(); // Ambil peluru dari gudang
            if (projGo != null)
            {
                projGo.transform.SetParent(null);    // Lepas peluru dari hirarki tower
                projGo.transform.position = transform.position; // Set posisi peluru di moncong tower
                projGo.transform.localScale = new Vector3(projectileScale, projectileScale, 1f); // Set ukuran peluru
                projGo.SetActive(true);              // Aktifkan peluru!

                Projectile projScript = projGo.GetComponent<Projectile>(); // Ambil script di pelurunya
                if (projScript != null) 
                    projScript.Shoot(data, targetEnemy.transform); // [FUNGSI INTI]: Kasih 'perintah' nembak ke peluru
            }
        }
    }

    private void OnDrawGizmosSelected()              // [EDITOR ONLY]: Buat nampilin jangkauan merah di Unity Editor
    {
        if (data == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }
}