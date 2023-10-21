using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TileObject : MonoBehaviour {
    public enum TileType {
        Normal,
        Obstacle
    }

    [SerializeField] private Sprite[] tileSprites;
    [SerializeField] private Sprite[] obstacleSprites;
    public bool isSelected { get; private set; } = false;
    private SpriteRenderer spriteRenderer;
    public TileType tileType;
    public int tileId;
    public int xIndex;
    public int yIndex;
    public int tileHp;
    public TextMeshPro posText;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        var str = $"({xIndex}, {yIndex})";
        posText.text = str;
        if (tileSprites[tileId] == spriteRenderer.sprite) {
            posText.color = Color.black;
        }
        else {
            posText.color = Color.red;
        }
    }

    public void SetNormalTile(int id) {
        tileType = TileType.Normal;
        tileId = id;
        tileHp = 1;
        spriteRenderer.sprite = tileSprites[tileId];
    }
    
    public void SetObstacleTile(int id) {
        tileType = TileType.Obstacle;
        tileId = id;
        tileHp = 1;
        spriteRenderer.sprite = obstacleSprites[tileId];
    }
    
    public void SelectTile() {
        isSelected = true;
    }

    public void DeselectTile() {
        isSelected = false;
    }

    public void SetTileXY(int x, int y) {
        xIndex = x;
        yIndex = y;
    }

    public void SetTilePos(Vector2 pos) {
        transform.position = pos;
    }
    public void SwapTile(TileObject tile) {
        var orgX = xIndex;
        var orgY = yIndex;
        SetTileXY(tile.xIndex, tile.yIndex);
        tile.SetTileXY(orgX, orgY);
    }

    public bool TakeDmgAndCheckDie(int dmg) {
        tileHp -= dmg;
        if (tileHp <= 0) {
            Removetile();
            return true;
        }

        return false;
    }

    public void Removetile() {
        Destroy(gameObject);
    }
}