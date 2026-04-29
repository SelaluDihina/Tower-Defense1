using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 5;
    private List<GameObject> _pool;

    void Start()
    {
        _pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
            CreateNewObject();
    }

    private GameObject CreateNewObject()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab di ObjectPooler kosong! Seret prefabnya.");
            return null;
        }

        // [FIX UTAMA]: Parent = null → peluru spawn di root scene, bukan child tower
        // Kalau jadi child tower, scale tower (misal 0.05f) nge-warp world position peluru
        // dan bikin dia muncul di posisi yang salah waktu di-detach
        GameObject obj = Instantiate(prefab, null);
        obj.SetActive(false);
        _pool.Add(obj);
        return obj;
    }

    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in _pool)
        {
            // Cek null dulu biar aman kalau ada objek yang ke-destroy
            if (obj != null && !obj.activeInHierarchy)
                return obj;
        }

        // Pool habis? Bikin objek baru (auto-expand)
        return CreateNewObject();
    }
}