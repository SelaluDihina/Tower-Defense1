using UnityEngine;

public class MouseDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Tembak laser dari mouse ke dunia game
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("<color=cyan>MOUSE NABRAK: </color>" + hit.collider.gameObject.name + 
                          " | Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
            }
            else
            {
                Debug.Log("<color=red>MOUSE GAK NABRAK APA-APA!</color>");
            }
        }
    }
}