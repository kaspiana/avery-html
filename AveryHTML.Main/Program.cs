using AveryHTML;

var doc = new Document();
doc.SetTitle("Hello World Page");
doc.SetFavicon("test.png");
doc.AddStylesheet("main.css");
doc.AddScript("main.js");

var body = new ElementNode(
    "body",
    [("id", "main")]
);
doc.root.AddChild(body);
body.AddChild(new DataNode("Hello world."));
body.AddChild(new DataNode("My name is Rose."));

File.WriteAllText("output.html", doc.Render());