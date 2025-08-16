using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Entities;
public class EntityManager : MonoBehaviour
{
    private readonly EntityRegistry registry;
    private readonly EntitySpatialIndex spatialIndex;
    private readonly EntityTicker entityTicker;

    public EntityManager(Ticker ticker, int gridWidth, int gridHeight)
    {
        registry = new EntityRegistry();
        spatialIndex = new EntitySpatialIndex(gridWidth, gridHeight);
        entityTicker = new EntityTicker(ticker);
    }

    public void AddEntity(IEntity entity, Vector2Int position)
    {
        registry.Register(entity);
        entityTicker.Register(entity);
        spatialIndex.Add(entity, position);
    }

    public void RemoveEntity(Guid id)
    {
        var entity = registry.Get(id);
        if (entity == null) return;

        entityTicker.Unregister(entity);
        spatialIndex.Remove(entity, ((BaseEntity)entity).Position);
        registry.Unregister(id);
    }

    public IEntity GetEntity(Guid id) => registry.Get(id);
    public IEnumerable<IEntity> GetEntitiesAt(Vector2Int pos) => spatialIndex.GetAt(pos);
    public bool IsOccupied(Vector2Int pos) => spatialIndex.IsOccupied(pos);

    public void BeginTick() => entityTicker.BeginTick();
    public void EndTick() => entityTicker.EndTick();

    public void ClearAll()
    {
        entityTicker.BeginTick();
        foreach (var e in registry.All)
            entityTicker.Unregister(e);
        registry.Clear();
        spatialIndex.Clear();
        entityTicker.EndTick();
    }
}
