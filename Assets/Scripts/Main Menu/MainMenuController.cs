using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour 
{
    public void StartGame() 
    {
        // Pindah ke scene index 1 (Map Game lu)
        SceneManager.LoadScene(1); 
    }
}