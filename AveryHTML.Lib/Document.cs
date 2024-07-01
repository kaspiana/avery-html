namespace AveryHTML;

public class Document {
    public DocumentNode root = new([]);
    public string title;
    public string? favicon;
    public string[] stylesheets = [];
    public string[] scripts = [];

    public string Render(){

        return 
$@"<!DOCTYPE html>
<html>
    <head>
        <title>{title}</title>
        <link rel=""shortcut icon"" href=""{favicon}"">
        {string.Join("", stylesheets.Select((s) => $"<link href=\"{s}\" rel=\"stylesheet\" type=\"text/css\">"))}
        {string.Join("", scripts.Select((s) => $"<script src=\"{s}\" type=\"text/javascript\"></script>"))}
    </head>
    {root.Render()}
</html>
<!-- BUILT AT {DateTime.Now} -->";

    }
}