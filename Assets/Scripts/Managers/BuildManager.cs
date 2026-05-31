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
    
    [SerializeField] private LayerMask grassLayer; 
    private Color validColor = new Color(0f, 1f, 0f, 0.4f);  
    private Color invalidColor = new Color(1f, 0f, 0f, 0.4f); 
    
    [Header("Tower Limits")]
    [SerializeField] private int maxTowers = 10; 
    private int _currentTowerCount = 0; 

    private void Awake()
    {
        instance = this;
        if (ghostPreview == null) return;
        ghostRenderer = ghostPreview.GetComponent<SpriteRenderer>();
        if (ghostPreview.transform.childCount > 0)
            rangeIndicator = ghostPreview.transform.GetChild(0);
        ghostPreview.SetActive(false);
    }

    // --- FUNGSI SINKRONISASI WARNA (KASTA DEWA OOP) ---
    private void SetPreviewColor(Color newColor)
    {
        // Ubah warna tower kecil
        ghostRenderer.color = newColor;
        
        // Ubah juga warna lingkaran jangkauannya secara bersamaan!
        if (rangeIndicator != null)
        {
            SpriteRenderer circleRenderer = rangeIndicator.GetComponent<SpriteRenderer>();
            if (circleRenderer != null) circleRenderer.color = newColor;
        }
    }

    private void Update()
    {
        if (ghostPreview == null || !ghostPreview.activeSelf) return;

        // Netralkan distorsi Z kamera
        Vector3 rawMousePos = Input.mousePosition;
        rawMousePos.z = Mathf.Abs(Camera.main.transform.position.z); 
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(rawMousePos);
        mousePos.z = 0f; 

        ghostPreview.transform.position = new Vector3(mousePos.x, mousePos.y, -1f);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);

        if (hit.collider != null)
        {
            // 1. BLOKIR MUTLAK AREA JALAN MUSUH (ASPAL/PATH)
            if (hit.collider.name.Contains("Aspal") || hit.collider.name.Contains("Path") || hit.collider.name.Contains("Jalan"))
            {
                SetPreviewColor(invalidColor); // Sinkronisasi: Tower & Lingkaran jadi MERAH!
                return; 
            }

            Node node = hit.collider.GetComponent<Node>();
            
            // 2. KASTA LEVEL 1 (Petak Kayu Node)
            if (node != null)
            {
                if (node.tower == null) SetPreviewColor(validColor);
                else SetPreviewColor(invalidColor);
                
                ghostPreview.transform.position = new Vector3(node.transform.position.x, node.transform.position.y, -1f);
            }
            // 3. KASTA LEVEL 2 (Tanah Raksasa Tilemap)
            else if (hit.collider.name.Contains("Grid") || hit.collider.name.Contains("Tanah") || hit.collider.gameObject.layer == LayerMask.NameToLayer("TanahBangun"))
            {
                float snapX = Mathf.Floor(mousePos.x) + 0.5f;
                float snapY = Mathf.Floor(mousePos.y) + 0.5f;
                Vector3 snapPos = new Vector3(snapX, snapY, -1f);
                
                // Radar Anti-Tumpuk
                Collider2D[] overlaps = Physics2D.OverlapCircleAll(new Vector2(snapX, snapY), 0.2f);
                bool areaPenuh = false;

                foreach (var col in overlaps)
                {
                    if (col.gameObject != hit.collider.gameObject && !col.isTrigger)
                    {
                        areaPenuh = true; 
                        break; 
                    }
                }

                if (!areaPenuh) SetPreviewColor(validColor);
                else SetPreviewColor(invalidColor);

                ghostPreview.transform.position = snapPos;
            }
            else
            {
                SetPreviewColor(invalidColor);
            }
        }
        else
        {
            SetPreviewColor(invalidColor);
        }

        if (Input.GetMouseButtonDown(1)) CancelSelection();
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
        if (prefabSR != null) ghostRenderer.sprite = prefabSR.sprite;
        UpdateRangeIndicator(data.range);
    }

    public void CancelSelection()
    {
        if (ghostPreview != null) ghostPreview.SetActive(false);
        towerPrefab = null;
        towerData = null;
    }

    public GameObject BuildTowerOn(Node node)
    {
        if (towerPrefab == null || towerData == null) return null;
        if (_currentTowerCount >= maxTowers) return null;
        if (PlayerStats.Money < towerData.cost) return null;

        PlayerStats.Money -= towerData.cost;
        _currentTowerCount++; 

        Vector3 spawnPos = new Vector3(node.transform.position.x, node.transform.position.y, -1f);
        Instantiate(towerPrefab, spawnPos, Quaternion.identity);

        CancelSelection();
        return towerPrefab; // Hanya untuk return GameObject jika diminta node
    }

    private void UpdateRangeIndicator(float range)
    {
        if (rangeIndicator == null) return;
        float parentScale = ghostPreview.transform.localScale.x;
        float finalScale = (range * 2f) / parentScale;
        rangeIndicator.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}