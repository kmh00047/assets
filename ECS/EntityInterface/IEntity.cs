using System;

public interface IEntity : ITickable
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    Guid Id { get; }
}
