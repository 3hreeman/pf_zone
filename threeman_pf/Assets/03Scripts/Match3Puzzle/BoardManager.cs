using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public const float TILE_SIZE = 0.64f;
    public const float TILE_MOVE_SPD = 0.5f;

    public GameObject tilePrefab;
    public float BoardWidth => row * TILE_SIZE;
    public float BoardHeight => col * TILE_SIZE;
    private int row;
    private int col;

    private TileObject[,] _tileGrid;
    public TileObject selectedTile;

    public void SelectTile(TileObject tile) {
        if (selectedTile != null) {
            selectedTile.DeselectTile();
        }

        if (tile != null) {
            tile.SelectTile();
            selectedTile = tile;
        }
    }
    public void InitBoard(int w, int h) {
        row = w;
        col = h;
        row = w;
        col = h;
        _tileGrid = new TileObject[row, col];
        for (int y = 0; y < col; y++) {
            for (int x = 0; x < row; x++) {
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

    
    public void SwapTiles(Vector2 direction) {
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
    
    public void RemoveTiles(List<TileObject> tileList) {
        foreach (var tile in tileList) {
            _tileGrid[tile.xIndex, tile.yIndex] = null;
            tile.Removetile();
        }
    }
    
    List<TileObject> CheckForMatchesAt(TileObject tile) {
        var xIndex = tile.xIndex;
        var yIndex = tile.yIndex;

        List<TileObject> matchedTiles = new List<TileObject>();

        // Check horizontally
        List<TileObject> horizontalMatches = new List<TileObject>();
        horizontalMatches.Add(_tileGrid[xIndex, yIndex]);
        for (int i = xIndex + 1; i < row; i++) {
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
        for (int i = yIndex + 1; i < col; i++) {
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
    
    public IEnumerator FillEmptySpaces() {
        for(int y=0; y<col-1; y++) {
            bool isFilled = false;
            for(int x=0; x<row; x++) {
                if(_tileGrid[x,y] == null) {
                    for (int up_y = y + 1; up_y < col; up_y++) {
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
        for (int x = 0; x < row; x++) {
            for (int y = col - 1; y >= 0; y--) {
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

    IEnumerator TranslateTile(TileObject tile, int x, int y) {
        Vector2 targetPos = GetTilePos(x, y);
        float time = 0;
        //기준 속도는 타일 한칸 움직이는 정도
        float duration = TILE_MOVE_SPD;
        while (time < duration) {
            time += Time.deltaTime;
            float t = time / duration;
            var nextPos = Vector2.Lerp(tile.transform.position, targetPos, t);
            // var nextPos = Vector2.MoveTowards(tile.transform.position, targetPos, t);
            tile.SetTilePos(nextPos);
            yield return null;
        }

        tile.SetTilePos(targetPos);
        tile.SetTileXY(x, y);
    }

    
    TileObject GetAdjacentTile(TileObject tile, int x, int y) {
        var targetX = tile.xIndex + x;
        var targetY = tile.yIndex + y;
        if (targetX >= 0 && targetX < row && targetY >= 0 && targetY < col) {
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
}
