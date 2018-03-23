using System;

using LuaInterface;

namespace Alien_World.Script
{
    public class LuaEngine
    {
        static LuaEngine s_Instance;
        public static LuaEngine Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new LuaEngine();
                return s_Instance;
            }
        }

        Lua m_LuaState = null;
        public Lua Lua { get { return m_LuaState; } }

        private LuaEngine() { }

        public void Init()
        {
            if (m_LuaState != null)
                throw new InvalidOperationException("lua engine already initialized");

            m_LuaState = new Lua();
            m_LuaState["Program"] = App.Application.Instance;
            m_LuaState["InputManager"] = App.InputManager.Instance;
            m_LuaState["Log"] = App.Application.Logger;
        }

        public void ExecuteFile(string file)
        {
            string source = Resource_Manager.ResourceLoader.LoadTextFile(file);

            try
            {
                m_LuaState.DoString(source);
            }
            catch (LuaException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
