using System;
using UnityEngine;  // for Vector2Int

namespace Assets.Entities
{
    public abstract class BaseEntity : IEntity
    {
        public Guid Id { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Vector2Int position;

        // Event fired when Position changes: (oldPos, newPos)
        public event Action<BaseEntity, Vector2Int, Vector2Int> OnPositionChanged;

        public Vector2Int Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    var oldPos = position;
                    position = value;
                    OnPositionChanged?.Invoke(this, oldPos, position);
                }
            }
        }

        public BaseEntity()
        {
            Id = Guid.NewGuid();
            OnCreated();
        }

        public virtual void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                OnActivated();
            }
        }

        public virtual void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                OnDeactivated();
            }
        }

        public virtual void Destroy()
        {
            OnDestroyed();
        }

        public virtual void OnCreated() { }
        public virtual void OnActivated() { }
        public virtual void OnDeactivated() { }
        public virtual void OnDestroyed() { }

        public abstract void Tick();
    }
}
