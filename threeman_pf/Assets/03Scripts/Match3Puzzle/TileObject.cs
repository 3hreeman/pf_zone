using UnityEngine;

public class TileObject : MonoBehaviour
{
    public bool isSelected { get; private set; } = false;
    private SpriteRenderer spriteRenderer;
    public int xIndex;
    public int yIndex;
    public int tileType;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(int x, int y, int type, Sprite spr)
    {
        xIndex = x;
        yIndex = y;
        tileType = type;
        spriteRenderer.sprite = spr;
    }

    public void SelectTile()
    {
        isSelected = true;
        // 선택된 타일에 대한 간단한 피드백으로 색상을 변경합니다.
        spriteRenderer.color = Color.yellow;
    }

    public void DeselectTile()
    {
        isSelected = false;
        spriteRenderer.color = Color.white;
    }

    public void SwapTile(TileObject tile)
    {
        var orgX = xIndex;
        var orgY = yIndex;
        xIndex = tile.xIndex;
        yIndex = tile.yIndex;
        tile.xIndex = orgX;
        tile.yIndex = orgY;
        
    }
}