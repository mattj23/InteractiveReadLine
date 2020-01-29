using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace InteractiveReadLine.Demo
{
    public class OptionSet
    {
        private List<string> _keys;
        private Dictionary<string, Action> _actions;
        private Dictionary<string, string> _descriptions;

        public OptionSet()
        {
            _keys = new List<string>();
            _actions = new Dictionary<string, Action>();
            _descriptions = new Dictionary<string, string>();
        }

        public string[] Keys => _keys.ToArray();

        public string[] Descriptions => _keys.Select(GetDescription).ToArray();

        public void Add(string key, string description, Action action)
        {
            if (_actions.ContainsKey(key))
                throw new DuplicateNameException();
            _keys.Add(key);
            _descriptions[key] = description;
            _actions[key] = action;
        }

        public void AddToStart(string key, string description, Action action)
        {
            if (_actions.ContainsKey(key))
                throw new DuplicateNameException();
            _keys.Insert(0, key);
            _descriptions[key] = description;
            _actions[key] = action;
        }

        public string GetDescription(string key) => _descriptions[key];

        public Action GetAction(string key) => _actions[key];

        public bool ContainsKey(string key) => _actions.ContainsKey(key);

    }
}