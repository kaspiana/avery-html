using AveryHTML;
using NLua;

string[] files = [
    "test.lua"
];

foreach(var file in files){  
    LuaContext.state.DoFile(file);
}