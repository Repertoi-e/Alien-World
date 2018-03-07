using System.Collections.Generic;

using Engine.Entity_Component_System;
using Engine.File_System;
using Engine.Resource_Manager;

using LuaInterface;

namespace Engine.Script
{
    public class LuaScriptManager
    {
        static LuaTable s_ThisTable;
        static Stack<LuaScript> s_Stack = new Stack<LuaScript>();

        public static void Init()
        {
            Lua lua = LuaEngine.Instance.Lua;

            lua.NewTable("this");
            s_ThisTable = lua.GetTable("this");
        }

        public static LuaScript PeekScript()
        {
            if (s_Stack.Count > 0)
                return s_Stack.Peek();
            return null;
        }

        public static void PushScript(LuaScript script)
        {
            s_Stack.Push(script);
        }

        public static void PopScript()
        {
            if (s_Stack.Count > 0)
                s_Stack.Pop();
        }

        public static LuaScript LoadScript(Entity parent, string path)
        {
            FileSystemPath fileSystemPath = FileSystemPath.Parse(path);
            if (ResourceLoader.FileSystem.Exists(fileSystemPath))
            {
                string filename = fileSystemPath.EntityName;
                int extensionIndex = filename.LastIndexOf('.');
                filename = filename.Substring(0, extensionIndex);

                Lua lua = LuaEngine.Instance.Lua;
                lua.NewTable(filename);
                lua.NewTable(filename + ".hash");

                LuaEngine.Instance.ExecuteFile(path);

                return new LuaScript
                {
                    OnInitFunc = lua.GetFunction(filename + ".Init"),
                    OnUpdateFunc = lua.GetFunction(filename + ".Update"),
                    OnDisposeFunc = lua.GetFunction(filename + ".Dispose"),
                    ID = filename,
                    Path = path,
                    Parent = parent
                };
            }

            return null;
        }
    }
}
