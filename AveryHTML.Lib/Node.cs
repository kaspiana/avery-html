namespace AveryHTML;

using System.Web;

public abstract class Node {
    public ParentNode? parent = null;

    public abstract string Render();
}

public abstract class ParentNode : Node {
    public List<Node> children = [];

    public override string Render(){
        return string.Join("", children.Select(c => c.Render()));
    }

    public Node AddChild(Node child){
        children.Add(child);
        return child;
    }

    public void AddChildren(IEnumerable<Node> _children){
        children.AddRange(_children);
    }
}

public class DataNode : Node {
    public string data = "";

    public DataNode(string _data){
        data = _data;
    }

    public override string Render() => HttpUtility.HtmlEncode(data);
}

public class DocumentNode : ParentNode {

    public DocumentNode(IEnumerable<Node>? _children = null){
        if(_children is not null)
            children = _children.ToList();
    }

}

public class ElementNode : ParentNode {
    public string tag;
    public (string, string)[] attributes = [];

    public ElementNode(string _tag, (string, string)[]? _attributes = null, IEnumerable<Node>? _children = null){
        tag = _tag;
        if(_attributes is not null)
            attributes = _attributes;
        if(_children is not null)
            children = _children.ToList();
    }

    public override string Render(){
        return $"<{tag} {string.Join("", attributes.Select(t => $"{t.Item1}=\"{t.Item2}\""))}>{base.Render()}</{tag}>";
    }
}