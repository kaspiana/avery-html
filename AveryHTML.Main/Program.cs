using AveryHTML;

var page = new Page();
page.SetTitle("Hello World Page");
page.SetFavicon("test.png");
page.AddStylesheet("main.css");
page.AddScript("main.js");

page.root.Write(HTML.Parse(@"
    <body id=""main"" other=""meow"">
        Hello world.<br>
        My name is Rose.
    </body>
"));

page.RenderToFile("output.html");