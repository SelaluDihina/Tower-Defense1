using UnityEngine;
using UnityEngine.SceneManagement; // Library wajib buat urusan pindah-pindah alam (Scene)

public class EndGameController : MonoBehaviour
{
    // Fungsi buat tombol "Coba Lagi" (Retry)
    public void Retry()
    {
        // SceneManager.GetActiveScene().name ngambil nama scene yang lagi jalan sekarang
        // Gunanya biar kodingan lu fleksibel, kaga perlu ngetik nama scene manual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        // Reset Time.timeScale ke 1 karena biasanya pas Game Over waktu kita berhentiin (Pause)
        Time.timeScale = 1f; 
    }

    // Fungsi buat tombol "Kembali" (Back to Main Menu)
    public void BackToMainMenu()
    {
        // LoadScene(0) manggil scene Main Menu yang udah lu set di index 0 di Build Settings
        SceneManager.LoadScene(0);
        
        // Pastikan waktu jalan normal lagi sebelum pindah scene
        Time.timeScale = 1f;
    }
}