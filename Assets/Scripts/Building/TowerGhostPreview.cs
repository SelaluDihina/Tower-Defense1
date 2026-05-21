using UnityEngine;

public class TowerGhostPreview : MonoBehaviour
{
    // Komponen untuk ngerender gambar bayangan towernya
    private SpriteRenderer spriteRenderer;
    
    [Header("Seting Warna Transparan")]
    [SerializeField] private Color validColor = new Color(0f, 1f, 0f, 0.4f);  // Hijau Transparan (A = 0.4f)
    [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.4f); // Merah Transparan (A = 0.4f)

    private void Awake()
    {
        // Ambil komponen SpriteRenderer dari objek ini saat game mulai berjalan
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Fungsi untuk mindahin bayangan ngikutin mouse dan ngubah warnanya
    public void MovePreview(Vector3 targetPosition, bool isValid)
    {
        // Kalau objeknya lagi mati/nonaktif, paksa aktifkan dulu
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        // Ubah posisi koordinat X dan Y objek ini sesuai posisi targetPosition (kursor mouse)
        transform.position = targetPosition;

        // Logika warna: JikaisValid true (di rumput) kasih warna hijau, jika false (di aspal) kasih merah
        spriteRenderer.color = isValid ? validColor : invalidColor;
    }

    // Fungsi untuk nyembunyiin bayangan kalau player batal beli
    public void HidePreview()
    {
        gameObject.SetActive(false);
    }
}