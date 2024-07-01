using AveryHTML;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var doc = new Document(){
    root = new DocumentNode([

        new ElementNode(
            "body",
            [("id", "main")],
            [
                new DataNode("Hello World.")
            ]
        )

    ]),
    title = "Hello World Page",
    favicon = "test.png"
};

File.WriteAllText("output.html", doc.Render());