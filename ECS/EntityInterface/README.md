# Entity Interface

The **Entity Interface** folder defines the core contracts and base classes for all entities in the ECS.  
It ensures a consistent API for creation, identification, updating, and lifecycle management of entities.

Think of it as the "DNA blueprint" for every object that participates in the ECS.

---

## Files Overview

### `ITickable.cs`
Interface for any object that can receive regular updates ("ticks").

```csharp
public interface ITickable
{
    void Tick();
}
```
Purpose:
 - Allows the ECS to update entities, systems, or any object in a unified manner.

 -  Decouples ticking logic from specific implementations.

### `IEntity.cs`

Defines the contract for all entities in the ECS.

```csharp
public interface IEntity : ITickable
{
    Guid Id { get; }
}
```
Key Points:

    Every entity must have a unique Guid Id.

    Entities must implement Tick() (inherited from ITickable).

### `BaseEntity.cs`

An abstract base class providing:

    Unique ID generation (Guid)

    Activation/Deactivation lifecycle

    Position tracking (with change events)

    Lifecycle hooks (OnCreated, OnActivated, OnDeactivated, OnDestroyed)
```csharp
public abstract class BaseEntity : IEntity
{
    public Guid Id { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Vector2Int Position { get; set; }

    public virtual void Activate() { ... }
    public virtual void Deactivate() { ... }
    public virtual void Destroy() { ... }

    public abstract void Tick();
}
```
Notable Feature – Position Change Event:
```csharp
public event Action<BaseEntity, Vector2Int, Vector2Int> OnPositionChanged;
```
Fires whenever an entity moves, making it easy to trigger spatial updates or AI reactions.
Example: Creating a Custom Entity
```csharp
public class Enemy : BaseEntity
{
    public override void Tick()
    {
        // Custom AI or behavior logic
        Position += Vector2Int.right; // Move right each tick
    }

    public override void OnCreated()
    {
        Debug.Log("Enemy spawned with ID: " + Id);
    }
}
```
Usage in ECS:
```csharp
var enemy = new Enemy();
enemy.Position = new Vector2Int(0, 0);
enemy.Activate();
```
Design Philosophy

    Minimal but essential contracts → Keeps ECS flexible for any game type.

    Event-driven state changes → Easy to hook into movement or lifecycle events.

    Extendable base class → Common logic handled once; specific behavior defined in subclasses.

---
