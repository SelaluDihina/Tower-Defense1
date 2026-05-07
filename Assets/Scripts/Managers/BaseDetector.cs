using UnityEngine;

// 1. Memaksa objek untuk memiliki komponen Collider2D
[RequireComponent(typeof(Collider2D))]
public class BaseDetector : MonoBehaviour
{
    // 2. Fungsi bawaan Unity untuk mendeteksi tabrakan (sensor)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 3. Mengambil script 'Enemy' dari objek yang menabrak sensor ini
        Enemy enemy = other.GetComponent<Enemy>();

        // 4. Memastikan bahwa objek yang menabrak benar-benar adalah musuh
        if (enemy != null)
        {
            // 5. Memberikan sinyal ke musuh bahwa dia sudah sampai tujuan
            enemy.ReachedEnd();

            // 6. Log ke Console untuk keperluan debugging
            Debug.Log($"<color=red>[SENSOR GUDANG]</color> Tikus {other.name} berhasil menembus pertahanan!");
        }
    }
}