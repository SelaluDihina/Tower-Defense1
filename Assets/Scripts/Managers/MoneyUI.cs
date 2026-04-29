using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour {
    public TextMeshProUGUI moneyText;

    void Update() {
        // Hapus tulisan "Gold: " biar cuma angkanya aja yang nongol
        moneyText.text = PlayerStats.Money.ToString();
    }
}