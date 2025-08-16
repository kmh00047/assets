# Entity Management

The **Entity Management** layer is the coordination hub of the ECS.  
It handles **tracking**, **positioning**, and **updating** all entities, while providing a clean API for the rest of the game.

---

## Files Overview

### `EntityRegistry.cs`
Maintains a **central dictionary** of all active entities, keyed by their `Guid`.

```csharp
public void Register(IEntity entity);
public bool Unregister(Guid id);
public IEntity Get(Guid id);
public IEnumerable<IEntity> All { get; }
public void Clear();
```
Purpose:

 - Fast lookups by unique ID

 - Single source of truth for all active entities

### `EntitySpatialIndex.cs`

Tracks entity positions in a GridEntityMap.
```csharp
public void Add(IEntity entity, Vector2Int position);
public void Remove(IEntity entity, Vector2Int position);
public void Move(IEntity entity, Vector2Int oldPos, Vector2Int newPos);
public IEnumerable<IEntity> GetAt(Vector2Int position);
public bool IsOccupied(Vector2Int position);
public void Clear();
```
Purpose:

 - Optimized spatial queries

 - Efficient add/remove/move operations

 - Uses GridEntityMap (Powered by Assets.GridSystem)

### `EntityTicker.cs`

Bridges the Ticker (from Time System) with entities.
Ensures safe registration/unregistration even while ticking.
```csharp
public void BeginTick();
public void EndTick();
public void Register(IEntity entity);
public void Unregister(IEntity entity);
```
Purpose:

 - Avoids modifying the active tick list during iteration

 - Queues changes until tick cycle ends

### `GridEntityMap.cs`

Maps grid cells to sets of entities using a custom Grid2D implementation.

Key Features:

 - O(1) add/remove/lookup per cell

 - HashSet<IEntity> ensures no duplicates

 - ForEachRef for allocation-free clearing
```csharp
public void AddEntity(IEntity entity, Vector2Int cell);
public void RemoveEntity(IEntity entity, Vector2Int cell);
public IEnumerable<IEntity> GetEntitiesAt(Vector2Int cell);
public bool IsOccupied(Vector2Int cell);
public bool IsInBounds(Vector2Int cell);
public void Clear();
```
### `EntityManager.cs`

The single entry point for working with entities at runtime.
Combines registry, spatial index, and ticker into a unified API.
```csharp
public void AddEntity(IEntity entity, Vector2Int position);
public void RemoveEntity(Guid id);
public IEntity GetEntity(Guid id);
public IEnumerable<IEntity> GetEntitiesAt(Vector2Int pos);
public bool IsOccupied(Vector2Int pos);
public void BeginTick();
public void EndTick();
public void ClearAll();
```
### `Example Usage:`
```csharp
var entityManager = new EntityManager(ticker, 100, 100);
entityManager.AddEntity(enemy, new Vector2Int(5, 5));

if (entityManager.IsOccupied(new Vector2Int(5, 5))) {
    Debug.Log("Enemy present in cell");
}
```
Design Highlights

 - Modular: Each responsibility (tracking, spatial, ticking) has its own class.

 - Safe ticking: No collection modification during iteration.

 - Spatially aware: Fast lookups by position.

 - Unified API: Game code only talks to EntityManager.


---
