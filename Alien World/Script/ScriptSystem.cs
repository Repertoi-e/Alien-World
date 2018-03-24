using Entitas;

namespace Alien_World.Script
{
    public class ScriptSystem : IExecuteSystem
    {
        GameContext m_Context;
        IGroup<GameEntity> m_ScriptEntities;

        public ScriptSystem(GameContext context)
        {
            m_Context = context;
            m_ScriptEntities = context.GetGroup(GameMatcher.AllOf(GameMatcher.Script));
        }

        void IExecuteSystem.Execute()
        {
            foreach (GameEntity entity in m_ScriptEntities.GetEntities())
            {
                LuaScript script = entity.script.LuaScript;
                script.CallFunction(script.OnUpdateFunc);
            }
        }
    }
}
