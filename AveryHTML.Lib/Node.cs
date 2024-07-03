namespace AveryHTML;

using System.Web;

public abstract class Node {
    public ParentNode? parent = null;

    public abstract string Render();
    public abstract Node Copy();

    public void Reparent(ParentNode? newParent){
        if(parent is not null){
            parent.children.Remove(this);
        }

        parent = newParent;
    }
}

public abstract class ParentNode : Node {
    public List<Node> children = [];

    public override string Render(){
        return string.Join("", children.Select(c => c.Render()));
    }

    public void WriteAt(int index, Node node){
        if(node is FragmentNode){
            var fragNode = node as FragmentNode;
            var i = 0;
            foreach(var child in fragNode.children){
                children.Insert(index + (i++), child);
                child.Reparent(this);
            }
        } else {
            children.Insert(index, node);
            node.Reparent(this);
        }
    }

    public void WriteAt(int index, string html){
        WriteAt(index, HTML.Parse(html));
    }

    public void Write(Node node){
        WriteAt(children.Count, node);
    }

    public void Write(string html){
        Write(HTML.Parse(html));
    }

    public void WriteBefore(Node node){
        WriteAt(0, node);
    }

    public void WriteBefore(string html){
        WriteBefore(HTML.Parse(html));
    }

    public void Overwrite(Node node){
        children = [];
        Write(node);
    }

    public void Overwrite(string html){
        Overwrite(HTML.Parse(html));
    }
}

public class DataNode : Node {
    public string data = "";

    public DataNode(string _data){
        data = _data;
    }

    public override string Render() => HttpUtility.HtmlEncode(data);

    public override DataNode Copy(){
        return new DataNode(data);
    }
}

public class DocumentNode : ParentNode {

    public DocumentNode(IEnumerable<Node>? _children = null){
        if(_children is not null)
            children = _children.ToList();
    }

    public override DocumentNode Copy(){
        return new DocumentNode(children.Select(c => c.Copy()));
    }

}

public class FragmentNode : DocumentNode {

    public FragmentNode(IEnumerable<Node>? _children = null){
        if(_children is not null)
            children = _children.ToList();
    }

}

public class ElementNode : ParentNode {
    public string tag;
    public Dictionary<string, string> attributes = [];

    public ElementNode(string _tag, IEnumerable<(string Key, string Value)>? _attributes = null, IEnumerable<Node>? _children = null){
        tag = _tag;
        if(_attributes is not null)
            attributes = _attributes.ToDictionary();
        if(_children is not null)
            children = _children.ToList();
    }

    public override string Render(){
        return $"<{tag} {string.Join(" ", attributes.Select((t) => $"{t.Key}=\"{t.Value}\""))}>{base.Render()}</{tag}>";
    }

    public override ElementNode Copy(){
        return new ElementNode(tag, attributes.Select(t => (t.Key, t.Value)), children.Select(c => c.Copy()));
    }
}