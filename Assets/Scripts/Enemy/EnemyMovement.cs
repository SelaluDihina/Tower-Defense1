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

    void OnEnable()
    {
        _waypointIndex = 0; // Reset index buat Object Pooler
    }

    void Update()
    {
        // 1. Cek apa jalanannya ada
        if (_waypoints == null || _waypointIndex >= _waypoints.Length) return;

        // 2. Tentukan target posisi
        Vector3 targetPos = _waypoints[_waypointIndex].position;

        // 3. LOGIKA BALIK BADAN (Tengok Kanan/Kiri)
        if (targetPos.x > transform.position.x) 
        {
            transform.localScale = new Vector3(-0.1f, 0.1f, 1f); // Ngadep Kanan
        }
        else 
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 1f); // Ngadep Kiri
        }

        // 4. GERAK MENUJU TARGET
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