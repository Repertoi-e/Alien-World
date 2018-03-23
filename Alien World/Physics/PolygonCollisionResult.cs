using SharpDX;

namespace Alien_World.Physics
{
    public struct PolygonCollisionResult
    {
       public bool Intersecting; // Are the polygons currently intersecting
       public bool WillIntersect; // Are the polygons going to intersect forward in time
       public Vector2 TranslationVector; // The translation to apply to polygon A to push the polygons appart.
    }
}
