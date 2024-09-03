namespace AveryHTML;

using System.Diagnostics;
using System.Web;
using NLua;

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
            var fragNode = node as FragmentNode  ?? throw new UnreachableException();
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

    public void WriteStr(string html){
        Write(HTML.Parse(html));
    }

    public void Include(string filename){
        WriteStr(File.ReadAllText(filename));
    }

    public void WriteBefore(Node node){
        WriteAt(0, node);
    }

    public void WriteBeforeStr(string html){
        WriteBefore(HTML.Parse(html));
    }

    public void Overwrite(Node node){
        children = [];
        Write(node);
    }

    public void OverwriteStr(string html){
        Overwrite(HTML.Parse(html));
    }

    public ElementNode? Get(string id){
        
        if((this is ElementNode) && ((this as ElementNode)?.attributes.GetValueOrDefault("id", "") == id))
            return this as ElementNode;
        
        foreach(var child in this.children.OfType<ParentNode>()){
            var result = child.Get(id);
            if(result is not null)
                return result;
        }

        return null;
    }

    public void OverwriteStrInner(LuaTable table){
        foreach(string key in table.Keys){
            (Get(key) ?? throw new UnreachableException()).OverwriteStr(table[key] as string ?? throw new UnreachableException());
        }
    }

    public void IncludeInner(LuaTable table){
        foreach(string key in table.Keys){
            (Get(key) ?? throw new UnreachableException()).Include(table[key] as string ?? throw new UnreachableException());
        }
    }

    public void SetAttributeInner(LuaTable table){
        foreach(string id in table.Keys){
            var attribs = table[id] as LuaTable ?? throw new UnreachableException();
            foreach(string attribKey in attribs.Keys){
                (Get(id) ?? throw new UnreachableException()).SetAttribute(attribKey, attribs[attribKey] as string ?? throw new UnreachableException());
            }
        }
    }

    public void SetInner(LuaTable table){
        foreach(string id in table.Keys){
            var attribs = table[id] as LuaTable ?? throw new UnreachableException();
            foreach(string attribKey in attribs.Keys){
                if(attribKey == "_" || attribKey == "_w")
                    (Get(id) ?? throw new UnreachableException()).OverwriteStr(attribs[attribKey] as string ?? throw new UnreachableException());
                else if(attribKey == "_i")
                    (Get(id) ?? throw new UnreachableException()).Include(attribs[attribKey] as string ?? throw new UnreachableException());
                else
                    (Get(id) ?? throw new UnreachableException()).SetAttribute(attribKey, attribs[attribKey] as string ?? throw new UnreachableException());
            }
        }
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

    public DocumentNode(Node[]? _children = null){
        if(_children is not null)
            children = _children.ToList();
    }

    public DocumentNode(LuaTable? _children = null){
        if(_children is not null){
            children = [.. _children.Values.Cast<Node>()];
        }
    }

    public override DocumentNode Copy(){
        return new DocumentNode(children.Select(c => c.Copy()).ToArray());
    }

}

public class FragmentNode : DocumentNode {

    public FragmentNode(Node[]? _children = null) : base(_children) {
    }

    public FragmentNode(LuaTable? _children = null) : base(_children) {
    }

}

public class ElementNode : ParentNode {
    public string tag;
    public Dictionary<string, string> attributes = [];

    public ElementNode(string _tag, Dictionary<string, string>? _attributes = null, Node[]? _children = null){
        tag = _tag;
        if(_attributes is not null)
            attributes = _attributes;
        if(_children is not null)
            children = _children.ToList();
    }

    public ElementNode(string _tag, LuaTable? _attributes = null, LuaTable? _children = null){
        tag = _tag;

        if(_attributes is not null){
            foreach(string key in _attributes.Keys){
                attributes[key] = _attributes[key] as string ?? throw new UnreachableException();
            }
        }

        if(_children is not null){
            children = [.. _children.Values.Cast<Node>()];
        }
    }

    public void SetAttribute(string key, string value){
        attributes[key] = value;
    }

    public string GetAttribute(string key){
        return attributes[key];
    }

    public override string Render(){
        // special case
        if(tag == "br") return "<br>";

        return $"<{tag} {string.Join(" ", attributes.Select((t) => $"{t.Key}=\"{t.Value}\""))}>{base.Render()}</{tag}>";
    }

    public override ElementNode Copy(){
        return new ElementNode(tag, attributes.Select(t => (t.Key, t.Value)).ToDictionary(), children.Select(c => c.Copy()).ToArray());
    }
}

public class LuaNode : Node {
    public string data;

    public LuaNode(string _data){
        data = _data;
    }

    public override string Render(){
        return LuaContext.state.DoString(data)[0] as string ?? "";
    }

    public override LuaNode Copy(){
        return new LuaNode(data);
    }
}