using System.Collections;
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
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        if (prefab == null)
        {
            Debug.LogError("Riz, prefab di ObjectPooler kosong! Seret prefabnya dulu.");
            return null;
        }

        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        _pool.Add(obj);
        return obj;
    }

    public GameObject GetPooledObject()
    {
        // Pake for loop atau foreach, tapi tambahin cek null (obj != null)
        foreach(GameObject obj in _pool)
        {
            // Pastiin objeknya masih eksis dan lagi gak aktif
            if (obj != null && !obj.activeInHierarchy)
            {
                return obj; 
            }
        }

        // Kalau semua peluru lagi dipake atau ada yang hancur, bikin baru
        return CreateNewObject();
    }
}