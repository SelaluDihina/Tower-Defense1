using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text LivesText;

    private void OnEnable()
    {
        GameManager.OnLivesChanged += UpdateLivesText;
        // SIGNATURE HARUS MATCH: Dengerin event yang bawa 2 INT
        Spawner.OnWaveChanged += UpdateWaveText; 
    }  
    
    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLivesText;
    }

    // Fungsi nerima 2 INT (Sekarang, Total)
    private void UpdateWaveText(int currentWave, int totalWaves)
    {
        if (waveText != null)
        {
            // Pake String Interpolation biar kodingan lu sekelas PENS / Jepang!
            // Formatnya bakal jadi: "1 / 15"
            waveText.text = $"{currentWave} / {totalWaves}"; 
        }
    }

    private void UpdateLivesText(int CurrentLives)
    {
        if (LivesText != null)
        {
            // Cuma angka doang, biar icon hati di pita lu yang bicara!
            LivesText.text = CurrentLives.ToString();
        }
    }
}