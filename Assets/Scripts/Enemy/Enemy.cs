using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    [SerializeField] private Slider healthSlider;

    private float _currentHealth;
    private float _moveSpeed;

    // [FUNGSI INTI]: Dipanggil Spawner buat naikin status musuh
    public void SetDifficultyScale(int waveIndex)
    {
        if (data == null) return;

        // Scaling: HP +20% per wave, Speed +5% per wave
        float hpMultiplier = 1f + (waveIndex * 0.2f); 
        float speedMultiplier = 1f + (waveIndex * 0.05f);

        _currentHealth = data.lives * hpMultiplier;
        _moveSpeed = data.speed * speedMultiplier;

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

        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        // Balikin ke Pool, jangan di-Destroy!
        gameObject.SetActive(false);
    }
}