using UnityEngine;

public class PuzzleMain : MonoBehaviour
{
    private const float TILE_SIZE = 0.64f;
    public GameObject tilePrefab; // Tile 프리팹을 연결합니다.
    private const int Rows = 8;
    private const int Columns = 8;
    private const int MatchLength = 3;
    private GameObject[,] tiles = new GameObject[Rows, Columns];
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
                GameObject tile = Instantiate(tilePrefab, new Vector2(j * TILE_SIZE, i * TILE_SIZE), Quaternion.identity);
                tile.GetComponent<SpriteRenderer>().sprite = tileSprites[Random.Range(0, tileSprites.Length)];
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
        TileObject tileToSwapWith = null;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal swipe
            if (direction.x > 0)
                tileToSwapWith = GetAdjacentTile(selectedTile, Vector2.right);
            else
                tileToSwapWith = GetAdjacentTile(selectedTile, Vector2.left);
        }
        else
        {
            // Vertical swipe
            if (direction.y > 0)
                tileToSwapWith = GetAdjacentTile(selectedTile, Vector2.up);
            else
                tileToSwapWith = GetAdjacentTile(selectedTile, Vector2.down);
        }

        if (tileToSwapWith != null)
        {
            // Swap positions of the selected tile and the adjacent tile
            Vector3 tempPosition = selectedTile.transform.position;
            selectedTile.transform.position = tileToSwapWith.transform.position;
            tileToSwapWith.transform.position = tempPosition;

            // Additionally, you can also swap their references in the 'tiles' array if necessary.
        }
    }

    TileObject GetAdjacentTile(TileObject tile, Vector2 direction)
    {
        Vector2 position = tile.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(position + direction, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("TileObject"))
            return hit.collider.gameObject.GetComponent<TileObject>();
    
        return null;
    }
}