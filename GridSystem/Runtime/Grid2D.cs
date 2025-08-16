using System;
using System.Text;
using UnityEngine;

namespace Assets.GridSystem
{
    /// <summary>
    /// Generic, high-performance 2D grid container.
    /// - Stores a T per-cell.
    /// - Lightweight bounds checks and fast indexing.
    /// - Designed for reuse across projects (no game logic here).
    ///
    /// Features:
    /// - Generic storage: Grid2D<T>
    /// - Constructor with default value factory
    /// - Get / Set / TryGet
    /// - Indexer and safe/unsafe access methods
    /// - Fast iteration helpers
    /// - Optional OnCellChanged event (no allocations in hot path)
    ///
    /// Notes for mobile performance:
    /// - Internally uses a flat array (T[]), cache-friendly.
    /// - Avoids per-call allocations.
    /// - Keep T small (structs preferred) for best memory layout.
    ///
    /// Usage example:
    /// var grid = new Grid2D<TileData>(width, height, Vector3.zero, 1f, () => new TileData());
    /// grid.SetCell(2,3, new TileData(...));
    /// </summary>

    // Custom delegate to allow passing ref parameter in callbacks
    public delegate void RefAction<TCell>(int x, int y, ref TCell cell);

    public class Grid2D<T>
    {
        // Public readonly fields (fast access without properties)
        public readonly int Width;
        public readonly int Height;
        public readonly float CellSize;
        public readonly Vector3 Origin;

        // Flat array storage: index = x + y * Width
        private readonly T[] _cells;

        // Default factory to create "empty" cells when needed. Optional.
        private readonly Func<T> _defaultFactory;

        // Event fired when a cell changes. Use with care in hot paths.
        // Signature: (x, y, oldValue, newValue)
        public event Action<int, int, T, T> OnCellChanged;

        /// <summary>
        /// Creates a Grid2D with given dimensions and cell size. Optionally supply an origin and default factory.
        /// </summary>
        public Grid2D(int width, int height, Vector3 origin = default, float cellSize = 1f, Func<T> defaultFactory = null)
        {
            if (width <= 0) throw new ArgumentException("width must be > 0");
            if (height <= 0) throw new ArgumentException("height must be > 0");
            if (cellSize <= 0f) throw new ArgumentException("cellSize must be > 0");

            Width = width;
            Height = height;
            Origin = origin;
            CellSize = cellSize;

            _cells = new T[Width * Height];
            _defaultFactory = defaultFactory;

            if (_defaultFactory != null)
            {
                // Initialize with factory values to avoid repeated allocations later
                for (int i = 0; i < _cells.Length; ++i)
                    _cells[i] = _defaultFactory();
            }
        }

        #region Indexing Helpers
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private int Index(int x, int y) => x + y * Width;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(int x, int y) => (uint)x < (uint)Width && (uint)y < (uint)Height;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(Vector2Int cell) => IsInBounds(cell.x, cell.y);
        #endregion

        #region Accessors
        /// <summary>
        /// Safe getter. Returns default(T) if out of bounds (or created default from factory if provided).
        /// </summary>
        public T GetCell(int x, int y)
        {
            if (!IsInBounds(x, y)) throw new IndexOutOfRangeException($"GetCell out of bounds: {x},{y}");
            return _cells[Index(x, y)];
        }

        /// <summary>
        /// Safe setter. Fires OnCellChanged if value differs (== operator used).
        /// </summary>
        public void SetCell(int x, int y, T value)
        {
            if (!IsInBounds(x, y)) throw new IndexOutOfRangeException($"SetCell out of bounds: {x},{y}");
            int i = Index(x, y);
            T old = _cells[i];
            // Only invoke if changed - avoids noisy events.
            if (!Equals(old, value))
            {
                _cells[i] = value;
                OnCellChanged?.Invoke(x, y, old, value);
            }
        }

        /// <summary>
        /// Try-get without exception overhead. Returns false if out-of-bounds.
        /// </summary>
        public bool TryGetCell(int x, int y, out T result)
        {
            if (!IsInBounds(x, y))
            {
                result = default;
                return false;
            }
            result = _cells[Index(x, y)];
            return true;
        }

        /// <summary>
        /// Unsafe fast getter: no bounds checks. Use only when you've already validated coordinates.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ref T GetCellRefUnsafe(int x, int y)
        {
            return ref _cells[Index(x, y)];
        }

        /// <summary>
        /// Indexer (safe) for convenience.
        /// </summary>
        public T this[int x, int y]
        {
            get => GetCell(x, y);
            set => SetCell(x, y, value);
        }
        #endregion

        #region Iteration
        /// <summary>
        /// Fast foreach over all cells. Caller provides an Action with x,y and ref T to modify in-place.
        /// Avoid closures allocating in hot loops by using static delegates if possible.
        /// </summary>
        public void ForEach(Action<int, int, T> action)
        {
            int idx = 0;
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    action(x, y, _cells[idx]);
                    idx++;
                }
            }
        }

        /// <summary>
        /// Faster mutation loop using a ref accessor lambda (avoids copying large structs).
        /// Example: grid.ForEachRef((x,y, ref cell) => { cell.SomeField = 1; });
        /// </summary>
        public void ForEachRef(RefAction<T> action)
        {
            int idx = 0;
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    action(x, y, ref _cells[idx]);
                    idx++;
                }
            }
        }
        #endregion

        #region Conversion Helpers
        /// <summary>
        /// Converts a world position to grid coordinates (floored). Uses Origin and CellSize.
        /// Note: this is a convenience helper; a full-featured GridUtils handles different alignments.
        /// </summary>
        public Vector2Int WorldToCell(Vector3 worldPos)
        {
            var local = worldPos - Origin;
            int x = Mathf.FloorToInt(local.x / CellSize);
            int y = Mathf.FloorToInt(local.y / CellSize);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Converts grid coordinates to world position at the cell center.
        /// </summary>
        public Vector3 CellToWorldCenter(int x, int y)
        {
            float cx = Origin.x + (x + 0.5f) * CellSize;
            float cy = Origin.y + (y + 0.5f) * CellSize;
            return new Vector3(cx, cy, Origin.z);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Clear all cells to their default value (factory or default(T)).
        /// </summary>
        public void ClearAll()
        {
            if (_defaultFactory != null)
            {
                for (int i = 0; i < _cells.Length; ++i) _cells[i] = _defaultFactory();
            }
            else
            {
                Array.Clear(_cells, 0, _cells.Length);
            }
        }

        /// <summary>
        /// Fill the grid using a callback. Useful for procedural generation.
        /// </summary>
        public void Fill(Func<int, int, T> filler)
        {
            int idx = 0;
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    _cells[idx] = filler(x, y);
                    idx++;
                }
            }
        }

        /// <summary>
        /// Returns a debug string with basic size info and a small sample of cells (ToString should not be used in hot loops).
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Grid2D<{typeof(T).Name}> {Width}x{Height} | CellSize={CellSize}");
            int sampleH = Math.Min(4, Height);
            int sampleW = Math.Min(8, Width);
            for (int y = 0; y < sampleH; ++y)
            {
                for (int x = 0; x < sampleW; ++x)
                {
                    sb.Append(_cells[Index(x, y)]?.ToString() ?? "_");
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        #endregion
    }
}
