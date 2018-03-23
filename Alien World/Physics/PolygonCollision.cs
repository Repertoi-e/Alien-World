using System;

using SharpDX;

namespace Alien_World.Physics
{
    public class PolygonCollision
    {
        public static PolygonCollisionResult Test(Polygon polygonA, Polygon polygonB, Vector2 velocity)
        {
            PolygonCollisionResult result = new PolygonCollisionResult
            {
                Intersecting = true,
                WillIntersect = true
            };

            int edgeCountA = polygonA.Edges.Count;
            int edgeCountB = polygonB.Edges.Count;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();
            Vector2 edge;

            // Loop through all the edges of both polygons
            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA)
                    edge = polygonA.Edges[edgeIndex];
                else
                    edge = polygonB.Edges[edgeIndex - edgeCountA];

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, polygonA, ref minA, ref maxA);
                ProjectPolygon(axis, polygonB, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                if (IntervalDistance(minA, maxA, minB, maxB) > 0)
                    result.Intersecting = false;

                // ===== 2. Now find if the polygons *will* intersect =====

                // Project the velocity on the current axis
                float velocityProjection = Vector2.Dot(axis, velocity);

                // Get the projection of polygon A during the movement
                if (velocityProjection < 0)
                {
                    minA += velocityProjection;
                }
                else
                {
                    maxA += velocityProjection;
                }

                // Do the same test as above for the new projection
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0)
                    result.WillIntersect = false;

                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!result.Intersecting && !result.WillIntersect)
                    break;

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = polygonA.Center - polygonB.Center;
                    if (Vector2.Dot(d, translationAxis) < 0)
                        translationAxis = -translationAxis;
                }
            }

            // The minimum translation vector can be used to push the polygons appart.
            // First moves the polygons by their velocity
            // then move polygonA by MinimumTranslationVector.
            if (result.WillIntersect)
                result.TranslationVector = translationAxis * minIntervalDistance;

            return result;
        }

        private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
                return minB - maxA;
            else
                return minA - maxB;
        }

        // Calculate the projection of a polygon on an axis and returns it as a [min, max] interval
        private static void ProjectPolygon(Vector2 axis, Polygon polygon, ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            float d = Vector2.Dot(axis, polygon.Vertices[0]);
            min = d;
            max = d;
            for (int i = 0; i < polygon.Vertices.Count; i++)
            {
                d = Vector2.Dot(polygon.Vertices[i], axis);
                if (d < min)
                    min = d;
                else if (d > max)
                    max = d;
            }
        }
    }
}
