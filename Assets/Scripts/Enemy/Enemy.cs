using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))] 
public class Enemy : MonoBehaviour
{
    public static Action<Enemy> OnEnemyReachedEnd;
    public static Action<Enemy> OnEnemyDestroyed;

    [Header("Visuals & HP")]
    [SerializeField] private EnemyData data;
    [SerializeField] private Slider healthSlider;

    [Header("Audio Settings (Modular)")]
    [SerializeField] private AudioClip hitSound; 

    private float _currentHealth;
    private float _moveSpeed;
    private float _baseSpeed;
    private float _slowTimer;
    private bool _isSlowed;
    private AudioSource _audioSource; 

    public float MoveSpeed => _moveSpeed;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource != null) _audioSource.playOnAwake = false;
    }

    public void SetDifficultyScale(int waveIndex)
    {
        if (data == null) return;
        _currentHealth = data.lives * (1f + waveIndex * 0.2f);
        _baseSpeed = data.speed;
        _moveSpeed = _baseSpeed;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = _currentHealth;
            healthSlider.value = _currentHealth;
        }
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        // --- CONSOLE LOG TOWER LEM ---
        Debug.Log($"<color=yellow>[TOWER LEM]</color> Musuh {gameObject.name} TERKENA SLOW! Speed turun ke: {1f - slowAmount}%");

        _moveSpeed = _baseSpeed * (1f - slowAmount);
        _slowTimer = duration;
        _isSlowed  = true;
    }

    private void Update()
    {
        if (!_isSlowed) return;

        _slowTimer -= Time.deltaTime;
        if (_slowTimer <= 0)
        {
            Debug.Log($"<color=white>[SYSTEM]</color> Efek Slow pada {gameObject.name} SELESAI.");
            _moveSpeed = _baseSpeed; 
            _isSlowed  = false;
        }
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (healthSlider != null) healthSlider.value = _currentHealth;

        if (_audioSource != null && hitSound != null)
        {
            _audioSource.PlayOneShot(hitSound); 
        }

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
        Projectile[] attachedProjectiles = GetComponentsInChildren<Projectile>(true);
        foreach (Projectile proj in attachedProjectiles)
        {
            proj.gameObject.SetActive(false); 
        }

        _isSlowed = false; // Reset state biar pool kaga glitch
        gameObject.SetActive(false);
    }
}