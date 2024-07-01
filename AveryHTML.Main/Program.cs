using AveryHTML;

var doc = new Document(){
    root = new DocumentNode([

        new ElementNode(
            "body",
            [("id", "main")],
            [
                new DataNode("Hello World.")
            ]
        )

    ])
};

doc.SetTitle("Hello World Page");
doc.SetFavicon("test.png");
doc.AddStylesheet("main.css");
doc.AddScript("main.js");

File.WriteAllText("output.html", doc.Render());