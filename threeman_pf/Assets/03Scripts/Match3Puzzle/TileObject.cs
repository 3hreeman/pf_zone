using UnityEngine;

public class TileObject : MonoBehaviour
{
    public bool isSelected { get; private set; } = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
}