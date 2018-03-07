using System;

using Engine.Entity_Component_System;

using LuaInterface;

namespace Engine.Script
{
    public class LuaScript
    {
        static LuaTable s_ThisTable = null;

        LuaTable m_Table;
        string m_TablePath;
        LuaFunction m_OnInitFunc = null;
        LuaFunction m_OnUpdateFunc = null;
        LuaFunction m_OnDisposeFunc = null;

        public string ID { get; set; }
        public string Path { get; set; }

        public Entity Parent { get; set; }

        public LuaFunction OnInitFunc { set { m_OnInitFunc = value; } }
        public LuaFunction OnUpdateFunc { set { m_OnUpdateFunc = value; } }
        public LuaFunction OnDisposeFunc { set { m_OnDisposeFunc = value; } }

        public void OnInit()
        {
            BindParent();
            SetObject();

            LuaScriptManager.PushScript(this);

            try
            {
                m_OnInitFunc?.Call();
            } catch (LuaException e)
            {
                Console.WriteLine(e.Message);
            }

            LuaScriptManager.PopScript();
            if (LuaScriptManager.PeekScript() != null)
                LuaScriptManager.PeekScript().SetObject();
        }

        public void OnUpdate(float dt)
        {
            SetObject();

            LuaScriptManager.PushScript(this);

            try
            {
                m_OnUpdateFunc?.Call(dt);
            }
            catch (LuaException e)
            {
                Console.WriteLine(e.Message);
            }

            LuaScriptManager.PopScript();
            if (LuaScriptManager.PeekScript() != null)
                LuaScriptManager.PeekScript().SetObject();
        }

        public void OnDispose()
        {
            SetObject();

            LuaScriptManager.PushScript(this);

            try
            {
                m_OnDisposeFunc?.Call();
            }
            catch (LuaException e)
            {
                Console.WriteLine(e.Message);
            }

            s_ThisTable[m_TablePath] = null;

            LuaScriptManager.PopScript();
            if (LuaScriptManager.PeekScript() != null)
                LuaScriptManager.PeekScript().SetObject();
        }

        public void BindParent()
        {
            Lua lua = LuaEngine.Instance.Lua;

            m_TablePath = "_G." + ID + ".hash." + Parent.ID.ToString();

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
