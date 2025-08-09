using UnityEngine;
using Assets.GridSystem;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public Vector3 origin = Vector3.zero;

    private Grid2D<GridCell> grid;
    private GridVisualizer visualizer;

    void Awake()
    {
        try
        {// Create grid with default factory initializing floor tiles walkable
            grid = new Grid2D<GridCell>(width, height, origin, cellSize,
                () => new GridCell(TileType.Floor, true));

            // Find or add visualizer component on this gameObject
            visualizer = GetComponent<GridVisualizer>();
            if (visualizer == null)
                visualizer = gameObject.AddComponent<GridVisualizer>();

            visualizer.grid = grid;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to initialize grid: {ex.Message}");
            enabled = false; // Disable this component if grid creation fails
        }
    }

    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int cellPos = grid.WorldToCell(mouseWorld);

        if (grid.IsInBounds(cellPos))
        {
            visualizer.highlightedCell = cellPos;

            if (Input.GetMouseButtonDown(0)) // Left click toggles walkability
            {
                var cell = grid.GetCell(cellPos.x, cellPos.y);
                cell.IsWalkable = !cell.IsWalkable;
                cell.Type = cell.IsWalkable ? TileType.Floor : TileType.Wall;
                grid.SetCell(cellPos.x, cellPos.y, cell);

                Debug.Log($"Toggled cell at {cellPos} to {cell}");
            }
        }
        else
        {
            visualizer.highlightedCell = new Vector2Int(-1, -1);
        }
    }
}
