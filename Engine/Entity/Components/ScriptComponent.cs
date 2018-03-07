using Engine.Script;

namespace Engine.Entity_Component_System.Components
{
    public class ScriptComponent : IComponent
    {
        public LuaScript LuaScript;

        public IComponent Clone() => (IComponent)MemberwiseClone();

        public void Dispose()
        {
            LuaScript.OnDispose();
        }
    }

    public class ScriptComponentJson : ComponentJsonDefinition<ScriptComponent>
    {
        public string File;

        public override ScriptComponent GetComponentFromDefinition(Entity entity)
        {
            LuaScript script = LuaScriptManager.LoadScript(entity, File);
            script.OnInit();

            return new ScriptComponent
            {
                LuaScript = script
            };
        }
    }
}
