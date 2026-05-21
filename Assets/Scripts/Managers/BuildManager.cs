using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    [Header("Tower Library")]
    public GameObject garpuPrefab;
    public TowerData garpuData;
    public GameObject pisoPrefab;
    public TowerData pisoData;
    public GameObject lemPrefab;
    public TowerData lemData;

    [Header("Current Selection")]
    public GameObject towerPrefab;
    public TowerData towerData;

    [Header("Ghost Preview Settings")]
    public GameObject ghostPreview;
    private SpriteRenderer ghostRenderer;
    private Transform rangeIndicator;
    // --- SUNTIKAN SAKTI HARD MODE (TIDAK MENGHAPUS KODE LAMA) ---
    [SerializeField] private LayerMask grassLayer; // Kolom baru buat nampung layer "Node_Tower" lu
    private Color validColor = new Color(0f, 1f, 0f, 0.4f);  // Hijau Transparan
    private Color invalidColor = new Color(1f, 0f, 0f, 0.4f); // Merah Transparan
    // ------------------------------------------------------------
    
    [Header("Tower Limits")]
    [SerializeField] private int maxTowers = 10; 
    private int _currentTowerCount = 0; // Variabel internal buat ngitung tower

    private void Awake()
    {
        instance = this;
        if (ghostPreview == null) return;
        ghostRenderer = ghostPreview.GetComponent<SpriteRenderer>();
        if (ghostPreview.transform.childCount > 0)
            rangeIndicator = ghostPreview.transform.GetChild(0);
        ghostPreview.SetActive(false);
    }

    private void Update()
    {
        if (ghostPreview == null || !ghostPreview.activeSelf) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -1f;
        ghostPreview.transform.position = mousePos;

        // --- ARSITEKTUR DETEKSI LAYER TANAH (SUNTIKAN BARU TANPA HAPUS) ---
        // Tembakkan sinar 2D dari koordinat mouse untuk ngecek kasta layer tanah bebas lu
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, grassLayer);

        if (hit.collider != null)
        {
            // Jika mengenai objek yang memiliki Layer "Node_Tower" (Legal/Bisa dibangun)
            ghostRenderer.color = validColor;
        }
        else
        {
            // Jika mengenai aspal orange atau area kosong luar map (Ilegal/Gak bisa dibangun)
            ghostRenderer.color = invalidColor;
        }
        // ------------------------------------------------------------------

        if (Input.GetMouseButtonDown(1))
            CancelSelection();
    }

    public void Button_PilihGarpu() => SelectTowerToBuild(garpuPrefab, garpuData);
    public void Button_PilihPiso() => SelectTowerToBuild(pisoPrefab, pisoData);
    public void Button_PilihLem() => SelectTowerToBuild(lemPrefab, lemData);

    public void SelectTowerToBuild(GameObject prefab, TowerData data)
    {
        if (prefab == null || data == null) return;
        towerPrefab = prefab;
        towerData = data;
        if (ghostPreview == null) return;
        ghostPreview.SetActive(true);
        SpriteRenderer prefabSR = prefab.GetComponent<SpriteRenderer>();
        if (prefabSR != null)
            ghostRenderer.sprite = prefabSR.sprite;
        UpdateRangeIndicator(data.range);
        Debug.Log($"<color=cyan>SIAAP! {data.name} dipilih.</color>");
    }

    public void CancelSelection()
    {
        if (ghostPreview != null) ghostPreview.SetActive(false);
        towerPrefab = null;
        towerData = null;
    }

    // LOGIKA MEMBANGUN UTAMA
    public GameObject BuildTowerOn(Node node)
    {
        if (towerPrefab == null || towerData == null) return null;

        // CEK LIMIT: Stop pembangunan kalau sudah mencapai maxTowers
        if (_currentTowerCount >= maxTowers)
        {
            Debug.Log("<color=red>LIMIT! Tower udah 10, Riz! Jangan maruk!</color>");
            return null;
        }

        if (PlayerStats.Money < towerData.cost)
        {
            Debug.Log("<color=red>MISKIN! Duit kurang!</color>");
            return null;
        }

        PlayerStats.Money -= towerData.cost;
        _currentTowerCount++; // Tambah hitungan tower SETELAH pengecekan lolos

        Vector3 spawnPos = new Vector3(node.transform.position.x, node.transform.position.y, -1f);
        GameObject tower = Instantiate(towerPrefab, spawnPos, Quaternion.identity);

        Debug.Log($"<color=yellow>BOOM! {towerData.name} ke-{_currentTowerCount} BERHASIL!</color>");

        CancelSelection();
        return tower;
    }

    private void UpdateRangeIndicator(float range)
    {
        if (rangeIndicator == null) return;
        float parentScale = ghostPreview.transform.localScale.x;
        float finalScale = (range * 2f) / parentScale;
        rangeIndicator.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}