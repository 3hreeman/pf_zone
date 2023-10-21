using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleMain : MonoBehaviour {
    private const float TILE_SIZE = 0.64f;
    private const float TILE_MOVE_SPD = 0.5f;
    
    public GameObject tilePrefab; // Tile 프리팹을 연결합니다.
    public int height = 10;
    public int width = 5;
    private const int MatchLength = 3;
    private TileObject[,] _tileGrid;

    public Camera mainCam;

    void Start() {
        InitializeBoard(5, 5);
        AdjustCameraPosition();
    }

    void InitializeBoard(int w, int h) {
        width = w;
        height = h;
        _tileGrid = new TileObject[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                var pos = GetTilePos(x, y);
                var isObstacle = Random.Range(0, 10) < 2;
                TileObject tile;
                if (isObstacle) {
                    tile = GenerateObstacleTile(pos);
                }
                else {
                    tile = GenerateNormalTile(pos);
                }
                tile.SetTileXY(x, y);
                _tileGrid[x, y] = tile;
            }
        }
    }

    TileObject GenerateNormalTile(Vector2 pos) {
        GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity);
        var tile = tileObj.GetComponent<TileObject>();
        int sprIdx = Random.Range(0, 4);
        tile.SetNormalTile(sprIdx);
        return tile;
    }

    TileObject GenerateObstacleTile(Vector2 pos) {
        GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity);
        var tile = tileObj.GetComponent<TileObject>();
        int id = 0;
        tile.SetObstacleTile(id);
        return tile;    
    }
    
    
    private TileObject GetTile(int x, int y) {
        if (_tileGrid[x, y] != null) {
            return _tileGrid[x, y];
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
        else if (Input.GetMouseButtonDown(1)) {
            var tile = GetSelectTile();
            if (tile != null) {
                if(tile.TakeDmgAndCheckDie(1)) {
                    RemoveTiles(new List<TileObject>() {tile});
                }
            }
        }else if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(FillEmptySpaces());
        }
    }

    private TileObject GetSelectTile() {
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("TileObject")) {
            TileObject tile = hit.collider.gameObject.GetComponent<TileObject>();
            if (tile != null) {
                return tile;
            }
        }

        return null;
    }

    private Vector2 startDragPosition;
    private Vector2 endDragPosition;

    void HandleMouseDrag() {
        if (selectedTile != null) {
            if (Input.GetMouseButtonDown(0)) {
                startDragPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0)) {
                endDragPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
                SwapTiles();
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
            if (targetTile.tileType == TileObject.TileType.Normal) {
                StartCoroutine(SwapTileProcess(selectedTile, targetTile));
            }
            else {
                Debug.LogWarning("Target Tile is Obstacle");
            }
        }
        else {
            Debug.LogWarning("Target Tile Is Null");
        }
    }

    IEnumerator SwapTileProcess(TileObject tile1, TileObject tile2) {
        var x1 = tile1.xIndex;
        var y1 = tile1.yIndex;
        var x2 = tile2.xIndex;
        var y2 = tile2.yIndex;
        StartCoroutine(TranslateTile(tile1, x2, y2));
        yield return TranslateTile(tile2, x1, y1);

        _tileGrid[x1, y1] = tile2;
        _tileGrid[x2, y2] = tile1;

        var removeList = new List<TileObject>();
        var matched_1 = CheckForMatchesAt(_tileGrid[tile1.xIndex, tile1.yIndex]);
        if (matched_1.Count >= 3) {
            foreach (var tile in matched_1) {
                removeList.Add(tile);
            }
        }

        var matched_2 = CheckForMatchesAt(_tileGrid[tile2.xIndex, tile2.yIndex]);
        if (matched_2.Count >= 3) {
            foreach (var tile in matched_2) {
                removeList.Add(tile);
            }
        }

        if (removeList.Count > 0) {
            // Debug.LogWarning("Swap tile: " + tile1.xIndex + "," + tile1.yIndex + " <-> " + tile2.xIndex + "," + tile2.yIndex);
            RemoveTiles(removeList);
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
            _tileGrid[x1, y1] = tile2;
            _tileGrid[x2, y2] = tile1;
        }
    }
    
    private void RemoveTiles(List<TileObject> tileList) {
        foreach (var tile in tileList) {
            _tileGrid[tile.xIndex, tile.yIndex] = null;
            tile.Removetile();
        }
    }


    IEnumerator TranslateTile(TileObject tile, int x, int y) {
        Vector2 targetPos = GetTilePos(x, y);
        float time = 0;
        //기준 속도는 타일 한칸 움직이는 정도
        float duration = TILE_MOVE_SPD;
        while (time < duration) {
            time += Time.deltaTime;
            float t = time / duration;
            var nextPos = Vector2.Lerp(tile.transform.position, targetPos, t);
            tile.SetTilePos(nextPos);
            yield return null;
        }

        tile.SetTilePos(targetPos);
        tile.SetTileXY(x, y);
    }

     //original FillEmptySpaces backup
    IEnumerator FillEmptySpaces() {
        for(int y=0; y<height-1; y++) {
            bool isFilled = false;
            for(int x=0; x<width; x++) {
                if(_tileGrid[x,y] == null) {
                    for (int up_y = y + 1; up_y < height; up_y++) {
                        if (_tileGrid[x, up_y] != null) {
                            if(_tileGrid[x, up_y].tileType == TileObject.TileType.Obstacle) {
                                break;
                            }
                            _tileGrid[x,y] = _tileGrid[x,up_y];
                            _tileGrid[x,up_y] = null;
                            StartCoroutine(TranslateTile(_tileGrid[x,y], x, y));
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
                if (_tileGrid[x, y] == null) {
                    var pos = GetTilePos(x, y);
                    var newTile = GenerateNormalTile(pos);
                    newTile.SetTileXY(x, y);
                    _tileGrid[x, y] = newTile;
                }
                else {
                    break;
                }
            }
        }
        
    }

    /*
    IEnumerator FillEmptySpaces() {
        /*for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (tiles[x, y] == null) {
                    // 바로 위의 타일들을 먼저 검사합니다.
                    TileObject tileToMove = FindTileDirectlyAbove(x, y);
                    if (tileToMove == null) {
                        // 바로 위에 사용 가능한 타일이 없다면 대각선을 검사합니다.
                        tileToMove = FindDiagonalTileAbove(x, y);
                    }
                    if (tileToMove != null) {
                        tiles[x, y] = tileToMove;
                        tiles[tileToMove.xIndex, tileToMove.yIndex] = null;
                        StartCoroutine(TranslateTile(tiles[x, y], x, y));
                        yield return new WaitForSeconds(.1f);
                    }
                }
            }
        }#1#
        
        for (int y = 1; y < height; y++) {
            for (int x =0; x < width; x++) { // y=0부터 시작할 필요가 없습니다. 맨 아래줄은 검사하지 않아도 됩니다.
                if (tiles[x, y]?.tileType == TileObject.TileType.Normal && tiles[x, y - 1] == null) {
                    // 아래의 타일이 비어 있으면 현재 타일을 아래로 한 칸 이동
                    tiles[x, y - 1] = tiles[x, y];
                    tiles[x, y] = null;
                    StartCoroutine(TranslateTile(tiles[x, y - 1], x, y - 1));
                }
            }
            yield return new WaitForSeconds(.1f);
        }
        
        // Create new tiles in the empty spaces at the top
        for (int x = 0; x < width; x++) {
            for (int y = height - 1; y >= 0; y--) {
                if (tiles[x, y] == null) {
                    var pos = GetTilePos(x, y);
                    var newTile = GenerateNormalTile(pos);
                    newTile.SetTileXY(x, y);
                    tiles[x, y] = newTile;
                }
                else {
                    break;
                }
            }
        }
        
    }
    */
    
    void FillSpaceDirect() {
        for (int x = 0; x < width; x++) {
            for (int y = 1; y < height; y++) { // y=0부터 시작할 필요가 없습니다. 맨 아래줄은 검사하지 않아도 됩니다.
                if (_tileGrid[x, y] != null && _tileGrid[x, y - 1] == null) {
                    // 아래의 타일이 비어 있으면 현재 타일을 아래로 한 칸 이동
                    _tileGrid[x, y - 1] = _tileGrid[x, y];
                    _tileGrid[x, y] = null;
                    StartCoroutine(TranslateTile(_tileGrid[x, y - 1], x, y - 1));
                }
            }
        }
    }
    
    TileObject FindTileDirectlyAbove(int x, int y) {
        for (int up_y = y + 1; up_y < height; up_y++) {
            if (_tileGrid[x, up_y]?.tileType == TileObject.TileType.Normal) {
                return _tileGrid[x, up_y];
            }
        }
        return null;
    }

    TileObject FindDiagonalTileAbove(int x, int y) {
        for (int up_y = y + 1; up_y < height; up_y++) {
            // 대각선 위 오른쪽 타일 검사 (경계 검사 포함)
            if (x + 1 < width && _tileGrid[x + 1, up_y]?.tileType == TileObject.TileType.Normal) {
                return _tileGrid[x + 1, up_y];
            }
            // 대각선 위 왼쪽 타일 검사 (경계 검사 포함)
            if (x - 1 >= 0 && _tileGrid[x - 1, up_y]?.tileType == TileObject.TileType.Normal) {
                return _tileGrid[x - 1, up_y];
            }
        }
        return null;
    }
    
    
    TileObject GetAdjacentTile(TileObject tile, int x, int y) {
        var targetX = tile.xIndex + x;
        var targetY = tile.yIndex + y;
        if (targetX >= 0 && targetX < width && targetY >= 0 && targetY < height) {
            return GetTile(targetX, targetY);
        }
        
        return null;
    }

    //tileList의 주변 타일들을 반환
    List<TileObject> GetAdjacentTileList(List<TileObject> tileList) {   
        var result = new List<TileObject>();
        foreach (var tile in tileList) {
            var leftTile = GetAdjacentTile(tile, -1, 0);
            if (leftTile != null ) {
                result.Add(leftTile);
            }
            var rightTile = GetAdjacentTile(tile, 1, 0);
            if (rightTile != null) {
                result.Add(rightTile);
            }
            var upTile = GetAdjacentTile(tile, 0, 1);
            if (upTile != null) {
                result.Add(upTile);
            }
            var downTile = GetAdjacentTile(tile, 0, -1);
            if (downTile != null) {
                result.Add(downTile);
            }
        }

        return result;
    }

    List<TileObject> CheckForMatchesAt(TileObject tile) {
        var xIndex = tile.xIndex;
        var yIndex = tile.yIndex;

        List<TileObject> matchedTiles = new List<TileObject>();

        // Check horizontally
        List<TileObject> horizontalMatches = new List<TileObject>();
        horizontalMatches.Add(_tileGrid[xIndex, yIndex]);
        for (int i = xIndex + 1; i < width; i++) {
            if(GetTile(i, yIndex) == null) break;
            if (_tileGrid[i, yIndex].tileId == _tileGrid[xIndex, yIndex].tileId) {
                horizontalMatches.Add(_tileGrid[i, yIndex]);
            }
            else
                break;
        }

        for (int i = xIndex - 1; i >= 0; i--) {
            if (GetTile(i, yIndex) == null) break;
            if (_tileGrid[i, yIndex].tileId == _tileGrid[xIndex, yIndex].tileId) {
                horizontalMatches.Add(_tileGrid[i, yIndex]);
            }
            else
                break;
        }

        if (horizontalMatches.Count >= 3) {
            matchedTiles.AddRange(horizontalMatches);
        }

        // Check vertically
        List<TileObject> verticalMatches = new List<TileObject>();
        verticalMatches.Add(_tileGrid[xIndex, yIndex]);
        for (int i = yIndex + 1; i < height; i++) {
            if (GetTile(xIndex, i) == null) 
                break;
            if (_tileGrid[xIndex, i].tileId == _tileGrid[xIndex, yIndex].tileId) {
                verticalMatches.Add(_tileGrid[xIndex, i]);
            }
            else
                break;
        }

        for (int i = yIndex - 1; i >= 0; i--) {
            if (_tileGrid[xIndex, i] == null)
                break;
            if (_tileGrid[xIndex, i].tileId == _tileGrid[xIndex, yIndex].tileId) {
                verticalMatches.Add(_tileGrid[xIndex, i]);
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