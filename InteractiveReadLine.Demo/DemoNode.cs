using System.Collections.Generic;
using System.Security.Cryptography;

namespace InteractiveReadLine.Demo
{
    public class DemoNode
    {
        private readonly Dictionary<string, DemoNode> _children;
        private readonly List<string> _childOrder;

        public DemoNode(DemoNode parent, IDemo demo)
            : this(parent, demo?.Description)
        {
            Demo = demo;
        }

        public DemoNode(DemoNode parent, string description)
        {
            Description = description;
            Parent = parent;
            _children = new Dictionary<string, DemoNode>();
            _childOrder = new List<string>();
        }

        public DemoNode Parent { get; }

        public string Description { get; }

        public IDemo Demo { get; }

        public IReadOnlyDictionary<string, DemoNode> Children => _children;

        public DemoNode AddChild(string name, string description)
        {
            var node = new DemoNode(this, description);
            _childOrder.Add(name);
            _children[name] = node;
            return node;
        }

        public DemoNode AddChild(string name, IDemo demo)
        {
            var node = new DemoNode(this, demo);
            _childOrder.Add(name);
            _children[name] = node;
            return node;
        }

        
    }
}