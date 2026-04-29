using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // [PENTING]: Ini kabel yang dicari GameManager lu!
    public static Action<Enemy> OnEnemyReachedEnd;
    public static Action<Enemy> OnEnemyDestroyed;

    [SerializeField] private EnemyData data;
    [SerializeField] private Slider healthSlider;

    private float _currentHealth;
    private float _moveSpeed;

    public void SetDifficultyScale(int waveIndex)
    {
        if (data == null) return;
        float hpMultiplier = 1f + (waveIndex * 0.2f); 
        float speedMultiplier = 1f + (waveIndex * 0.05f);

        _currentHealth = data.lives * hpMultiplier;
        _moveSpeed = data.speed * speedMultiplier;

        if (healthSlider != null) {
            healthSlider.maxValue = _currentHealth;
            healthSlider.value = _currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (healthSlider != null) healthSlider.value = _currentHealth;
        if (_currentHealth <= 0) {
            OnEnemyDestroyed?.Invoke(this); // Teriak: "Gue mati, kasih duit!"
            Die();
        }
    }

    // Panggil ini kalau musuh sampe ujung path
    public void ReachedEnd()
    {
        OnEnemyReachedEnd?.Invoke(this); // Teriak: "Gue lolos, kurangin nyawa player!"
        Die();
    }

    private void Die()
    {
        gameObject.SetActive(false); // Balikin ke pool
    }
}