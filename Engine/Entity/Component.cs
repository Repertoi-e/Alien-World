using System;

namespace Engine.Entity_Component_System
{
    public interface IComponent : IDisposable
    {
        IComponent Clone();
    }

    public abstract class ComponentJsonDefinition<T> where T : IComponent
    {
        public Type DefinedType = typeof(T);

        public abstract T GetComponentFromDefinition(Entity entity);
    }

    public abstract class BaseComponentHelper
    {
        public abstract void RemoveComponent(Entity entity);
        public abstract void CopyComponentTo(Entity source, Entity target);
    }

    class ComponentHelper<C> : BaseComponentHelper where C : class, IComponent, new()
    {
        public override void RemoveComponent(Entity entity) => entity.Remove<C>();
        public override void CopyComponentTo(Entity source, Entity target) => target.Assign(source.Component<C>());
    };
}
