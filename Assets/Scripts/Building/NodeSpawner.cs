using UnityEngine;

public class NodeSpawner : MonoBehaviour
{
    [Header("Node & Grid Settings")]
    [Tooltip("Prefab objek node tunggal yang akan di-instantiate")]
    public GameObject nodePrefab; 
    [Tooltip("Ukuran matriks grid (X untuk lebar, Y untuk tinggi)")]
    public Vector2Int gridSize;    
    [Tooltip("Jarak antar node di dalam arena")]
    public float spacing = 1f;     

    [Header("Ghost Preview System")]
    [SerializeField] private TowerGhostPreview ghostPreview; 
    [SerializeField] private LayerMask grassLayer; 

    // Variabel internal untuk kontrol logika tracking mouse (Private)
    private Sprite selectedTowerSprite; 
    private bool isPreparingToBuild = false;

    private void Update()
    {
        // KONDISI: Jika player/AI sedang memilih lokasi untuk membangun tower
        if (isPreparingToBuild)
        {
            // 1. Ambil koordinat kursor mouse di dunia game 2D
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // Kunci koordinat Z di angka 0 agar tetap berada di area 2D

            // 2. Tembakkan sinar ghaib (Raycast2D) tepat di posisi kursor mouse
            // Sinar ini dikhususkan hanya mendeteksi objek yang berada di dalam "grassLayer"
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, grassLayer);

            // 3. Validasi Kasta Tanah
            if (hit.collider != null)
            {
                // Jika mengenai area legal (rumput/node), pindahkan hantu ke titik hit dan beri warna HIJAU (true)
                ghostPreview.MovePreview(hit.point, true);
            }
            else
            {
                // Jika mengenai area ilegal (aspal/luar map), pindahkan hantu mengikuti mouse dan beri warna MERAH (false)
                ghostPreview.MovePreview(mousePos, false);
            }
        }
        else
        {
            // Jika sedang tidak dalam fase memilih/membeli tower, paksa objek hantu bersembunyi
            if (ghostPreview != null) ghostPreview.HidePreview();
        }
    }

    /// <summary>
    /// Fungsi untuk mentrigger sistem Ghost Preview aktif dari UI Button
    /// </summary>
    public void StartGhostPreview(Sprite towerSprite)
    {
        isPreparingToBuild = true;
        selectedTowerSprite = towerSprite;
        
        if (ghostPreview != null)
        {
            ghostPreview.GetComponent<SpriteRenderer>().sprite = selectedTowerSprite;
        }
    }

    /// <summary>
    /// Fungsi untuk mematikan Ghost Preview setelah tower berhasil dibangun atau dibatalkan
    /// </summary>
    public void StopGhostPreview()
    {
        isPreparingToBuild = false;
        if (ghostPreview != null) ghostPreview.HidePreview();
    }

    [ContextMenu("Spawn Nodes")] 
    public void SpawnNodes()
    {
        // 1. Bersihin sisa node lama
        foreach (Transform child in transform) {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () => {
                if (child != null) DestroyImmediate(child.gameObject);
            };
            #endif
        }

        // 2. Loop Spawning dengan Penamaan Spesifik
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 spawnPos = new Vector3(x * spacing, y * spacing, 0) + transform.position;
                GameObject newNode = Instantiate(nodePrefab, spawnPos, Quaternion.identity, transform);
                
                // --- PENAMAAN OTOMATIS BERDASARKAN KOORDINAT ---
                newNode.name = $"Node_{x}_{y}"; 
            }
        }
        Debug.Log($"Nodes Berhasil Di-automasi! Total: {gridSize.x * gridSize.y} nodes.");
    }
}