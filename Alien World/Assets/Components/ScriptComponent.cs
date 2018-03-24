using Entitas;

using Alien_World.Script;

public struct ScriptComponent : IComponent
{
    public LuaScript LuaScript;

    public static LuaScript GetLuaScript(GameEntity entity, string file)
    {
        LuaScript result = LuaScriptManager.LoadScript(entity, file);
        result.BindParent();
        result.CallFunction(result.OnInitFunc);

        entity.OnDestroyEntity += (IEntity e) =>
        {
            if (e is GameEntity gameE)
                if (gameE.hasScript)
                {
                    LuaScript script = gameE.script.LuaScript;
                    script.CallFunction(script.OnDisposeFunc);
                }
        };

        entity.OnComponentReplaced += (IEntity e, int index, IComponent previousComponent, IComponent component) =>
        {
            if (previousComponent is ScriptComponent script)
                script.LuaScript.CallFunction(script.LuaScript.OnDisposeFunc);
        };

        entity.OnComponentRemoved += (IEntity e, int index, IComponent component) =>
        {
            if (component is ScriptComponent script)
                script.LuaScript.CallFunction(script.LuaScript.OnDisposeFunc);
        };
        return result;
    }
}
