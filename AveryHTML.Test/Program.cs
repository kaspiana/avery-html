using AveryHTML;
using NLua;
using NLua.Exceptions;

string[] files = [
    "test.lua"
];

foreach(var file in files){
    LuaContext.state.DoFile(file);
}