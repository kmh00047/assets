using UnityEngine;

namespace Assets.GridSystem
{
    /// <summary>
    /// Component to visually debug and display a Grid2D in the Unity Editor.
    /// Attach to a GameObject and assign the grid to visualize.
    /// </summary>
    [ExecuteAlways]  // Run in edit mode too
    public class GridVisualizer : MonoBehaviour
    {
        public Grid2D<GridCell> grid;  // The grid instance to visualize

        [Header("Grid Settings")]
        public Color gridLineColor = Color.green;
        public Color cellHighlightColor = new Color(1f, 1f, 0f, 0.5f);
        public bool showGridLines = true;
        public bool showCellIndices = true;

        [Header("Cell Highlight")]
        public Vector2Int highlightedCell = new Vector2Int(-1, -1);

        private void OnDrawGizmos()
        {
            if (grid == null) return;

            DrawGridLines();
            if (highlightedCell.x >= 0 && highlightedCell.y >= 0 && grid.IsInBounds(highlightedCell))
            {
                HighlightCell(highlightedCell);
            }

            if (showCellIndices)
            {
                DrawCellIndices();
            }
        }

        private void DrawGridLines()
        {
            Gizmos.color = gridLineColor;

            float cs = grid.CellSize;
            Vector3 origin = grid.Origin;

            // Draw vertical lines
            for (int x = 0; x <= grid.Width; x++)
            {
                Vector3 start = new Vector3(origin.x + x * cs, origin.y, origin.z);
                Vector3 end = new Vector3(origin.x + x * cs, origin.y + grid.Height * cs, origin.z);
                Gizmos.DrawLine(start, end);
            }

            // Draw horizontal lines
            for (int y = 0; y <= grid.Height; y++)
            {
                Vector3 start = new Vector3(origin.x, origin.y + y * cs, origin.z);
                Vector3 end = new Vector3(origin.x + grid.Width * cs, origin.y + y * cs, origin.z);
                Gizmos.DrawLine(start, end);
            }
        }

        private void HighlightCell(Vector2Int cell)
        {
            Gizmos.color = cellHighlightColor;
            Vector3 cellCenter = grid.CellToWorldCenter(cell.x, cell.y);
            float cs = grid.CellSize;

            Vector3 bottomLeft = new Vector3(cellCenter.x - cs / 2, cellCenter.y - cs / 2, cellCenter.z);
            Vector3 size = new Vector3(cs, cs, 0.1f);

            Gizmos.DrawCube(bottomLeft + size / 2, size);
        }

        private void DrawCellIndices()
        {
            // This requires Handles API (UnityEditor namespace), so only works in Editor
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.white;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    Vector3 pos = grid.CellToWorldCenter(x, y);
                    string label = $"({x},{y})";
                    UnityEditor.Handles.Label(pos, label);
                }
            }
#endif
        }
    }
}
