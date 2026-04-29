using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Visual Link")]
    // [PENTING]: Tarik objek PaluBangun ke slot ini di Inspector nanti!
    [SerializeField] private GameObject hammerIcon; 

    private GameObject tower;

    void OnMouseDown()
    {
        // LOG 1: Mastiin klik masuk ke script ini
        Debug.Log("<color=green>● [NODE] Klik masuk ke: </color>" + gameObject.name);

        if (tower != null)
        {
            Debug.Log("<color=yellow>● [NODE] Ubin udah ada towernya!</color>");
            return;
        }

        // Panggil BuildManager
        Debug.Log("<color=white>● [NODE] Minta BuildManager buat bangun...</color>");
        
        // Asumsi BuildManager.instance.BuildTowerOn balikin GameObject tower yang baru jadi
        tower = BuildManager.instance.BuildTowerOn(this);
        
        if(tower != null) {
            Debug.Log("<color=cyan>● [NODE] BuildManager lapor: BERHASIL!</color>");
            
            // --- LOGIKA MODULAR PENS: MATIIN PALU ---
            if (hammerIcon != null) {
                hammerIcon.SetActive(false); // Palu "pamit" undur diri
                Debug.Log("<color=magenta>● [NODE] Visual Palu dimatikan!</color>");
            }
            
            // Opsional: Matiin sprite kotak induknya biar bersih
            GetComponent<SpriteRenderer>().enabled = false;
        } else {
            Debug.Log("<color=red>● [NODE] BuildManager lapor: GAGAL!</color>");
        }
    }
}