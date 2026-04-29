using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour {
    public TextMeshProUGUI livesText;

    void Update() {
        // Asumsi lu punya static variable 'Lives' di PlayerStats
        livesText.text = PlayerStats.Lives.ToString();
    }
}