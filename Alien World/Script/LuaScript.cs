using System;

using LuaInterface;

namespace Alien_World.Script
{
    public class LuaScript
    {
        static LuaTable s_ThisTable = null;

        LuaTable m_Table;
        string m_TablePath;

        public string ID { get; set; }
        public string Path { get; set; }

        public GameEntity Parent { get; set; }

        public LuaFunction OnInitFunc { get; set; }
        public LuaFunction OnUpdateFunc  { get; set; }
        public LuaFunction OnDisposeFunc { get; set; }

        public void CallFunction(LuaFunction func)
        {
            SetObject();

            LuaScriptManager.PushScript(this);

            try
            {
                func?.Call();
            }
            catch (LuaException e)
            {
                Console.WriteLine(e.ToString());
            }

            LuaScriptManager.PopScript();
            if (LuaScriptManager.PeekScript() != null)
                LuaScriptManager.PeekScript().SetObject();
        }

        public void BindParent()
        {
            Lua lua = LuaEngine.Instance.Lua;

            m_TablePath = "_G." + ID + ".hash." + Parent.creationIndex.ToString();

            lua.NewTable(m_TablePath);
            m_Table = lua.GetTable(m_TablePath);
            m_Table["Entity"] = Parent;
        }

        public void SetObject()
        {
            if (s_ThisTable == null)
                s_ThisTable = LuaEngine.Instance.Lua.GetTable("_G");

            s_ThisTable["this"] = m_Table;
        }
    }
}
