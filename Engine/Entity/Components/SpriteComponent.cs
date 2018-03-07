using System;

using Engine.Graphics;
using Engine.Resource_Manager;

namespace Engine.Entity_Component_System.Components
{
    public class SpriteComponent : IComponent
    {
        public Renderable2D Value;

        public SpriteComponent() { Value = null; }

        public SpriteComponent(Renderable2D value)
        {
            Value = value;
        }
    
        public IComponent Clone() => (IComponent)MemberwiseClone();
        public void Dispose() { }
    }

    public class SpriteComponentJson : ComponentJsonDefinition<SpriteComponent>
    {
        public float[] Size { get; set; }
        public string SpriteType { get; set; }
        public string Resource { get; set; }
    
        public override SpriteComponent GetComponentFromDefinition(Entity entity)
        {
            if (SpriteType == null)
                throw new NullReferenceException("sprite type");
    
            if (SpriteType.ToLower().Equals("static"))
            {
                if (Resource == null)
                    throw new NullReferenceException("static type, resource");
                if (ResourceManager<Texture>.Get(Resource) is null)
                    ResourceManager<Texture>.Add(ResourceLoader.LoadTexture(Resource, TextureFilter.NEAREST));
                return new SpriteComponent
                {
                    Value = new Renderable2D(new SharpDX.Vector2(Size[0], Size[1]), ResourceManager<Texture>.Get(Resource))
                };
            }
            return new SpriteComponent
            {
                Value = new Renderable2D(new SharpDX.Vector2(Size[0], Size[1]), 0xffff00ff)
            };
        }
    }
}
