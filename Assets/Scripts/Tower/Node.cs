using UnityEngine;

public class Node : MonoBehaviour
{
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
        tower = BuildManager.instance.BuildTowerOn(this);
        
        if(tower != null) {
            Debug.Log("<color=cyan>● [NODE] BuildManager lapor: BERHASIL!</color>");
        } else {
            Debug.Log("<color=red>● [NODE] BuildManager lapor: GAGAL!</color>");
        }
    }
}