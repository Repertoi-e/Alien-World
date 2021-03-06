﻿using System;
using System.Collections.Generic;

using SharpDX;

namespace Alien_World.Graphics
{
    public class AnimatedSprite : Sprite, IRenderable2D
    {
        public struct Frame
        {
            public TextureRegion Region;
            public uint Duration;
        }

        private int m_Timer = 0, m_CurrentFrame = 0, m_Direction = 1;
        private bool m_Playing = false;
        List<Frame> m_Frames;

        public AnimatedSprite(Vector2 size, Frame[] frames)
            : base(size, frames[0].Region.Texture)
        {
            m_Frames = new List<Frame>(frames.Length);
            m_Frames.AddRange(frames);
        }

        public AnimatedSprite(Vector2 size, TextureRegion[] textures, uint frameDuration)
            : base(size, textures[0].Texture)
        {
            m_Frames = new List<Frame>(textures.Length);
            foreach (TextureRegion texture in textures)
                m_Frames.Add(new Frame
                {
                    Region = texture,
                    Duration = frameDuration
                });
        }

        public AnimatedSprite Start()
        {
            m_Playing = true;
            return this;
        }

        public AnimatedSprite Stop()
        {
            m_Playing = false;
            return this;
        }

        public AnimatedSprite Restart()
        {
            m_Playing = true;
            m_CurrentFrame = 0;
            return this;
        }

        public void SetFrame(int frame)
        {
            if (frame < 0 || frame >= m_Frames.Count)
                throw new IndexOutOfRangeException("Invalid frame index.");

            m_CurrentFrame = frame;
        }

        public AnimatedSprite Reset()
        {
            m_Playing = false;
            m_Timer = 0;
            m_CurrentFrame = 0;
            return this;
        }

        public Frame GetFrame()
        {
            return m_Frames[m_CurrentFrame];
        }

        public void Update()
        {
            if (m_Playing)
            {
                m_Timer++;

                if (m_Timer > m_Frames[m_CurrentFrame].Duration)
                {
                    m_Timer = 0;
                    m_CurrentFrame = (m_CurrentFrame + m_Direction) % m_Frames.Count;

                    TextureRegion newRegion = m_Frames[m_CurrentFrame].Region;

                    m_Texture = newRegion.Texture;
                    m_UVs = newRegion.UVs;
                }
            }
        }
    }
}
