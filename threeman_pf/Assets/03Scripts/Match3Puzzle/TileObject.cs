using System;
using TMPro;
using UnityEngine;

public class TileObject : MonoBehaviour {
    public enum TileType {
        NormalTile_1 = 0,
        NormalTile_2 = 1,
        NormalTile_3 = 2,
        NormalTile_4 = 3,
        ObstacleTile_1 = 101,
    }

    [SerializeField] private Sprite[] tileSprites;
    [SerializeField] private Sprite[] obstacleSprites;
    public bool isSelected { get; private set; } = false;
    private SpriteRenderer spriteRenderer;
    public int xIndex;
    public int yIndex;
    public int tileType;
    public int tileHp;
    public TextMeshPro posText;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        var str = $"({xIndex}, {yIndex})";
        posText.text = str;
        if (tileSprites[tileType] == spriteRenderer.sprite) {
            posText.color = Color.black;
        }
        else {
            posText.color = Color.red;
        }
    }

    public void SetTileType(int type) {
        tileType = type;
        spriteRenderer.sprite = tileSprites[type];
    }

    public void SelectTile() {
        isSelected = true;
        // 선택된 타일에 대한 간단한 피드백으로 색상을 변경합니다.
        spriteRenderer.color = Color.yellow;
    }

    public void DeselectTile() {
        isSelected = false;
        spriteRenderer.color = Color.white;
    }

    public void SetTileXY(int x, int y) {
        xIndex = x;
        yIndex = y;
    }

    public void SwapTile(TileObject tile) {
        var orgX = xIndex;
        var orgY = yIndex;
        SetTileXY(tile.xIndex, tile.yIndex);
        tile.SetTileXY(orgX, orgY);
    }

    public void TakeDmgTile(int dmg) {
        tileHp -= dmg;
        if (tileHp <= 0) {
            Removetile();
        }
    }

    public void Removetile() {
        Destroy(gameObject);
    }
}