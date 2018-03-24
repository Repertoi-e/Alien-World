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
            m_AnimatableEntities = context.GetGroup(GameMatcher.AllOf(GameMatcher.Sprite));
        }

        void IExecuteSystem.Execute()
        {
            foreach (GameEntity entity in m_AnimatableEntities.GetEntities())
            {
                AnimatedSprite animatedSprite = entity.sprite.GetAsAnimated;
                if (animatedSprite == null)
                    continue;

                animatedSprite.Update();

                TextureRegion region = animatedSprite.GetFrame().Region;
                ((AnimatedSprite)entity.sprite.Renderable).Texture = region.Texture;

                Vector2[] uvs = region.UVs;
                if (entity.sprite.Flipped)
                {
                    List<Vector2> uvsList = new List<Vector2>(uvs);
                    uvsList.Reverse(0, 2);
                    uvsList.Reverse(2, 2);
                    uvs = uvsList.ToArray();
                }
                ((AnimatedSprite)entity.sprite.Renderable).UVs = uvs;
            }
        }
    }
}
