using System.Collections.Generic;
using UnityEngine;
public class EntitySpatialIndex
{
    private readonly GridEntityMap gridMap;

    public EntitySpatialIndex(int width, int height)
    {
        gridMap = new GridEntityMap(width, height);
    }

    public void Add(IEntity entity, Vector2Int position) => gridMap.AddEntity(entity, position);
    public void Remove(IEntity entity, Vector2Int position) => gridMap.RemoveEntity(entity, position);
    public void Move(IEntity entity, Vector2Int oldPos, Vector2Int newPos)
    {
        gridMap.RemoveEntity(entity, oldPos);
        gridMap.AddEntity(entity, newPos);
    }
    public IEnumerable<IEntity> GetAt(Vector2Int position) => gridMap.GetEntitiesAt(position);
    public bool IsOccupied(Vector2Int position) => gridMap.IsOccupied(position);
    public void Clear() => gridMap.Clear();
}
