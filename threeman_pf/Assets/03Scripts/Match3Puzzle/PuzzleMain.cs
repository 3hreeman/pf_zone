using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleMain : MonoBehaviour
{
    private const float TILE_SIZE = 0.64f;
    public GameObject tilePrefab; // Tile 프리팹을 연결합니다.
    private const int Rows = 8;
    private const int Columns = 8;
    private const int MatchLength = 3;
    private TileObject[,] tiles = new TileObject[Rows, Columns];
    public Sprite[] tileSprites;

    public Camera mainCam;
    void Start()
    {
        InitializeBoard();
        AdjustCameraPosition();
    }

    void InitializeBoard()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject tileObj = Instantiate(tilePrefab, new Vector2(j * TILE_SIZE, i * TILE_SIZE), Quaternion.identity);
                var tile = tileObj.GetComponent<TileObject>();
                int tileType = Random.Range(0, tileSprites.Length);
                tile.Init(i, j, tileType, tileSprites[tileType]);
                tiles[i, j] = tile;
            }
        }
    }
    
    void AdjustCameraPosition()
    {
        float boardWidth = Columns * TILE_SIZE;
        float boardHeight = Rows * TILE_SIZE;

        Vector2 boardCenter = new Vector2((boardWidth - TILE_SIZE) / 2, (boardHeight - TILE_SIZE) / 2);
    
        mainCam.transform.position = new Vector3(boardCenter.x, boardCenter.y, mainCam.transform.position.z);
    }

    public TileObject selectedTile = null;

    void Update()
    {
        HandleMouseInput();
        HandleMouseDrag();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 버튼을 클릭했을 때
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            
            if (hit.collider != null && hit.collider.gameObject.CompareTag("TileObject"))
            {
                TileObject tile = hit.collider.gameObject.GetComponent<TileObject>();
                if (selectedTile == null)
                {
                    tile.SelectTile();
                    selectedTile = tile;
                }
                else if (selectedTile == tile)
                {
                    tile.DeselectTile();
                    selectedTile = null;
                }
                else
                {
                    selectedTile.DeselectTile();
                    tile.SelectTile();
                    selectedTile = tile;
                }
            }
        }
    }
    
    private Vector2 startDragPosition;
    private Vector2 endDragPosition;

// Update 함수 내에 기존 HandleMouseInput() 호출 밑에 추가
    void HandleMouseDrag()
    {
        if (selectedTile != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startDragPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                endDragPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
                SwapTiles();
                selectedTile.DeselectTile();
                selectedTile = null;
            }
        }
    }

    void SwapTiles()
    {
        if (Vector2.Distance(endDragPosition, startDragPosition) < 0.1f) {
            Debug.Log("mouse drag too short");
            return;
        }
        
        Vector2 direction = endDragPosition - startDragPosition;
        TileObject targetTile = null;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal swipe
            if (direction.x > 0)
                targetTile = GetAdjacentTile(selectedTile, Vector2Int.up);
            else
                targetTile = GetAdjacentTile(selectedTile, Vector2Int.down);
        }
        else
        {
            // Vertical swipe
            if (direction.y > 0)
                targetTile = GetAdjacentTile(selectedTile, Vector2Int.right);
            else
                targetTile = GetAdjacentTile(selectedTile, Vector2Int.left);
        }

        if (targetTile != null)
        {
            StartCoroutine(SwapTileAnim(selectedTile, targetTile));
        }
    }

    IEnumerator SwapTileAnim(TileObject tile1, TileObject tile2)
    {
        Debug.LogWarning("Swap tile: "+tile1.xIndex + ","+tile1.yIndex+" <-> "+tile2.xIndex + ","+tile2.yIndex);
        Vector3 selectDest = tile2.transform.position;
        Vector3 targetDest = tile1.transform.position;
        float time = 0;
        float duration = 0.2f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            tile1.transform.position = Vector3.Lerp(tile1.transform.position, selectDest, t);
            tile2.transform.position = Vector3.Lerp(tile2.transform.position, targetDest, t);
            yield return null;
        }
        tile1.transform.position = selectDest;
        tile2.transform.position = targetDest;

        tiles[tile1.xIndex, tile1.yIndex] = tile2;
        tiles[tile2.xIndex, tile2.yIndex] = tile1;
        tile1.SwapTile(tile2);
        
        var matched_1 = CheckForMatchesAt(tiles[tile1.xIndex, tile1.yIndex]);
        if (matched_1.Count >= 3) {
            foreach(var tile in matched_1) {
                Destroy(tile.gameObject);
            }
        }
        
        var matched_2 = CheckForMatchesAt(tiles[tile2.xIndex, tile2.yIndex]);
        if (matched_2.Count >= 3) {
            foreach(var tile in matched_2) {
                Destroy(tile.gameObject);
            }
        }
    }
    
    TileObject GetAdjacentTile(TileObject tile, Vector2Int dir)
    {
        var targetX = tile.xIndex + dir.x;
        var targetY = tile.yIndex + dir.y;
        if (targetX >= 0 && targetX < Columns && targetY >= 0 && targetY < Rows)
        {
            return tiles[targetX, targetY];
        }

        return null;
    }
    
    List<TileObject> CheckForMatchesAt(TileObject tile) {
        var xIndex = tile.xIndex;
        var yIndex = tile.yIndex;
        
        List<TileObject> matchedTiles = new List<TileObject>();

        // Check horizontally
        List<TileObject> horizontalMatches = new List<TileObject>();
        horizontalMatches.Add(tiles[xIndex, yIndex]);
        for (int i = xIndex + 1; i < Columns; i++)
        {
            if (tiles[i, yIndex].tileType == tiles[xIndex, yIndex].tileType)
            {
                horizontalMatches.Add(tiles[i, yIndex]);
            }
            else
                break;
        }
        for (int i = xIndex - 1; i >= 0; i--)
        {
            if (tiles[i, yIndex].tileType == tiles[xIndex, yIndex].tileType)
            {
                horizontalMatches.Add(tiles[i, yIndex]);
            }
            else
                break;
        }
        if (horizontalMatches.Count >= 3)
        {
            matchedTiles.AddRange(horizontalMatches);
        }

        // Check vertically
        List<TileObject> verticalMatches = new List<TileObject>();
        verticalMatches.Add(tiles[xIndex, yIndex]);
        for (int i = yIndex + 1; i < Rows; i++) {
            if (tiles[xIndex, i].tileType == tiles[xIndex, yIndex].tileType) {
                verticalMatches.Add(tiles[xIndex, i]);
            }
            else
                break;
        }
        for (int i = yIndex - 1; i >= 0; i--) {
            if (tiles[xIndex, i].tileType == tiles[xIndex, yIndex].tileType) {
                verticalMatches.Add(tiles[xIndex, i]);
            }
            else
                break;
        }

        if (verticalMatches.Count >= 3)
        {
            matchedTiles.AddRange(verticalMatches);
        }

        return matchedTiles;
    }
}