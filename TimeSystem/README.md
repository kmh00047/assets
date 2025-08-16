# Time System

The **Time System** is the centralized heartbeat of the ECS.  
It controls how often updates ("ticks") are sent to registered objects, ensuring consistent simulation timing across the game.

By decoupling updates from Unity's frame rate, the Time System ensures:
- Deterministic behavior (important for AI, physics, and multiplayer sync)
- Stable pacing even under variable frame performance
- Fine-grained control over simulation speed

---

## Files Overview

### `ITickable.cs`
Defines the contract for any object that can receive ticks.

```csharp
public interface ITickable
{
    void Tick();
}
```
Purpose: Standardizes update calls across all ECS components.

 - Used by Ticker to update entities, managers, and other systems.

### `Ticker.cs`

The centralized tick manager.
Responsible for firing ticks at a fixed interval and updating all registered ITickable objects.

Key Features:

 - Adjustable tick rate (tickInterval)

 - Option to enable/disable custom tick timing (fallback to Unity’s per-frame updates)

 - Runtime registration and unregistration of tickables

 - Efficient List<ITickable> storage

- **Example:**
```csharp
Ticker ticker = FindObjectOfType<Ticker>();
ticker.RegisterTickable(myEntity);
ticker.SetTickInterval(0.5f); // 2 ticks per second
```
- **Core Update Flow:**

 - Accumulates delta time

 - Fires Tick() calls when enough time has passed

 - Supports processing multiple ticks in one frame if needed

### `TickerEditor.cs (Editor Only)`

A custom Unity Inspector for the Ticker component.

 - Enable Tick Manager toggle

 - Editable Tick Interval (only when enabled)

 - Automatically clamps tick interval to a minimum of 0.01f seconds

 - Changes apply immediately in the Editor

**Visual Benefit:** Makes the Time System easy to configure without digging into the code.

### `Example Usage:`
```csharp
public class EnemyAI : ITickable
{
    public void Tick()
    {
        Debug.Log("Enemy logic executed");
    }
}

// Registering with the ticker
var ticker = FindAnyObjectByType<Ticker>();
var enemy = new EnemyAI();
ticker.RegisterTickable(enemy);
```

Design Notes

 - Frame rate independence: Perfect for deterministic gameplay loops.

 - Flexible: Any system can be ticked — not just entities.

 - Unity-friendly: Built as a MonoBehaviour with custom Inspector.

 - Performance conscious: Uses a simple List<ITickable> with O(n) iteration.


---
