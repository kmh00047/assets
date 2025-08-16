using System;

namespace Assets.GridSystem
{
    /// <summary>
    /// Enum representing the type of tile in a grid cell.
    /// Enum is expandable as per the need.
    /// </summary>
    public enum TileType
    {
        Empty = 0,
        Floor,
        Wall,
    }

    /// <summary>
    /// Represents a single cell in the grid.
    /// Use as the generic type parameter for Grid2D<T>.
    /// </summary>
    public struct GridCell
    {
        /// <summary>Type of tile this cell represents.</summary>
        public TileType Type;

        /// <summary>Whether this cell can be walked on or blocked.</summary>
        public bool IsWalkable;

        /// <summary>
        /// Occupant ID to track entities occupying this cell.
        /// -1 means empty.
        /// </summary>
        public int OccupantID;

        /// <summary>
        /// Constructor with default values.
        /// </summary>
        /// <param name="type">Tile type, default Empty</param>
        /// <param name="isWalkable">Is walkable, default true</param>
        /// <param name="occupantID">Occupant ID, default -1 (empty)</param>
        public GridCell(TileType type = TileType.Empty, bool isWalkable = true, int occupantID = -1)
        {
            Type = type;
            IsWalkable = isWalkable;
            OccupantID = occupantID;
        }

        /// <summary>
        /// Override ToString for quick debugging display. Do not use in performance-critical paths.
        /// </summary>
        public override string ToString()
        {
            return $"GridCell(Type={Type}, Walkable={IsWalkable}, Occupant={OccupantID})";
        }
    }
}
