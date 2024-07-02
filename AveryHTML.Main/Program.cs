using AveryHTML;

var page = new Page();
page.SetTitle("Hello World Page");
page.SetFavicon("test.png");
page.AddStylesheet("main.css");
page.AddScript("main.js");

var body = new ElementNode("body", [
    ("id", "main")
]);
page.root.AddChild(body);
body.AddChild(new DataNode("Hello world."));
body.AddChild(new DataNode("My name is <Rose>."));

page.RenderToFile("output.html");