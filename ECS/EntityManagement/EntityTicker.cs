using System.Collections.Generic;
public class EntityTicker
{
    private readonly Ticker ticker;
    private readonly Queue<IEntity> pendingAdd = new();
    private readonly Queue<IEntity> pendingRemove = new();
    private bool ticking;

    public EntityTicker(Ticker ticker) => this.ticker = ticker;

    public void BeginTick() => ticking = true;
    public void EndTick()
    {
        ticking = false;
        Flush();
    }

    public void Register(IEntity entity)
    {
        if (ticking) pendingAdd.Enqueue(entity);
        else ticker.RegisterTickable(entity);
    }

    public void Unregister(IEntity entity)
    {
        if (ticking) pendingRemove.Enqueue(entity);
        else ticker.UnregisterTickable(entity);
    }

    private void Flush()
    {
        while (pendingRemove.Count > 0)
            ticker.UnregisterTickable(pendingRemove.Dequeue());

        while (pendingAdd.Count > 0)
            ticker.RegisterTickable(pendingAdd.Dequeue());
    }
}
