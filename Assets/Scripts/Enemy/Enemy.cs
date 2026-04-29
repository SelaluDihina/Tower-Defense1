using System;
using UnityEngine;
using UnityEngine.UI;

// [PENS REQUIREMENT]: Maksa objek ini punya AudioSource
[RequireComponent(typeof(AudioSource))] 
public class Enemy : MonoBehaviour
{
    public static Action<Enemy> OnEnemyReachedEnd;
    public static Action<Enemy> OnEnemyDestroyed;

    [Header("Visuals & HP")]
    [SerializeField] private EnemyData data;
    [SerializeField] private Slider healthSlider;

    [Header("Audio Settings (Modular)")]
    [SerializeField] private AudioClip hitSound; // Suara pas kena garpu

    private float _currentHealth;
    private float _moveSpeed;
    private AudioSource _audioSource; // Variabel AudioSource modular

    public float MoveSpeed => _moveSpeed;

    private void Awake()
    {
        // [FIX BUMBU PENS]: Cari component AudioSource di objek ini
        _audioSource = GetComponent<AudioSource>();
        // Matiin Play On Awake biar kaga teriak pas spawn
        if (_audioSource != null) _audioSource.playOnAwake = false;
    }

    public void SetDifficultyScale(int waveIndex)
    {
        if (data == null) return;
        _currentHealth = data.lives * (1f + waveIndex * 0.2f);
        _moveSpeed = data.speed;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = _currentHealth;
            healthSlider.value = _currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (healthSlider != null) healthSlider.value = _currentHealth;

        // --- LOGIKA modular AUDIO PAS KENA HIT ---
        if (_audioSource != null && hitSound != null)
        {
            // Pake PlayOneShot biar suaranya tumpuk-tumpuk kalau kena hit beruntun
            _audioSource.PlayOneShot(hitSound); 
        }
        // ----------------------------------------

        if (_currentHealth <= 0)
        {
            OnEnemyDestroyed?.Invoke(this);
            Die();
        }
    }

    public void ReachedEnd()
    {
        OnEnemyReachedEnd?.Invoke(this);
        Die();
    }

    private void Die()
    {
        // [FIX PENS]: Bersihin garpu yang nancep biar kaga melayang
        Projectile[] attachedProjectiles = GetComponentsInChildren<Projectile>(true);
        foreach (Projectile proj in attachedProjectiles)
        {
            proj.gameObject.SetActive(false); 
        }

        gameObject.SetActive(false);
    }
}