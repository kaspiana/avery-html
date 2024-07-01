using AveryHTML;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var doc = new DocumentNode([

    new ElementNode(
        "div",
        [("id", "main_div")],
        [
            new DataNode("Hello World.")
        ]
    )

]);

Console.WriteLine(doc.Render());