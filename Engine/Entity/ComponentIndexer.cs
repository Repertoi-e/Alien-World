using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

using fastJSON;

namespace Engine.Entity_Component_System
{
    public static class ComponentIndexer
    {
        private delegate object JsonDefCtorDelegate();

        static Dictionary<Type, int> m_ComponentIDs = new Dictionary<Type, int>();
        static Dictionary<Type, Tuple<JsonDefCtorDelegate, Type>> m_ComponentJsonDefCtors = new Dictionary<Type, Tuple<JsonDefCtorDelegate, Type>>();
        static bool m_Indexed = false;

        static MethodInfo m_JsonToObjectCall;

        public static bool Indexed => m_Indexed;

        public static void Index()
        {
            int identifier = 0;
            // Search for all components in the assembly
            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(t => t.GetInterfaces().Contains(typeof(IComponent))))
            {
                m_ComponentIDs[type] = identifier++;
                m_ComponentJsonDefCtors[type] = GetComponentJsonDefConstructor(type);
            }

            // Get ToObject generic method from JSON 
            foreach (MethodInfo method in typeof(JSON).GetMethods().Where(m => m.Name == "ToObject"))
                if (method.IsGenericMethod && method.GetParameters().Length == 1)
                {
                    m_JsonToObjectCall = method;
                    break;
                }

            m_Indexed = true;
        }

        // Creates a dynamic method to construct a component json definition class 
        private static Tuple<JsonDefCtorDelegate, Type> GetComponentJsonDefConstructor(Type componentStructType)
        {
            // Find the type
            Type type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(t =>
            {
                Type baseType = t.BaseType;
                return baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(ComponentJsonDefinition<>)
                        && baseType.GetGenericArguments()[0] == componentStructType;
            });

            // If the component doesn't have a json def type
            if (type == null)
                return null;

            // *Do some magic*
            ConstructorInfo ctor = type.GetConstructor(new Type[0]);

            DynamicMethod dm = new DynamicMethod(type.Name + "Ctor", type, new Type[0], typeof(Activator));
            ILGenerator ilgen = dm.GetILGenerator();
            ilgen.Emit(OpCodes.Newobj, ctor);
            ilgen.Emit(OpCodes.Ret);

            return new Tuple<JsonDefCtorDelegate, Type>((JsonDefCtorDelegate)dm.CreateDelegate(typeof(JsonDefCtorDelegate)), type);
        }

        // Cache component json tuples
        static class ComponentJsonCache<C>
        {
            public static readonly Tuple<JsonDefCtorDelegate, Type> TypeTuple = m_ComponentJsonDefCtors[typeof(C)];
            public static readonly JsonDefCtorDelegate Item1 = TypeTuple?.Item1;
            public static readonly Type Item2 = TypeTuple?.Item2;
        }

        public static object CreateComponentForJson<C>(Entity entity, string componentJson)
        {
            // Attempting to create a component from json which doesn't have a json def class
            if (ComponentJsonCache<C>.TypeTuple == null)
                throw new ArgumentException($"type {typeof(C).Name} doesn't have a json def type definition");

            // Create the component from the "component from json" def
            object componentDefObj = ComponentJsonCache<C>.Item1();
            componentDefObj = m_JsonToObjectCall.MakeGenericMethod(ComponentJsonCache<C>.Item2).Invoke(null, new object[] { componentJson });
            return componentDefObj.GetType().GetMethod("GetComponentFromDefinition").Invoke(componentDefObj, new object[] { entity });
        }

        // Cache component IDs
        static class ComponentFamilyCache<C>
        {
            public static readonly int Family = m_ComponentIDs[typeof(C)];
        }

        public static int GetFamily<C>()
        {
            return ComponentFamilyCache<C>.Family;
        }
    }
}
