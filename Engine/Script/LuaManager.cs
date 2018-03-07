using System;
using System.Reflection;
using System.Collections.Generic;

namespace Engine.Script
{
    /*
    [Obsolete]
    public struct LuaFunctionDescriptor
    {
        public string Name { get; }
        public string Doc { get; }
        public Dictionary<string, string> Parameters { get; }
        public string DocString { get; }
        public string Header { get
            {
                if (DocString.IndexOf("\n") == -1)
                    return DocString;

                return DocString.Substring(0, DocString.IndexOf("\n"));
            } }

        public LuaFunctionDescriptor(string name, string doc, Dictionary<string, string> parameters)
        {
            Name = name;
            Doc = doc;
            Parameters = parameters;

            string funcBody = "\n\n";
            string funcParams = "";

            bool first = true;

            foreach (KeyValuePair<string, string> param in parameters)
            {
                if (!first)
                    funcParams += ", ";

                funcParams += param.Key;
                funcBody += "\t" + param.Key + "\t\t" + param.Value + "\n";

                first = false;
            }

            funcBody = funcBody.Substring(0, funcBody.Length - 1);
            if (first)
                funcBody = funcBody.Substring(0, funcBody.Length - 1);

            string funcHeader = name + "(%params%) - " + doc;
            DocString = funcHeader.Replace("%params%", funcParams) + funcBody;
        }
    }

    [Obsolete]
    public class LuaFunction : Attribute
    {
        public string Name { get; }
        public string Doc { get; }
        public string[] Parameters { get; } = null;

        public LuaFunction(string name, string doc)
        {
            Name = name;
            Doc = doc;
        }

        public LuaFunction(string name, string doc, params string[] parameters)
        {
            Name = name;
            Doc = doc;
            Parameters = parameters;
        }
    }

    [Obsolete]
    public static class LuaManager
    {
        public static LuaInterface.Lua LuaVM { get; } = new LuaInterface.Lua();
        public static Dictionary<string, LuaFunctionDescriptor> LuaFuncs { get; } = new Dictionary<string, LuaFunctionDescriptor>();

        public static void RegisterLuaFunctions(object target)
        {
            Type type = target.GetType();
            foreach (MethodInfo info in type.GetMethods())
            {
                LuaFunction attribute = info.GetCustomAttribute<LuaFunction>();
                if (attribute == null)
                    continue;

                string name = attribute.Name;
                string doc = attribute.Doc;
                string[] paramDoc = attribute.Parameters;

                // Get the expected parameters from the MethodInfo object
                ParameterInfo[] paramInfo = info.GetParameters();

                // If they don't match, someone forgot to add some documentation to the attribute
                if (paramDoc != null && (paramInfo.Length != paramDoc.Length))
                {
                    Console.WriteLine($"Function {info.Name} (exported as {name}): Argument number mismatch. Declared {paramDoc.Length}" +
                        $"but requires {paramInfo.Length}.");
                    break;
                }

                Dictionary<string, string> paramateres = new Dictionary<string, string>();
                for (int i = 0; i < paramInfo.Length; i++)
                    paramateres.Add(paramInfo[i].Name, paramDoc[i]);

                // Get a new function descriptor from this information
                LuaFunctionDescriptor desc = new LuaFunctionDescriptor(name, doc, paramateres);
                LuaFuncs.Add(name, desc);

                LuaVM.RegisterFunction(name, target, info);
            }
        }

        public static void RegisterLuaFunctions(Type type)
        {
            foreach (MethodInfo info in type.GetMethods())
            {
                LuaFunction attribute = info.GetCustomAttribute<LuaFunction>();
                if (attribute == null)
                    continue;

                string name = attribute.Name;
                string doc = attribute.Doc;
                string[] paramDoc = attribute.Parameters;

                // Get the expected parameters from the MethodInfo object
                ParameterInfo[] paramInfo = info.GetParameters();

                // If they don't match, someone forgot to add some documentation to the attribute
                if (paramDoc != null && (paramInfo.Length != paramDoc.Length))
                {
                    Console.WriteLine($"Function {info.Name} (exported as {name}): Argument number mismatch. Declared {paramDoc.Length}" +
                        $"but requires {paramInfo.Length}.");
                    break;
                }

                Dictionary<string, string> paramateres = new Dictionary<string, string>();
                for (int i = 0; i < paramInfo.Length; i++)
                    paramateres.Add(paramInfo[i].Name, paramDoc[i]);

                // Get a new function descriptor from this information
                LuaFunctionDescriptor desc = new LuaFunctionDescriptor(name, doc, paramateres);
                LuaFuncs.Add(name, desc);

                LuaVM.RegisterFunction(name, null, info);
            }
        }
    }

    [Obsolete]
    public static class LuaHelp
    {
        static LuaHelp()
        {
            LuaManager.RegisterLuaFunctions(typeof(LuaHelp));
        }

        [LuaFunction("help", "List available commands.")]
        public static void Help()
        {
            Console.WriteLine("Available commands:\n");
            foreach (KeyValuePair<string, LuaFunctionDescriptor> func in LuaManager.LuaFuncs)
                Console.WriteLine(func.Value.Header);
        }

        [LuaFunction("helpcmd", "Show help for a given command", "Command to get help of.")]
        public static void Help(string cmd)
        {
            if (!LuaManager.LuaFuncs.ContainsKey(cmd))
            {
                Console.WriteLine("No such function or package: " + cmd);
                return;
            }
            Console.WriteLine(LuaManager.LuaFuncs[cmd].DocString);
        }
    }
    */
}
