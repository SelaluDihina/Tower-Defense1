using UnityEngine;

public class NodeSpawner : MonoBehaviour
{
    public GameObject nodePrefab; 
    public Vector2Int gridSize;    
    public float spacing = 1f;     

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