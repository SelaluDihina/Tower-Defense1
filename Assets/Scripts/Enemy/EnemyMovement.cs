using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Enemy _enemy;
    private Transform[] _waypoints;
    private int _waypointIndex = 0;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        
        // Cari folder Path1 di Hierarchy
        GameObject pathObj = GameObject.Find("Path1");
        if (pathObj != null)
        {
            _waypoints = new Transform[pathObj.transform.childCount];
            for (int i = 0; i < pathObj.transform.childCount; i++)
            {
                _waypoints[i] = pathObj.transform.GetChild(i);
            }
        }
    }

    // Reset index tiap kali keluar dari pool
    void OnEnable()
    {
        _waypointIndex = 0; // Reset target index

        // [FIX SPAWN]: Paksa posisi tikus ke Waypoint(0) pas baru muncul
        if (_waypoints != null && _waypoints.Length > 0)
        {
            transform.position = _waypoints[0].position;
        }
    }

    void Update()
    {
        // 1. Cek apa jalanannya ada
        if (_waypoints == null || _waypointIndex >= _waypoints.Length) return;

        // 2. Tentukan target posisi waypoint sekarang
        Vector3 targetPos = _waypoints[_waypointIndex].position;

        // --- 3. LOGIKA modular HADAP (TANPA MJ) ---
        // Sesuai request lu, kita hapus logic flipping (hadap kiri/kanan).
        // Kita cuma pake logika rotation dasar biar dia selalu nengok ke target.
        // Asumsi: Sprite tikus lu aslinya hadap Kanan.
        Vector3 direction = targetPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // ------------------------------------------

        // 4. GERAK LURUS MENUJU TARGET (Direct Move)
        // Gerak pake MoveSpeed dari script Enemy yang udah dikali DifficultyScale
        transform.position = Vector2.MoveTowards(transform.position, targetPos, _enemy.MoveSpeed * Time.deltaTime);

        // 5. CEK JARAK (Kalau udah deket, ganti waypoint)
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            _waypointIndex++;

            // 6. CEK APA UDAH NYAMPE BASE LU
            if (_waypointIndex >= _waypoints.Length)
            {
                _enemy.ReachedEnd(); // Kurangi nyawa player & balik ke pool
            }
        }
    }
}