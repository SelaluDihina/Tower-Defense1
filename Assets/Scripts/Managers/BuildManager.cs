using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    // =========================================================
    // SINGLETON
    // =========================================================
    public static BuildManager instance;

    // =========================================================
    // TOWER LIBRARY
    // =========================================================
    [Header("Tower Library")]
    public GameObject garpuPrefab;
    public TowerData   garpuData;

    public GameObject pisoPrefab;
    public TowerData   pisoData;

    public GameObject lemPrefab;
    public TowerData   lemData;

    // =========================================================
    // SELEKSI AKTIF
    // =========================================================
    [Header("Current Selection")]
    public GameObject towerPrefab;
    public TowerData  towerData;

    // =========================================================
    // GHOST PREVIEW
    // =========================================================
    [Header("Ghost Preview Settings")]
    public GameObject ghostPreview;

    private SpriteRenderer ghostRenderer;
    private Transform      rangeIndicator;
    
    // fitur max towr berapa
    [Header("Tower Limits")]
    [SerializeField] private int maxTowers = 5; // Lu mau batesin berapa?
    private int _currentTowerCount = 0;
    
    public void BuildTower(GameObject towerPrefab, Vector3 position)
{
    // [LOGIKA HARD MODE]: Cek apa towernya udah kebanyakan
    if (_currentTowerCount >= maxTowers)
    {
        Debug.Log("Woy! Tower udah maksimal, kaga bisa bangun lagi!");
        return; 
    }

    // Kalau masih aman, baru bangun
    Instantiate(towerPrefab, position, Quaternion.identity);
    _currentTowerCount++;
}
    // =========================================================
    // UNITY CALLBACKS
    // =========================================================
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

        Vector3 mousePos   = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z         = -1f;
        ghostPreview.transform.position = mousePos;

        if (Input.GetMouseButtonDown(1))
            CancelSelection();
    }

    // =========================================================
    // TOMBOL UI — PILIH TOWER
    // =========================================================
    public void Button_PilihGarpu() => SelectTowerToBuild(garpuPrefab, garpuData);
    public void Button_PilihPiso()  => SelectTowerToBuild(pisoPrefab,  pisoData);
    public void Button_PilihLem()   => SelectTowerToBuild(lemPrefab,   lemData);

    // =========================================================
    // LOGIKA SELEKSI
    // =========================================================
    public void SelectTowerToBuild(GameObject prefab, TowerData data)
    {
        if (prefab == null || data == null) return;

        towerPrefab = prefab;
        towerData   = data;

        if (ghostPreview == null) return;

        ghostPreview.SetActive(true);

        SpriteRenderer prefabSR = prefab.GetComponent<SpriteRenderer>();
        if (prefabSR != null)
            ghostRenderer.sprite = prefabSR.sprite;

        UpdateRangeIndicator(data.range);

        Debug.Log($"<color=cyan>SIAAP! {data.name} dipilih. Range: {data.range}</color>");
    }

    public void CancelSelection()
    {
        if (ghostPreview != null)
            ghostPreview.SetActive(false);

        towerPrefab = null;
        towerData   = null;

        Debug.Log("Seleksi dibatalkan.");
    }

    // =========================================================
    // LOGIKA MEMBANGUN — dipanggil dari Node.cs
    // =========================================================
    public GameObject BuildTowerOn(Node node)
    {
        if (towerPrefab == null || towerData == null) return null;

        if (PlayerStats.Money < towerData.cost)
        {
            Debug.Log("<color=red>MISKIN! Duit kurang!</color>");
            return null;
        }

        PlayerStats.Money -= towerData.cost;

        Vector3 spawnPos = new Vector3(node.transform.position.x,
                                       node.transform.position.y,
                                       -1f);

        GameObject tower = Instantiate(towerPrefab, spawnPos, Quaternion.identity);

        Debug.Log($"<color=yellow>BOOM! {towerData.name} BERHASIL DI-SPAWN!</color>");

        CancelSelection();
        return tower;
    }

    // =========================================================
    // HELPER PRIVATE
    // =========================================================
    private void UpdateRangeIndicator(float range)
    {
        if (rangeIndicator == null) return;

        float parentScale = ghostPreview.transform.localScale.x;
        float finalScale  = (range * 2f) / parentScale;

        rangeIndicator.localScale = new Vector3(finalScale, finalScale, 1f);

        SpriteRenderer circleSR = rangeIndicator.GetComponent<SpriteRenderer>();
        if (circleSR == null) return;

        circleSR.color           = new Color(0f, 1f, 0f, 0.7f);
        circleSR.material        = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
        circleSR.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
    }
}