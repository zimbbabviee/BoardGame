using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [Header("Board Settings")]
    [SerializeField] private GameObject tilePrefab; 
    [SerializeField] private int totalTiles = 20; 
    [SerializeField] private float tileSize = 1.0f; 

    [Header("Board Layout")]
    [SerializeField] private int tilesPerRow = 7; 

    [Header("Materials")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material jumpMaterial;
    [SerializeField] private Material fallMaterial;
    [SerializeField] private Material finishMaterial;

    private List<BoardTile> tiles = new List<BoardTile>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab не назначен в BoardManager!");
            return;
        }

        int totalRows = Mathf.CeilToInt((float)totalTiles / tilesPerRow);
        float boardWidth = (tilesPerRow - 1) * tileSize;
        float boardDepth = (totalRows - 1) * tileSize;

        Vector3 startPosition = transform.position - new Vector3(boardWidth / 2f, 0, -boardDepth / 2f);

        Debug.Log($"BoardManager позиция: {transform.position}, Стартовая позиция доски: {startPosition}, Размер доски: {boardWidth}x{boardDepth}");

        int currentRow = 0;
        int tileInRow = 0;
        bool reverseDirection = false; 

        for (int i = 0; i < totalTiles; i++)
        {
            Vector3 position = CalculateTilePosition(i, ref currentRow, ref tileInRow, ref reverseDirection, startPosition);

            GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
            tileObj.name = $"Tile_{i + 1}";

            BoardTile tile = tileObj.GetComponent<BoardTile>();
            if (tile == null)
            {
                tile = tileObj.AddComponent<BoardTile>();
            }

            tile.SetTileNumber(i + 1);

            AssignSpecialTiles(tile, i + 1);

            tiles.Add(tile);
        }

        Debug.Log($"Игровая доска создана: {totalTiles} клеток");
    }

    private Vector3 CalculateTilePosition(int index, ref int currentRow, ref int tileInRow, ref bool reverseDirection, Vector3 startPosition)
    {
        currentRow = index / tilesPerRow;
        tileInRow = index % tilesPerRow;

        reverseDirection = (currentRow % 2 != 0);

        float x, z;

        if (reverseDirection)
        {
            x = startPosition.x + (tilesPerRow - 1 - tileInRow) * tileSize;
        }
        else
        {
            x = startPosition.x + tileInRow * tileSize;
        }

        z = startPosition.z - currentRow * tileSize;

        return new Vector3(x, startPosition.y, z);
    }

    private void AssignSpecialTiles(BoardTile tile, int tileNumber)
    {
        if (tileNumber == totalTiles)
        {
            tile.SetTileType(TileType.Finish);
            return;
        }

        switch (tileNumber)
        {
            case 4:
                tile.SetTileType(TileType.Jump, 2); 
                break;
            case 7:
                tile.SetTileType(TileType.Fall, -3); 
                break;
            case 11:
                tile.SetTileType(TileType.Jump, 3); 
                break;
            case 16:
                tile.SetTileType(TileType.Fall, -2);
                break;
            default:
                tile.SetTileType(TileType.Normal);
                break;
        }
    }

    public BoardTile GetTile(int tileNumber)
    {
        if (tileNumber < 1 || tileNumber > tiles.Count)
        {
            Debug.LogWarning($"Клетка {tileNumber} вне диапазона!");
            return null;
        }

        return tiles[tileNumber - 1];
    }

    public int GetTotalTiles()
    {
        return totalTiles;
    }
    public void ClearBoard()
    {
        foreach (BoardTile tile in tiles)
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
            }
        }
        tiles.Clear();
    }
    public void RegenerateBoard()
    {
        ClearBoard();
        GenerateBoard();
    }
}
