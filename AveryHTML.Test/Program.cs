using AveryHTML;
using NLua;
using NLua.Exceptions;

string[] files = [
    "main.lua"
];

foreach(var file in files){
    LuaContext.state.DoFile(file);
}