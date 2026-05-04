using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Visual Link")]
    [SerializeField] private GameObject hammerIcon; 

    private GameObject tower;

    void OnMouseDown()
    {
        Debug.Log("<color=green>● [NODE] Klik masuk ke: </color>" + gameObject.name);

        if (tower != null)
        {
            Debug.Log("<color=yellow>● [NODE] Ubin udah ada towernya!</color>");
            return;
        }

        Debug.Log("<color=white>● [NODE] Minta BuildManager buat bangun...</color>");
        
        tower = BuildManager.instance.BuildTowerOn(this);
        
        if(tower != null) 
        {
            Debug.Log("<color=cyan>● [NODE] BuildManager lapor: BERHASIL!</color>");
            
            if (hammerIcon != null) hammerIcon.SetActive(false);
            
            // SAFETY CHECK: Hanya matiin SpriteRenderer kalau emang ada komponennya
            SpriteRenderer rend = GetComponent<SpriteRenderer>();
            if (rend != null) 
            {
                rend.enabled = false;
            }
        } 
        else 
        {
            Debug.Log("<color=red>● [NODE] BuildManager lapor: GAGAL!</color>");
        }
    }
}