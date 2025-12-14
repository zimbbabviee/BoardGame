using UnityEngine;
using TMPro;

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
    private TextMeshPro stepText;

    public int TileNumber => tileNumber;
    public TileType Type => tileType;
    public int MoveOffset => moveOffset;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        CreateStepText();
        UpdateVisuals();
    }

    private void CreateStepText()
    {
        GameObject textObj = new GameObject("StepText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0, 0.6f, 0);
        textObj.transform.localRotation = Quaternion.Euler(90, 0, 0);

        stepText = textObj.AddComponent<TextMeshPro>();
        stepText.alignment = TextAlignmentOptions.Center;
        stepText.fontSize = 1.5f;
        stepText.fontStyle = FontStyles.Bold;
        stepText.text = "";
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
                ApplyBorderedMaterial(Color.white);
                UpdateStepText("");
                break;
            case TileType.Jump:
                if (jumpMaterial != null)
                    meshRenderer.material = jumpMaterial;
                UpdateStepText($"+{moveOffset}", Color.black);
                break;
            case TileType.Fall:
                if (fallMaterial != null)
                    meshRenderer.material = fallMaterial;
                UpdateStepText($"{moveOffset}", Color.black);
                break;
            case TileType.Finish:
                ApplyCheckerboardMaterial();
                UpdateStepText("", Color.yellow);
                break;
        }
    }

    private void ApplyBorderedMaterial(Color fillColor)
    {
        int size = 32;
        int borderWidth = 2;
        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool isBorder = x < borderWidth || x >= size - borderWidth ||
                               y < borderWidth || y >= size - borderWidth;
                texture.SetPixel(x, y, isBorder ? Color.black : fillColor);
            }
        }
        texture.Apply();

        Material borderedMat = new Material(Shader.Find("Standard"));
        borderedMat.mainTexture = texture;
        meshRenderer.material = borderedMat;
    }

    private void ApplyCheckerboardMaterial()
    {
        int size = 8;
        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool isWhite = (x + y) % 2 == 0;
                texture.SetPixel(x, y, isWhite ? Color.white : Color.black);
            }
        }
        texture.Apply();

        Material checkerMat = new Material(Shader.Find("Standard"));
        checkerMat.mainTexture = texture;
        checkerMat.mainTextureScale = new Vector2(4, 4);
        meshRenderer.material = checkerMat;
    }

    private void UpdateStepText(string text, Color? color = null)
    {
        if (stepText != null)
        {
            stepText.text = text;
            if (color.HasValue)
            {
                stepText.color = color.Value;
            }
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
