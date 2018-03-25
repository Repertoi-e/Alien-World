using System.Collections.Generic;

using Entitas;

using SharpDX;

namespace Alien_World.Graphics
{
    public class AnimationSystem : IExecuteSystem
    {
        GameContext m_Context;
        IGroup<GameEntity> m_AnimatableEntities;

        public AnimationSystem(GameContext context)
        {
            m_Context = context;
            m_AnimatableEntities = context.GetGroup(GameMatcher.AllOf(GameMatcher.Renderable));
        }

        void IExecuteSystem.Execute()
        {
            foreach (GameEntity entity in m_AnimatableEntities.GetEntities())
            {
                RenderableInfo info = entity.renderable.Info;
                if (info.Type == RenderableType.AnimatedSprite)
                {
                    AnimatedSprite animatedSprite = (AnimatedSprite)info.Reference;

                    animatedSprite.Update();

                    TextureRegion region = animatedSprite.GetFrame().Region;
                    animatedSprite.Texture = region.Texture;
                    animatedSprite.UVs = region.UVs;
                }

                if (info.Type != RenderableType.Text)
                {
                    Sprite sprite = (Sprite)info.Reference;

                    if (info.Flipped)
                    {
                        List<Vector2> uvsList = new List<Vector2>(sprite.UVs);
                        uvsList.Reverse(0, 2);
                        uvsList.Reverse(2, 2);
                        sprite.UVs = uvsList.ToArray();
                    }
                }
            }
        }
    }
}
