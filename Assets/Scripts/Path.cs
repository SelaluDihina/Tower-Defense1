using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Path : MonoBehaviour
{
    // List buat naruh semua titik tujuan di Inspector
    public GameObject[] Waypoints;

    // GPS: Kasih tau Enemy koordinat titik ke-sekian
    public Vector3 GetPosition(int index)
    {
        return Waypoints[index].transform.position;
    }

    private void OnDrawGizmos()
    {
        if (Waypoints.Length > 0)
        {
            for (int i = 0 ; i < Waypoints.Length; i++)
            {
                // Kasih NAMA melayang di atas titik biar gak ketuker
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(Waypoints[i].transform.position + Vector3.up * 0.7f, Waypoints[i].name, style);

                // Tarik GARIS abu-abu biar jalurnya kelihatan nyambung
                if (i < Waypoints.Length - 1)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(Waypoints[i].transform.position, Waypoints[i + 1].transform.position);
                }  
            }
        }
    }
}