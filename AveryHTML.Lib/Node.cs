namespace AveryHTML;

public abstract class Node {
    public ParentNode? parent = null;

    public abstract string Render();
}

public abstract class ParentNode : Node {
    public Node[] children = [];

    public override string Render(){
        return string.Join("", children.Select(c => c.Render()));
    }
}

public class DataNode : Node {
    public string data = "";

    public DataNode(string _data){
        data = _data;
    }

    public override string Render() => data; // TODO: Handle escaping characters
}

public class DocumentNode : ParentNode {

    public DocumentNode(Node[] _children){
        children = _children;
    }

}

public class ElementNode : ParentNode {
    public string tag;
    public (string, string)[] attributes = [];

    public ElementNode(string _tag, (string, string)[] _attributes, Node[] _children){
        tag = _tag;
        attributes = _attributes;
        children = _children;
    }

    public override string Render(){
        return $"<{tag} {string.Join("", attributes.Select(t => $"{t.Item1}=\"{t.Item2}\""))}>{base.Render()}</{tag}>";
    }
}