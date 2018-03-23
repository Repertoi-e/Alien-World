using System.Collections.Generic;

using Alien_World.Physics;

using Entitas;

public struct CollisionComponent : IComponent
{
    public Polygon CollisionBounds;
    public List<PolygonCollisionResult> LastCollisionResults;

    public static implicit operator Polygon(CollisionComponent cb) { return cb.CollisionBounds; }
}
