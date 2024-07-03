using NLua;

namespace AveryHTML;

public class LuaContext {

    public static Lua state = new();

    static LuaContext(){
        state.LoadCLRPackage();
        state.DoString(@"
            import('AveryHTML.Lib', 'AveryHTML')
        ");
    }

}