//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly StaticBodyComponent staticBodyComponent = new StaticBodyComponent();

    public bool isStaticBody {
        get { return HasComponent(GameComponentsLookup.StaticBody); }
        set {
            if (value != isStaticBody) {
                if (value) {
                    AddComponent(GameComponentsLookup.StaticBody, staticBodyComponent);
                } else {
                    RemoveComponent(GameComponentsLookup.StaticBody);
                }
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherStaticBody;

    public static Entitas.IMatcher<GameEntity> StaticBody {
        get {
            if (_matcherStaticBody == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.StaticBody);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherStaticBody = matcher;
            }

            return _matcherStaticBody;
        }
    }
}
