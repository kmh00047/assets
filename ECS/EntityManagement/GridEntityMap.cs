using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.GridSystem;

public class GridEntityMap
{
    private readonly Grid2D<HashSet<IEntity>> grid;

    public GridEntityMap(int width, int height)
    {
        // Initialize grid with default factory to create empty HashSet per cell
        grid = new Grid2D<HashSet<IEntity>>(width, height, defaultFactory: () => new HashSet<IEntity>());
    }

    /// <summary>
    /// Adds entity to the given cell in the grid.
    /// </summary>
    public void AddEntity(IEntity entity, Vector2Int cell)
    {
        if (!grid.IsInBounds(cell)) throw new ArgumentOutOfRangeException(nameof(cell));
        grid.GetCell(cell.x, cell.y).Add(entity);
    }

    /// <summary>
    /// Removes entity from the given cell.
    /// </summary>
    public void RemoveEntity(IEntity entity, Vector2Int cell)
    {
        if (!grid.IsInBounds(cell)) throw new ArgumentOutOfRangeException(nameof(cell));
        grid.GetCell(cell.x, cell.y).Remove(entity);
    }

    /// <summary>
    /// Gets all entities occupying the specified cell.
    /// Returns empty enumerable if none.
    /// </summary>
    public IEnumerable<IEntity> GetEntitiesAt(Vector2Int cell)
    {
        if (!grid.IsInBounds(cell)) yield break;
        foreach (var entity in grid.GetCell(cell.x, cell.y))
            yield return entity;
    }

    /// <summary>
    /// Checks if any entities occupy the cell.
    /// </summary>
    public bool IsOccupied(Vector2Int cell)
    {
        if (!grid.IsInBounds(cell)) return false;
        return grid.GetCell(cell.x, cell.y).Count > 0;
    }

    /// <summary>
    /// Checks if the given cell is within the grid bounds.
    /// </summary>
    /// <returns>True if cell is within bounds, false otherwise.</returns>
    public bool IsInBounds(Vector2Int cell)
    {
        return grid.IsInBounds(cell.x, cell.y);
    }


    /// <summary>
    /// Clears all occupancy.
    /// </summary>
    public void Clear()
    {
        grid.ForEachRef(ClearCell);
    }

    // Method Group, used by ForEachRef to clear each cell's entities.
    // Used to avoid huge run time heap alloctions and compiler will 
    // only generate one method for all cells
    private static void ClearCell(int x, int y, ref HashSet<IEntity> cellEntities)
    {
        cellEntities.Clear();
    }



}
