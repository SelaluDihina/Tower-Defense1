using UnityEngine;

public enum ProjectileType { DamageOnly, SlowEffect }

public class Projectile : MonoBehaviour
{
    public ProjectileType type;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private TowerData _data;
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _timeElapsed;
    private bool _isInitialized = false;

    public void Shoot(TowerData data, Transform target)
    {
        _data = data;
        _startPos = transform.position;
        _targetPos = target.position; // Catat koordinat buat Artillery
        _timeElapsed = 0f;
        _isInitialized = true;
    }

    void Update()
    {
        if (!_isInitialized || _data == null) return;

        _timeElapsed += Time.deltaTime;
        float progress = _timeElapsed / _data.travelTime;

        if (progress >= 1f) { Explode(); return; }

        if (_data.isArtillery) {
            // Logika Melambung (Parabola)
            Vector3 pos = Vector3.Lerp(_startPos, _targetPos, progress);
            float arc = 4f * _data.arcHeight * progress * (1f - progress);
            pos.y += arc; 
            transform.position = pos;
        } else {
            // Logika Lurus (Homing)
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _data.projectileSpeed * Time.deltaTime);
        }
    }

    private void Explode()
    {
        if (_data.isAOE) {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, _data.splashRadius);
            foreach (var col in targets) {
                if (col.CompareTag("Enemy")) {
                    Enemy e = col.GetComponent<Enemy>();
                    if (e != null) e.TakeDamage(_data.damage);
                }
            }
        }
        gameObject.SetActive(false);
    }
}