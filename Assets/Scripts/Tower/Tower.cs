using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TowerData data;
    [Range(0.01f, 2f)]
    [SerializeField] private float projectileScale = 0.05f;

    private List<Enemy> _enemiesInRange = new List<Enemy>();
    private ObjectPooler _projectitlePool;
    private float _shootTimer;

    private void Start()
    {
        _projectitlePool = GetComponent<ObjectPooler>();
        if (data != null) _shootTimer = data.shootInterval;
    }

    private void Update()
    {
        if (data == null) return;
        _shootTimer -= Time.deltaTime;

        if (_enemiesInRange.Count == 0) CheckForEnemiesManual();

        if (_shootTimer <= 0)
        {
            _shootTimer = data.shootInterval;
            Shoot();
        }
    }

    private void CheckForEnemiesManual()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, data.range);
        foreach (var c in colls)
        {
            if (c.CompareTag("Enemy"))
            {
                Enemy enemy = c.GetComponent<Enemy>();
                // Cek null + cek duplikat sebelum masukin list
                if (enemy != null && !_enemiesInRange.Contains(enemy))
                    _enemiesInRange.Add(enemy);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && !_enemiesInRange.Contains(enemy))
                _enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null) _enemiesInRange.Remove(enemy);
        }
    }

    public void Shoot()
    {
        // Bersihin musuh yang udah mati, nonaktif, atau kabur dari range
        _enemiesInRange.RemoveAll(e =>
            e == null ||
            !e.gameObject.activeInHierarchy ||
            Vector3.Distance(transform.position, e.transform.position) > data.range
        );

        if (_enemiesInRange.Count == 0) return;

        Enemy targetEnemy = _enemiesInRange[0];
        if (targetEnemy == null) return;

        GameObject projGo = _projectitlePool.GetPooledObject();
        if (projGo == null) return;

        // [FIX UTAMA]: SetParent(null) DULU sebelum set posisi
        // Kalau peluru masih jadi child tower waktu di-set posisi,
        // scale tower (0.05f) ngefek ke world position peluru → peluru loncat ke posisi aneh
        projGo.SetActive(false);
        projGo.transform.SetParent(null);
        projGo.transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
        projGo.transform.localScale = new Vector3(projectileScale, projectileScale, 1f);
        projGo.SetActive(true);

        Projectile projScript = projGo.GetComponent<Projectile>();
        if (projScript != null)
            projScript.Shoot(data, targetEnemy.transform);
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }
}