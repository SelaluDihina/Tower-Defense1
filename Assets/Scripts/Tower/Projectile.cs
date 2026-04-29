using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Modular Visual Adjustments (Hard Mode PENS)")]
    // [PENTING RIZ!]: Tweak angka ini di Inspector Prefab Peluru lu.
    // Jangan diubah di kode, tapi di Unity! Misal isi -90, 90, atau 180.
    // Tujuannya biar ujung garpunya beneran hadap depan pas terbang.
    [SerializeField] private float rotationOffset = 0f;

    private Transform _target;
    private TowerData _data;

    public void Shoot(TowerData data, Transform target)
    {
        _data = data;
        _target = target;
    }

    void Update()
    {
        // 1. Cek Target (Kalau mati/ilang, peluru harus mati)
        if (_target == null || !_target.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false); // Balik ke Pool
            return;
        }

        // --- 2. LOGIKA modular ROKET (HOMING) - Logic tetep ngejar Riz! ---
        // Kita pake speed dari TowerData (modular). Pastiin di data tower Is Artillery UNCHECK!
        float currentSpeed = (_data != null) ? _data.projectileSpeed : 15f;
        transform.position = Vector2.MoveTowards(transform.position, _target.position, currentSpeed * Time.deltaTime);


        // --- 3. LOGIKA modular ROTASI (Arah Ujung Tajam - BIAR NUSUK) ---
        // Kita hitung arah ke target
        Vector3 direction = _target.position - transform.position;
        // Cari sudut angle-nya
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // [KUNCI FIX]: Kita tambahin offset biar asset visual sinkron ama kodingan Unity.
        // Ini TDP Modular Hard Mode!
        transform.rotation = Quaternion.AngleAxis(angle + rotationOffset, Vector3.forward);


        // --- 4. DETEKSI TABRAKAN (Direct Hit) ---
        // Jarak threshold kita buat 0.15 unit
        if (Vector2.Distance(transform.position, _target.position) < 0.15f)
        {
            Enemy enemy = _target.GetComponent<Enemy>();
            if (enemy != null)
            {
                // [PENS REQUIREMENT]:uses damage dari TowerData (modular)
                enemy.TakeDamage(_data.damage); 
            }
            gameObject.SetActive(false); // Balik ke pool (Ilang)
        }
    }
}