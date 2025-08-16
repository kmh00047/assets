using System;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

/// <summary>
/// Centralized game tick manager.
/// Fires ticks at a fixed interval, independent of frame rate,
/// and updates all registered ITickables in sync.
/// </summary>
public class Ticker : MonoBehaviour
{
    // Time interval between ticks (in seconds) with a slider
    [SerializeField, Tooltip("Enable to allow TickManager to control tick timing.")]
    private bool enableTickManager = true;

    [SerializeField, Tooltip("Time interval between ticks in seconds.")]
    private float tickInterval = 1f;

    // Internal Set of registered tickable objects
    private readonly List<ITickable> tickables = new List<ITickable>();

    // Time accumulator for tick scheduling
    private float timeAccumulator = 0f;

    /// <summary>
    /// Registers an ITickable so it receives ticks.
    /// </summary>
    public void RegisterTickable(ITickable tickable)
    {
        if (tickable == null) throw new ArgumentNullException(nameof(tickable));
        if (!tickables.Contains(tickable))
            tickables.Add(tickable);
    }

    /// <summary>
    /// Unregisters an ITickable so it stops receiving ticks.
    /// </summary>
    public void UnregisterTickable(ITickable tickable)
    {
        if (tickable == null) return;
        tickables.Remove(tickable);
    }

    /// <summary>
    /// Update ticks on every Game entity
    /// </summary>
    public void UpdateTicks()
    {
        // Apply custom tick timer
        if (enableTickManager)
        {
            timeAccumulator += Time.deltaTime;

            // Process ticks while enough time has accumulated
            while (timeAccumulator >= tickInterval)
            {
                foreach (var tickable in tickables)
                {
                    tickable.Tick();
                }
                timeAccumulator -= tickInterval;
            }
        }
        // Apply the Unity default tick timer
        else
        {
            foreach (var tickable in tickables)
            {
                tickable.Tick();
            }
        }

    }

    /// <summary>
    /// Gets the current tick interval (in seconds).
    /// </summary>
    public float TickInterval => tickInterval;

    /// <summary>
    /// Enable or disable the TickManager's control over tick timing.
    /// </summary>
    public bool EnableTickManager => enableTickManager;


    /// <summary>
    /// Sets a new tick interval at runtime (in seconds).
    /// </summary>
    public void SetTickInterval(float newInterval)
    {
        if (newInterval < 0f)
            throw new ArgumentException("Tick interval must be non-negative.");
        tickInterval = newInterval;
    }
}

