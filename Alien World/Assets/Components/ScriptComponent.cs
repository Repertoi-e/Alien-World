using Entitas;

using Alien_World.Script;

public struct ScriptComponent : IComponent
{
    public LuaScript LuaScript;

    public static LuaScript GetLuaScript(GameEntity entity, string file)
    {
        LuaScript result = LuaScriptManager.LoadScript(entity, file);
        result.OnInit();
        return result;
    }
}
