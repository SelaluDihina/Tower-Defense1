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