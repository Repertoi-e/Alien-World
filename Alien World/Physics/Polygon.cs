using System.Collections.Generic;
using System.Linq;

using SharpDX;

namespace Alien_World.Physics
{
    public class Polygon
    {
        public List<Vector2> Vertices { get; }
        public List<Vector2> Edges { get; }
        public Vector2 Center { get
            {
                float totalX = 0, totalY = 0;
                foreach (Vector2 vertex in Vertices)
                {
                    totalX += vertex.X;
                    totalY += vertex.Y;
                }

                return new Vector2(totalX / (float)Vertices.Count, totalY / (float)Vertices.Count);
            } }

        public Polygon(float xo, float yo, Vector2[] vertices)
            : this(vertices)
        {
            ApplyOffset(xo, yo);
        }

        public Polygon(Vector2 offset, Vector2[] vertices)
            : this(vertices)
        {
            ApplyOffset(offset);
        }

        public Polygon(Vector2[] vertices)
        {
            Vertices = vertices.ToList();
            BuildEdges();
        }

        public Polygon(Vector2 offset, float hw, float hh)
            : this(hw, hh)
        {
            ApplyOffset(offset);
        }

        public Polygon(float xo, float yo, float hw, float hh)
            : this(hw, hh)
        {
            ApplyOffset(xo, yo);
        }

        public Polygon(float hw, float hh)
        {
            Vertices = new List<Vector2>(4)
            {
                new Vector2(-hw, -hh),
                new Vector2(hw, -hh),
                new Vector2(hw, hh),
                new Vector2(-hw, hh)
            };
            Edges = new List<Vector2>(Vertices.Count);
            BuildEdges();
        }

        void BuildEdges()
        {
            int size = Vertices.Count;
            for (int i = 0; i < size; i++)
                Edges.Add(Vertices[(i + 1) % size] - Vertices[i]);
        }

        public void ApplyOffset(Vector2 offset)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vector2 p = Vertices[i];
                Vertices[i] = p + offset;
            }
        }

        public void ApplyOffset(float x, float y)
        {
            ApplyOffset(new Vector2(x, y));
        }
    }
}
