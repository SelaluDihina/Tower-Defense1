using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmationUI : MonoBehaviour
{
    public static ConfirmationUI Instance;

    [SerializeField] private GameObject panel; // Drag object Panel ke sini di Inspector

    private Action onConfirm;
    private Action onCancel;

    private void Awake()
    {
        Instance = this;
        // Langsung matiin panel pas game start biar gak nongol terus
        if (panel != null) panel.SetActive(false); 
    }

    // Fungsi sakti buat manggil UI dari mana aja
    public void ShowConfirmation(Action confirmAction, Action cancelAction)
    {
        onConfirm = confirmAction;
        onCancel = cancelAction;
        
        panel.SetActive(true); // Munculkan UI
        Time.timeScale = 0f;    // Freeze game biar gak dicolong tikus pas lagi mikir
    }

    public void KlikYa()
    {
        onConfirm?.Invoke();
        TutupUI();
    }

    public void KlikTidak()
    {
        onCancel?.Invoke();
        TutupUI();
    }

    private void TutupUI()
    {
        panel.SetActive(false); // Sembunyiin lagi
        Time.timeScale = 1f;    // Jalanin lagi gamenya
    }
}