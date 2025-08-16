using System;
using System.Collections.Generic;
public class EntityRegistry
{
    private readonly Dictionary<Guid, IEntity> entities = new();

    public void Register(IEntity entity) => entities.Add(entity.Id, entity);
    public bool Unregister(Guid id) => entities.Remove(id);
    public IEntity Get(Guid id) => entities.TryGetValue(id, out var e) ? e : null;
    public IEnumerable<IEntity> All => entities.Values;
    public void Clear() => entities.Clear();
}
