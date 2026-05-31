// --- KODE REKONSILIASI KASTA SUCI (BALIK KE SETELAN AWAL LU YANG LURUS) ---
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Enemy _enemy;
    private Transform[] _waypoints;
    private int _waypointIndex = 0;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    // Fungsi pengoper jalur dari Spawner tanpa merusak urutan asli Hierarchy lu yang udah rapi!
    public void SetPath(Transform pathTransform)
    {
        if (pathTransform == null) return;

        // 1. Ambil anak sesuai urutan asli Hierarchy lu (karena emang dari awal udah lurus terus!)
        _waypoints = new Transform[pathTransform.childCount];
        for (int i = 0; i < pathTransform.childCount; i++)
        {
            _waypoints[i] = pathTransform.GetChild(i);
        }

        // 2. Reset index ke titik start awal jalur
        _waypointIndex = 0;

        // 3. Langsung kunci posisi awal tikus di belokan pertama biar gak lompat-lompat rute
        if (_waypoints.Length > 0 && _waypoints[0] != null)
        {
            transform.position = _waypoints[0].position;
        }
    }

    void OnEnable()
    {
        // Biarkan kosong total! JANGAN panggil UpdateRuntimePath ampas lagi biar gak amnesia!
    }

    void Update()
    {
        // Pengaman: Kalo data jalur belum masuk, jangan jalan dulu
        if (_waypoints == null || _waypoints.Length == 0) return;
        if (_waypointIndex >= _waypoints.Length) return;
        if (_waypoints[_waypointIndex] == null) return;

        // Ambil target koordinat belokan berikutnya
        Vector3 targetPos = _waypoints[_waypointIndex].position;

        // --- LOGIKA ROTASI HADAP TIKUS ---
        Vector3 direction = targetPos - transform.position;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // Jalankan fisik tikus maju lurus searah ke target
        transform.position = Vector2.MoveTowards(transform.position, targetPos, _enemy.MoveSpeed * Time.deltaTime);

        // Cek jarak, kalau udah nyampe belokan, ganti target ke belokan berikutnya
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            _waypointIndex++;

            if (_waypointIndex >= _waypoints.Length)
            {
                _enemy.ReachedEnd(); // Masuk finish, balik ke pooler
            }
        }
    }
}