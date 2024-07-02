using AveryHTML;

var page = new Page();
page.SetTitle("Hello World Page");
page.SetFavicon("test.png");
page.AddStylesheet("main.css");
page.AddScript("main.js");

var body = new ElementNode("body", [("id", "main")]);
body.Write(new DataNode("Hello world."));
body.Write(new DataNode("My name is <Rose>."));

page.root.Write(body);

page.RenderToFile("output.html");