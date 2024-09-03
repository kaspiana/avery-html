using System.Diagnostics;
using System.Xml;

namespace AveryHTML;

public class HTML {

    private static Node XMLNodeToNode(XmlNode xmlNode){
        
        if(xmlNode.NodeType == XmlNodeType.Text){
            var xmlTextNode = xmlNode as XmlText ?? throw new UnreachableException();
            return new DataNode(xmlTextNode.InnerText);
        }

        if(xmlNode.NodeType == XmlNodeType.Element){
            var xmlElNode = xmlNode as XmlElement ?? throw new UnreachableException();;

            if(xmlElNode.Name == "script"){
                if(xmlElNode.Attributes["type"]?.Value == "text/lua"){
                    if(xmlElNode.Attributes["src"] is not null){
                        return new LuaNode(File.ReadAllText(xmlElNode.Attributes["src"]?.Value ?? ""));
                    } else {
                        return new LuaNode(xmlElNode.InnerText);
                    }
                }
            } else if(xmlElNode.Name == "var"){
                return new LuaNode("return " + xmlElNode.InnerText);
            }

            var children = new Node[xmlElNode.ChildNodes.Count];
            for(var i = 0; i < xmlElNode.ChildNodes.Count; i++){
                var child = xmlElNode.ChildNodes[i] ?? throw new UnreachableException();
                children[i] = XMLNodeToNode(child);
            }

            var attributes = new (string Key, string Value)[xmlElNode.Attributes.Count];
            for(var i = 0; i < xmlElNode.Attributes.Count; i++){
                var attrib = xmlElNode.Attributes[i];
                attributes[i] = (attrib.Name, attrib.Value);
            }

            return new ElementNode(xmlElNode.Name, attributes.ToDictionary(), children);
        }

        throw new UnreachableException();
    }

    public static FragmentNode Parse(string data){

        // hacky HTML -> XHTML tidying
        data = data.Replace("<br>", "<br />");
        data = $"<root>{data}</root>";

        var xml = new XmlDocument();
        xml.LoadXml(data);

        if(xml.DocumentElement is null)
            return new FragmentNode([]);

        var node = XMLNodeToNode(xml.DocumentElement) as ParentNode  ?? throw new UnreachableException();

        return new FragmentNode(node.children.ToArray());
    }

    public static FragmentNode ParseFile(string filename){
        return Parse(File.ReadAllText(filename));
    }

}