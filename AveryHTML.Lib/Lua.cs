using System.Text;
using NLua;

namespace AveryHTML;

public class LuaContext {

    public static Lua state = new();

    static LuaContext(){
        state.State.Encoding = Encoding.UTF8;
        state.LoadCLRPackage();
        state.DoString(@"
            import('AveryHTML.Lib', 'AveryHTML')
        ");
    }

}