using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public float range = 5f;
    public float shootInterval = 1f;
    public float projectileSpeed = 10f;
    public float projectileDuration = 3f;
    public int damage = 10;
    public int cost = 50;
    public float slowMultiplier = 0.5f; 
    public float slowDuration = 2f;
    // Tambahin ini di TowerData.cs lu
    [Header("AOE Settings")]
    public bool isAOE = true;             // Centang ini buat Tower Lem
    public float splashRadius = 2.0f;     // Radius ledakan lem (misal 2 meter)
    // Tambahin ini di TowerData.cs lu Riz
    [Header("Artillery/Lob Settings")]
    public bool isArtillery = true;       // Centang buat Tower Lem
    public float arcHeight = 5.0f;        // Seberapa tinggi peluru melambung ke atas
    public float travelTime = 1.5f;       // Berapa detik peluru sampe target (Speed kaga dipake)    
}
