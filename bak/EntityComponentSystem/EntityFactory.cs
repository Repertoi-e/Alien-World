using System;
using System.Reflection;
using System.Collections.Generic;

using fastJSON;

namespace Alien_World.Entity_Component_System
{
    public static class EntityFactory
    {
        public static Entity GetFromJson(EntityContext manager, string source)
        {
            Entity entity = manager.Create();

            Dictionary<string, object> baseJson = JSON.Parse(source) as Dictionary<string, object>;
            if (!baseJson.ContainsKey("Components"))
                throw new ArgumentException("entity json definition doesn't contain components block");
            foreach (Dictionary<string, object> component in (List<object>)baseJson["Components"])
            {
                if (!component.ContainsKey("Type"))
                    throw new ArgumentException("components block doesn't contain Type identifier");

                string componentStructTypeStr = component["Type"] as string;

                // Get types with the name from json
                Type[] componentStructType = ReflectionUtils.GetTypesByName(componentStructTypeStr, true);

                // Check if type is ambiguous or non-existent
                if (componentStructType.Length > 1)
                    throw new AmbiguousMatchException($"multiple types with the name {componentStructTypeStr} exist");
                if (componentStructType.Length == 0)
                    throw new ArgumentException($"type: {componentStructTypeStr} is not defined");

                // Get method for constructing component
                MethodInfo constructComponent = typeof(ComponentIndexer).GetMethod("CreateComponentForJson").MakeGenericMethod(componentStructType);
                
                // Assign it
                MethodInfo assignCall = typeof(Entity).GetMethod("Assign").MakeGenericMethod(componentStructType);
                assignCall.Invoke(entity, new object[] { constructComponent.Invoke(null, new object[] { entity, JSON.ToJSON(component) }) });
            }
            return entity;
        }
    }
}
