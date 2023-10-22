using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleMain : MonoBehaviour {
    public Camera mainCam;
    public BoardManager boardManager;

    void Start() {
        boardManager.InitBoard(5, 5);
        AdjustCameraPosition();
    }

    void AdjustCameraPosition() {
        float boardWidth = boardManager.BoardWidth;
        float boardHeight = boardManager.BoardHeight;
        Vector2 boardCenter = new Vector2((boardWidth - BoardManager.TILE_SIZE) / 2, (boardHeight - BoardManager.TILE_SIZE) / 2);
        mainCam.transform.position = new Vector3(boardCenter.x, boardCenter.y, mainCam.transform.position.z);
    }

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
                boardManager.SelectTile(tile);
            }
        }
        else if (Input.GetMouseButtonDown(1)) {
            var tile = GetSelectTile();
            if (tile != null) {
                if(tile.TakeDmgAndCheckDie(1)) {
                    boardManager.RemoveTiles(new List<TileObject>() {tile});
                }
            }
        }else if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(boardManager.FillEmptySpaces());
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
        if (boardManager.selectedTile != null) {
            if (Input.GetMouseButtonDown(0)) {
                startDragPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0)) {
                endDragPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(endDragPosition, startDragPosition) < 0.1f) {
                    Debug.Log("mouse drag too short");
                    return;
                }
                Vector2 direction = endDragPosition - startDragPosition;

                boardManager.SwapTiles(direction);
                boardManager.SelectTile(null);
            }
        }
    }

    
     //original FillEmptySpaces backup

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

}