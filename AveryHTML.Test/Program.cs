using AveryHTML;
using NLua;

Lua lua = new();

lua.LoadCLRPackage();
lua.DoString(@"
    import('AveryHTML.Lib', 'AveryHTML')
");

lua.DoFile("test.lua");