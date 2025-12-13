using UnityEngine;

public class BoardTile : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField] private int tileNumber; 
    [SerializeField] private TileType tileType = TileType.Normal;

    [Header("Special Tile Settings")]
    [Tooltip("Для клеток типа Jump или Fall - насколько клеток переместиться (+ вперед, - назад)")]
    [SerializeField] private int moveOffset = 0;

    [Header("Visual")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material jumpMaterial;
    [SerializeField] private Material fallMaterial; 
    [SerializeField] private Material finishMaterial; 

    private MeshRenderer meshRenderer;

    public int TileNumber => tileNumber;
    public TileType Type => tileType;
    public int MoveOffset => moveOffset;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisuals();
    }
    public void SetTileNumber(int number)
    {
        tileNumber = number;
    }
    public void SetTileType(TileType type, int offset = 0)
    {
        tileType = type;
        moveOffset = offset;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (meshRenderer == null) return;

        switch (tileType)
        {
            case TileType.Normal:
                if (normalMaterial != null)
                    meshRenderer.material = normalMaterial;
                break;
            case TileType.Jump:
                if (jumpMaterial != null)
                    meshRenderer.material = jumpMaterial;
                break;
            case TileType.Fall:
                if (fallMaterial != null)
                    meshRenderer.material = fallMaterial;
                break;
            case TileType.Finish:
                if (finishMaterial != null)
                    meshRenderer.material = finishMaterial;
                break;
        }
    }
    public int ProcessTileEffect()
    {
        switch (tileType)
        {
            case TileType.Jump:
                Debug.Log($"Клетка {tileNumber}: Прыжок вперед на {moveOffset} клеток!");
                return moveOffset;
            case TileType.Fall:
                Debug.Log($"Клетка {tileNumber}: Падение назад на {Mathf.Abs(moveOffset)} клеток!");
                return moveOffset;
            case TileType.Finish:
                Debug.Log($"Клетка {tileNumber}: ФИНИШ!");
                return 0;
            default:
                return 0;
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position + Vector3.up * 0.5f;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up, $"Tile {tileNumber}");

        if (tileType == TileType.Jump && moveOffset > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.forward * moveOffset);
        }
        else if (tileType == TileType.Fall && moveOffset < 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.back * Mathf.Abs(moveOffset));
        }
    }
#endif
}
public enum TileType
{
    Normal, 
    Jump,    
    Fall,    
    Finish   
}
