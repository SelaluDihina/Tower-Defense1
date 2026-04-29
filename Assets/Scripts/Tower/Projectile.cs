using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Modular Visual")]
    [SerializeField] private float rotationOffset = 0f;

    [Header("Tower Lem Settings")]
    [SerializeField] private bool isGlueProjectile = false; // Checklist ini di Prefab Tower Lem!
    [SerializeField] private float slowAmount = 0.5f;
    [SerializeField] private float slowDuration = 2f;
    [SerializeField] private float splashRadius = 1.5f; // Radius area Lem

    private Transform _target;
    private TowerData _data;

    public void Shoot(TowerData data, Transform target)
    {
        _data = data;
        _target = target;
    }

    void Update()
    {
        if (_target == null || !_target.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false); 
            return;
        }

        float currentSpeed = (_data != null) ? _data.projectileSpeed : 15f;
        transform.position = Vector2.MoveTowards(transform.position, _target.position, currentSpeed * Time.deltaTime);

        Vector3 direction = _target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + rotationOffset, Vector3.forward);

        if (Vector2.Distance(transform.position, _target.position) < 0.15f)
        {
            HandleHit();
        }
    }

    private void HandleHit()
    {
        if (isGlueProjectile)
        {
            // --- LOGIKA AOE (AREA) LEM ---
            Debug.Log($"<color=cyan>[AOE]</color> Proyektil Lem Meledak di Area!");
            
            // Cari semua musuh di radius splash
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, splashRadius);
            foreach (var col in hitEnemies)
            {
                Enemy e = col.GetComponent<Enemy>();
                if (e != null)
                {
                    e.ApplySlow(slowAmount, slowDuration);
                    e.TakeDamage(_data.damage);
                }
            }
        }
        else
        {
            // Tembakan biasa (Garpu/Piso)
            Enemy enemy = _target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_data.damage); 
            }
        }

        gameObject.SetActive(false);
    }
    private AudioSource _audioSource;

void Awake()
{
    _audioSource = GetComponent<AudioSource>();
}

void OnEnable()
{
    // RESET STATE & PARENT
    transform.SetParent(null); 

    // --- PAKSA SUARA BUNYI TIAP KALI MUNCUL ---
    if (_audioSource != null && _audioSource.clip != null)
    {
        _audioSource.Stop(); // Stop dulu kalo ada sisa suara
        _audioSource.Play(); // Bunyiin lagi dari awal
        Debug.Log($"<color=green>[AUDIO]</color> {gameObject.name} nembak!");
    }
}
}