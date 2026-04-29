using UnityEngine;

public class TowerButton : MonoBehaviour
{
    public GameObject towerPrefab; 
    public TowerData towerData;

    public void SelectThisTower()
    {
        // Pake != ya Riz, bukan simbol matematika ≠
        if (BuildManager.instance != null)
        {
            BuildManager.instance.towerPrefab = towerPrefab;
            BuildManager.instance.towerData = towerData;
            
            // Karena di TowerData gak ada towerName, kita log harganya aja buat penanda
            Debug.Log("Tower terpilih dengan harga: " + towerData.cost);
        }
        else
        {
            Debug.LogError("BuildManager mana woy? Belum ada di Hierarchy!");
        }
    }
}