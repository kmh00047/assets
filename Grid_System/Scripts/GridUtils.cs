using UnityEngine;

namespace Assets.GridSystem
{
    /// <summary>
    /// Static utility class for advanced grid calculations.
    /// Supports rectangular and hexagonal grids (only rectangular implemented now).
    /// </summary>
    public static class GridUtils
    {
        /// <summary>
        /// Grid shape types.
        /// </summary>
        public enum GridShape
        {
            Rectangular,
            Hexagonal
        }

        /// <summary>
        /// Converts world position to grid coordinates for rectangular grids.
        /// Floors the position to the nearest lower integer cell.
        /// </summary>
        public static Vector2Int WorldToGridRect(Vector3 worldPos, Vector3 origin, float cellSize)
        {
            Vector3 local = worldPos - origin;
            int x = Mathf.FloorToInt(local.x / cellSize);
            int y = Mathf.FloorToInt(local.y / cellSize);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Converts grid coordinates to world position at cell center for rectangular grids.
        /// </summary>
        public static Vector3 GridToWorldRect(Vector2Int gridPos, Vector3 origin, float cellSize)
        {
            float x = origin.x + (gridPos.x + 0.5f) * cellSize;
            float y = origin.y + (gridPos.y + 0.5f) * cellSize;
            return new Vector3(x, y, origin.z);
        }

        /// <summary>
        /// Snaps any world position to the nearest grid cell center.
        /// Currently supports rectangular grids only.
        /// </summary>
        public static Vector3 SnapToGrid(Vector3 worldPos, Vector3 origin, float cellSize, GridShape shape = GridShape.Rectangular)
        {
            switch (shape)
            {
                case GridShape.Rectangular:
                    Vector2Int cell = WorldToGridRect(worldPos, origin, cellSize);
                    return GridToWorldRect(cell, origin, cellSize);

                case GridShape.Hexagonal:
                    // TODO: implement hex snapping later
                    return worldPos;

                default:
                    return worldPos;
            }
        }

        /// <summary>
        /// Neighbor offsets for rectangular grid (4-directional).
        /// Up, Right, Down, Left.
        /// </summary>
        public static readonly Vector2Int[] RectNeighbors4 =
        {
            new Vector2Int(0, 1),    // Up
            new Vector2Int(1, 0),    // Right
            new Vector2Int(0, -1),   // Down
            new Vector2Int(-1, 0)    // Left
        };

        /// <summary>
        /// Neighbor offsets for rectangular grid (8-directional).
        /// Includes diagonals: UpRight, DownRight, DownLeft, UpLeft.
        /// </summary>
        public static readonly Vector2Int[] RectNeighbors8 =
        {
            new Vector2Int(0, 1),    // Up
            new Vector2Int(1, 1),    // UpRight
            new Vector2Int(1, 0),    // Right
            new Vector2Int(1, -1),   // DownRight
            new Vector2Int(0, -1),   // Down
            new Vector2Int(-1, -1),  // DownLeft
            new Vector2Int(-1, 0),   // Left
            new Vector2Int(-1, 1)    // UpLeft
        };

        // Placeholder for Hex neighbors and conversions to be added later
        // This will be updated once we have support for hexagonal grids in 
        // Grid2D<T> and related utilities.
    }
}
