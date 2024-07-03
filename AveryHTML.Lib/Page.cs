using NLua;

namespace AveryHTML;

public class Page {
    public DocumentNode root = new([]);
    public Lua luaState = new();
    public string? title;
    public string? favicon;
    public List<string> stylesheets = [];
    public List<string> scripts = [];

    public Page(){
        luaState.LoadCLRPackage();
        luaState.DoString(@"
            import('AveryHTML.Lib', 'AveryHTML')
        ");
    }

    public Page(Lua _luaState){
        luaState = _luaState;
    }

    public void SetTitle(string _title){
        title = _title;
    }

    public void SetFavicon(string _favicon){
        favicon = _favicon;
    }

    public void AddStylesheet(string stylesheet){
        stylesheets.Add(stylesheet);
    }

    public void AddScript(string script){
        scripts.Add(script);
    }

    public string Render(){

        return 
$@"<!DOCTYPE html>
<html>
    <head>
        {(title is not null ? $"<title>{title}</title>" : "")}
        {(favicon is not null ? $"<link rel=\"shortcut icon\" href=\"{favicon}\" />" : "")}
        {string.Join("", stylesheets.Select((s) => $"<link href=\"{s}\" rel=\"stylesheet\" type=\"text/css\" />"))}
        {string.Join("", scripts.Select((s) => $"<script src=\"{s}\" type=\"text/javascript\"></script>"))}
    </head>
    {root.Render(this)}
</html>
<!-- BUILT AT {DateTime.Now} -->";

    }

    public void RenderToFile(string filename){
        File.WriteAllText(filename, Render());
    }
}