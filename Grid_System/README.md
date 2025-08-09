# Grid2D - Ultra-Performant Modular 2D Grid System for Unity

## Overview

**Grid2D** is a lightweight, high-performance, generic 2D grid container designed for modern game development workflows. Crafted for Unity, this system provides a rock-solid foundation to manage tile-based or grid-aligned gameplay data without sacrificing speed or flexibility.

Whether you're building complex strategy maps, puzzle grids, or RPG world layouts, Grid2D delivers *blazing-fast* access and mutation patterns, enabling developers to focus on gameplay — not grid overhead.

---

## Why Grid2D?

- **Ultra-High Performance**  
  Built from the ground up for speed:  
  - Flat contiguous memory array for cache-friendly access.  
  - Zero per-call allocations.  
  - Optimized safe and unsafe accessors with aggressive inlining.  
  - Minimal bounds checks using efficient unsigned comparisons.  

- **Generic & Flexible**  
  Store *any* struct or class type per cell: from simple flags to complex tile data, NPC states, or custom gameplay objects.  
  No bloat, no assumptions, just a solid grid backbone.

- **Memory & CPU Efficient**  
  Uses a single flat array internally with optional default factory initialization to avoid runtime GC pressure.  
  Designed to keep your mobile and desktop builds smooth under any load.

- **Rich API**  
  - Safe and unsafe getters/setters with indexers  
  - Fast iteration with mutation callbacks supporting `ref` access for large structs  
  - Built-in coordinate conversions between world space and grid cells  
  - Change events for reactive gameplay hooks (optional, zero-cost if unused)  

- **Unity Friendly**  
  Easily integrates into Unity projects, compatible with editor tools, custom visualizers, and scalable for large grid sizes.

---

## Quick Usage Example

```csharp
// Define your cell data struct or class
public struct TileData
{
    public bool IsWalkable;
    public int TerrainType;
}

// Create a 20x15 grid with cell size 1 unit, origin at world zero
var grid = new Grid2D<TileData>(20, 15, Vector3.zero, 1f, () => new TileData { IsWalkable = true });

// Set a cell property safely
var cell = grid.GetCell(5, 10);
cell.IsWalkable = false;
grid.SetCell(5, 10, cell);

// Iterate all cells efficiently
grid.ForEachRef((x, y, ref TileData tile) =>
{
    // Mark edge cells as non-walkable
    if (x == 0 || y == 0 || x == grid.Width - 1 || y == grid.Height - 1)
        tile.IsWalkable = false;
});
