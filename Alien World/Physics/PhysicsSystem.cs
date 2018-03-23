using System.Collections.Generic;

using Entitas;

using SharpDX;

namespace Alien_World.Physics
{
    public class MovementSystem : IExecuteSystem
    {
        GameContext m_Context;
        IGroup<GameEntity> m_MovableEntities;

        public MovementSystem(GameContext context)
        {
            m_Context = context;
            m_MovableEntities = context.GetGroup(GameMatcher.AllOf(GameMatcher.Position, GameMatcher.Velocity));
        }

        void IExecuteSystem.Execute()
        {
            foreach (GameEntity entity in m_MovableEntities.GetEntities())
            {
                if (entity.isStaticBody)
                    continue;

                PositionComponent position = entity.position;
                position.X += entity.velocity.X;
                position.Y += entity.velocity.Y;
                entity.ReplacePosition(position.X, position.Y);

                if (entity.hasCollision)
                    entity.collision.CollisionBounds.ApplyOffset(entity.velocity);
            }
        }
    }

    public class CollisionSystem : IExecuteSystem
    {
        GameContext m_Context;
        IGroup<GameEntity> m_ColliadableEntities;

        public CollisionSystem(GameContext context)
        {
            m_Context = context;
            m_ColliadableEntities = context.GetGroup(GameMatcher.AllOf(GameMatcher.Velocity, GameMatcher.Collision));
        }

        void IExecuteSystem.Execute()
        {
            GameEntity[] entities = m_ColliadableEntities.GetEntities();
            for (int i = 0; i < entities.Length - 1; i++)
            {
                GameEntity a = entities[i];
                List<PolygonCollisionResult> collisionResults = new List<PolygonCollisionResult>();
                for (int j = i + 1; j < entities.Length; j++)
                {
                    GameEntity b = entities[j];
                    if (a.isStaticBody)
                    {
                        if (b.isStaticBody)
                            continue;
                        GameEntity temp = b;
                        b = a;
                        a = temp;
                    }

                    PolygonCollisionResult collisionResult = PolygonCollision.Test(a.collision, b.collision, a.velocity);
                    if (collisionResult.WillIntersect)
                    {
                        collisionResults.Add(collisionResult);

                        Vector2 translation = collisionResult.TranslationVector;
                        Vector2 velocity = a.velocity;
                        velocity += translation;
                        a.ReplaceVelocity(velocity);
                    }
                }
                a.ReplaceCollision(a.collision, collisionResults);
            }
        }
    }
}
