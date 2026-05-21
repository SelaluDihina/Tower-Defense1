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

    // Fungsi cerdik buat ngecek ketersediaan jalur di setiap level
    public void UpdateRuntimePath()
    {
        // 1. LOGIKA LEVEL 2: Kalau Enemy punya data jalur dinamis atas/bawah dari Spawner
        if (_enemy != null && _enemy.Waypoints != null && _enemy.Waypoints.Length > 0)
        {
            _waypoints = _enemy.Waypoints;
            _waypointIndex = 0;

            if (_waypoints[0] != null)
            {
                transform.position = _waypoints[0].position;
            }
        }
        // 2. LOGIKA LEVEL 1 (SOLUSI TABRAKAN): Kalo data atas/bawah kosong, otomatis cari "Path1" tunggal!
        else
        {
            GameObject pathObj = GameObject.Find("Path1");
            if (pathObj != null)
            {
                _waypoints = new Transform[pathObj.transform.childCount];
                for (int i = 0; i < pathObj.transform.childCount; i++)
                {
                    _waypoints[i] = pathObj.transform.GetChild(i);
                }
                
                _waypointIndex = 0;
                
                if (_waypoints.Length > 0 && _waypoints[0] != null)
                {
                    transform.position = _waypoints[0].position;
                }
            }
        }
    }

    void OnEnable()
    {
        _waypointIndex = 0; 
        // Deteksi jalur langsung pas tikus keluar dari Object Pooler
        UpdateRuntimePath();
    }

    void Update()
    {
        // Pengaman darurat: kalo di frame awal datanya sempat miss, paksa cari lagi
        if (_waypoints == null || _waypoints.Length == 0)
        {
            UpdateRuntimePath();
            return;
        }

        if (_waypointIndex >= _waypoints.Length) return;
        if (_waypoints[_waypointIndex] == null) return;

        // Tentukan koordinat target
        Vector3 targetPos = _waypoints[_waypointIndex].position;

        // --- LOGIKA ROTASI HADAP TIKUS (Pertahankan Kode Asli Lu) ---
        Vector3 direction = targetPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // -------------------------------------------------------------

        // Eksekusi pergerakan tikus
        transform.position = Vector2.MoveTowards(transform.position, targetPos, _enemy.MoveSpeed * Time.deltaTime);

        // Cek jarak antar waypoint
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            _waypointIndex++;

            if (_waypointIndex >= _waypoints.Length)
            {
                _enemy.ReachedEnd(); // Tikus masuk finish, nyawa kurang, balik ke pool
            }
        }
    }
}