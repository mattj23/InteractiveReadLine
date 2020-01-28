using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace InteractiveReadLine.Demo
{
    public class DemoNode
    {
        private readonly Dictionary<string, DemoNode> _children;
        private readonly List<string> _childOrder;

        public DemoNode(DemoNode parent, string name, IDemo demo)
            : this(parent, name, demo?.Description)
        {
            Demo = demo;
        }

        public DemoNode(DemoNode parent, string name, string description)
        {
            Description = description;
            Name = name;
            Parent = parent;
            _children = new Dictionary<string, DemoNode>();
            _childOrder = new List<string>();
        }

        public DemoNode Parent { get; }

        public string Name { get; }

        public string Description { get; }

        public IDemo Demo { get; }

        public string Path => string.Join("/", this.GetPath(Array.Empty<string>()));

        public IReadOnlyDictionary<string, DemoNode> Children => _children;

        public string[] OrderedChildKeys => _childOrder.ToArray();

        public DemoNode AddChild(string name, string description)
        {
            var node = new DemoNode(this, name, description);
            _childOrder.Add(name);
            _children[name] = node;
            return node;
        }

        public DemoNode AddChild(string name, IDemo demo)
        {
            var node = new DemoNode(this, name, demo);
            _childOrder.Add(name);
            _children[name] = node;
            return node;
        }

        private string[] GetPath(string[] basePath)
        {
            var path = new string[]{this.Name}.Concat(basePath).ToArray();
            return this.Parent == null ? path : this.Parent.GetPath(path);
        }

        
    }
}