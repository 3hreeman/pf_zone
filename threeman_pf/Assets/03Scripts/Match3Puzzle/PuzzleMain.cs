using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleMain : MonoBehaviour {
    private const float TILE_SIZE = 0.64f;
    public GameObject tilePrefab; // Tile 프리팹을 연결합니다.
    private const int height = 10;
    private const int width = 5;
    private const int MatchLength = 3;
    private TileObject[,] tiles = new TileObject[width, height];
    public Sprite[] tileSprites;

    public Camera mainCam;

    void Start() {
        InitializeBoard();
        AdjustCameraPosition();
    }

    void InitializeBoard() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                var pos = GetTilePos(x, y);
                var tile = GenerateRandomTile(pos);
                tile.SetTileXY(x, y);
                tiles[x, y] = tile;
            }
        }
    }

    
    TileObject GenerateRandomTile(Vector2 pos) {
        GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity);
        var tile = tileObj.GetComponent<TileObject>();
        int tileType = Random.Range(0, tileSprites.Length);
        tile.SetTileType(tileType);
        return tile;
    }
    
    private TileObject GetTile(int x, int y) {
        if (tiles[x, y] != null) {
            return tiles[x, y];
        }

        return null;
    }

    private Vector2 GetTilePos(int x, int y) {
        return new Vector2(x * TILE_SIZE, y * TILE_SIZE);
    }
    
    void AdjustCameraPosition() {
        float boardWidth = width * TILE_SIZE;
        float boardHeight = height * TILE_SIZE;

        Vector2 boardCenter = new Vector2((boardWidth - TILE_SIZE) / 2, (boardHeight - TILE_SIZE) / 2);

        mainCam.transform.position = new Vector3(boardCenter.x, boardCenter.y, mainCam.transform.position.z);
    }

    public TileObject selectedTile = null;

    void Update() {
        HandleMouseInput();
        HandleMouseDrag();
    }

    void HandleMouseInput() {
        if (Input.GetMouseButtonDown(0)) {  // 왼쪽 마우스 버튼을 클릭했을 때
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("TileObject")) {
                TileObject tile = hit.collider.gameObject.GetComponent<TileObject>();
                if (selectedTile == null) {
                    tile.SelectTile();
                    selectedTile = tile;
                }
                else if (selectedTile == tile) {
                    tile.DeselectTile();
                    selectedTile = null;
                }
                else {
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
    void HandleMouseDrag() {
        if (selectedTile != null) {
            if (Input.GetMouseButtonDown(0)) {
                startDragPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0)) {
                endDragPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
                SwapTiles();
                selectedTile.DeselectTile();
                selectedTile = null;
            }
        }
    }

    void SwapTiles() {
        if (Vector2.Distance(endDragPosition, startDragPosition) < 0.1f) {
            Debug.Log("mouse drag too short");
            return;
        }

        Vector2 direction = endDragPosition - startDragPosition;
        TileObject targetTile = null;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) //좌우 움직임이 상하 움직임보다 클때 - 
        {
            // Horizontal swipe
            if (direction.x > 0)
                targetTile = GetAdjacentTile(selectedTile, 1, 0);
            else
                targetTile = GetAdjacentTile(selectedTile, -1, 0);
        }
        else {
            // Vertical swipe
            if (direction.y > 0)
                targetTile = GetAdjacentTile(selectedTile, 0, 1);
            else
                targetTile = GetAdjacentTile(selectedTile, 0, -1);
        }

        if (targetTile != null) {
            StartCoroutine(SwapTileProcess(selectedTile, targetTile));
        }
    }

    IEnumerator SwapTileProcess(TileObject tile1, TileObject tile2) {
        /*
        StartCoroutine(TranslateTile(tile1, tile2.xIndex, tile2.yIndex));
        yield return TranslateTile(tile2, tile1.xIndex, tile1.yIndex);
        */
        var x1 = tile1.xIndex;
        var y1 = tile1.yIndex;
        var x2 = tile2.xIndex;
        var y2 = tile2.yIndex;
        StartCoroutine(TranslateTile(tile1, x2, y2));
        yield return TranslateTile(tile2, x1, y1);

        tiles[x1, y1] = tile2;
        tiles[x2, y2] = tile1;

        var removeList = new List<TileObject>();
        var matched_1 = CheckForMatchesAt(tiles[tile1.xIndex, tile1.yIndex]);
        if (matched_1.Count >= 3) {
            foreach (var tile in matched_1) {
                removeList.Add(tile);
            }
        }

        var matched_2 = CheckForMatchesAt(tiles[tile2.xIndex, tile2.yIndex]);
        if (matched_2.Count >= 3) {
            foreach (var tile in matched_2) {
                removeList.Add(tile);
            }
        }

        if (removeList.Count > 0) {
            foreach (var tile in removeList) {
                tiles[tile.xIndex, tile.yIndex] = null;
                Destroy(tile.gameObject);
            }
            // Debug.LogWarning("Swap tile: " + tile1.xIndex + "," + tile1.yIndex + " <-> " + tile2.xIndex + "," + tile2.yIndex);
            yield return FillEmptySpaces();
        }
        else {
            //return to original position
            Debug.LogWarning("Not Matched. Return to original position");
            x1 = tile1.xIndex;
            y1 = tile1.yIndex;
            x2 = tile2.xIndex;
            y2 = tile2.yIndex;
            StartCoroutine(TranslateTile(tile1, x2, y2));
            yield return TranslateTile(tile2, x1, y1);
            tiles[x1, y1] = tile2;
            tiles[x2, y2] = tile1;
        }
    }


    IEnumerator TranslateTile(TileObject tile, int x, int y) {
        Vector2 targetPos = GetTilePos(x, y);
        float time = 0;
        float duration = 0.2f;
        while (time < duration) {
            time += Time.deltaTime;
            float t = time / duration;
            var nextPos = Vector2.Lerp(tile.transform.position, targetPos, t);
            tile.transform.position = nextPos;
            yield return null;
        }

        tile.transform.position = targetPos;
        tile.SetTileXY(x, y);
    }

    IEnumerator FillEmptySpaces() {
        for(int y=0; y<height-1; y++) {
            bool isFilled = false;
            for(int x=0; x<width; x++) {
                if(tiles[x,y] == null) {
                    for(int up_y = y+1; up_y<height; up_y++) {
                        if(tiles[x,up_y] != null) {
                            tiles[x,y] = tiles[x,up_y];
                            tiles[x,up_y] = null;
                            StartCoroutine(TranslateTile(tiles[x,y], x, y));
                            isFilled = true;
                            break;
                        }
                    }
                }
            }

            if (isFilled) {
                // yield return new WaitForSeconds(0.1f);
                yield return null;
            }
        }
        
        // Create new tiles in the empty spaces at the top
        for (int x = 0; x < width; x++) {
            for (int y = height - 1; y >= 0; y--) {
                if (tiles[x, y] == null) {
                    var pos = GetTilePos(x, y);
                    var newTile = GenerateRandomTile(pos);
                    newTile.SetTileXY(x, y);
                    tiles[x, y] = newTile;
                }
                else {
                    break;
                }
            }
        }
        
    }

    TileObject GetAdjacentTile(TileObject tile, int x, int y) {
        var targetX = tile.xIndex + x;
        var targetY = tile.yIndex + y;
        if (targetX >= 0 && targetX < width && targetY >= 0 && targetY < height) {
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
        for (int i = xIndex + 1; i < width; i++) {
            if(GetTile(i, yIndex) == null) break;
            if (tiles[i, yIndex].tileType == tiles[xIndex, yIndex].tileType) {
                horizontalMatches.Add(tiles[i, yIndex]);
            }
            else
                break;
        }

        for (int i = xIndex - 1; i >= 0; i--) {
            if (GetTile(i, yIndex) == null) break;
            if (tiles[i, yIndex].tileType == tiles[xIndex, yIndex].tileType) {
                horizontalMatches.Add(tiles[i, yIndex]);
            }
            else
                break;
        }

        if (horizontalMatches.Count >= 3) {
            matchedTiles.AddRange(horizontalMatches);
        }

        // Check vertically
        List<TileObject> verticalMatches = new List<TileObject>();
        verticalMatches.Add(tiles[xIndex, yIndex]);
        for (int i = yIndex + 1; i < height; i++) {
            if (GetTile(xIndex, i) == null) 
                break;
            if (tiles[xIndex, i].tileType == tiles[xIndex, yIndex].tileType) {
                verticalMatches.Add(tiles[xIndex, i]);
            }
            else
                break;
        }

        for (int i = yIndex - 1; i >= 0; i--) {
            if (tiles[xIndex, i] == null)
                break;
            if (tiles[xIndex, i].tileType == tiles[xIndex, yIndex].tileType) {
                verticalMatches.Add(tiles[xIndex, i]);
            }
            else
                break;
        }

        if (verticalMatches.Count >= 3) {
            matchedTiles.AddRange(verticalMatches);
        }
        
        return matchedTiles;
    }
}