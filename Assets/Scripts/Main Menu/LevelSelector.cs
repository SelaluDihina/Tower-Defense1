using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("UI Level Buttons Array")]
    // Variabel ini WAJIB bertipe public atau memiliki [SerializeField] agar nongol di Inspector Canvas
    [SerializeField] private Button[] levelButtons;

private void Start()
    {
        // --- SUNTIKAN SAKTI TESTING (HARD MODE HACK) ---
        // Paksa registry laptop lu untuk membuka sampai level 3 demi kebutuhan debug/testing
        PlayerPrefs.SetInt("levelReached", 3); 
        // -----------------------------------------------

        // Ambil data progress level lokal dari registry laptop lu
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        // Validasi tombol level otomatis
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                // Matikan fungsi klik tombol level yang belum terbuka
                levelButtons[i].interactable = false;
            }
            else
            {
                // Pengaman: Pastikan tombol yang sudah terbuka bisa diklik dengan lancar
                levelButtons[i].interactable = true;
            }
        }
    }

    // Fungsi klik tombol untuk pindah ke scene level
    public void SelectLevel(string sceneName)
    {
        Time.timeScale = 1f; // Supaya game ga freeze pas masuk scene baru
        SceneManager.LoadScene(sceneName);
    }

    // Fungsi klik tombol exit pintu keluar
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Sesuaikan nama scene menu utama lu
    }
}