using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Collections;

namespace SUIM.Core.Parser
{
    public class SUIMProcessor
    {
        private readonly string _componentPath;
        private readonly string[] _primitiveTags = { "vstack", "hstack", "button", "label", "panel", "image", "grid", "div" };

        public SUIMProcessor(string componentPath = "UI/Components")
        {
            _componentPath = componentPath;
        }

        /// <summary>
        /// Entry point: Takes raw SUIM XML and a data model, returns expanded, logic-free XML.
        /// </summary>
        public string Process(string xmlInput, object globalModel = null)
        {
            XElement root = XElement.Parse(xmlInput);
            var rootModel = ObjectToDictionary(globalModel);
            
            ExpandNode(root, rootModel);
            
            return root.ToString();
        }

        private void ExpandNode(XElement parent, Dictionary<string, object> currentModel)
        {
            // We use ToList because we will be modifying the tree structure during iteration
            var nodes = parent.Elements().ToList();

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (node.Parent == null) continue; // Skip if already removed by a previous conditional logic

                string tagName = node.Name.LocalName.ToLower();

                // 1. Handle Conditional Logic Chains (@if -> @else-if -> @else)
                if (tagName == "@if")
                {
                    HandleConditionalChain(node, currentModel);
                }
                // 2. Handle Component Expansion (Custom Tags)
                else if (!IsPrimitive(tagName) && !tagName.StartsWith("@"))
                {
                    ExpandComponent(node, currentModel);
                }
                // 3. Regular Tag (Recurse to children)
                else if (IsPrimitive(tagName))
                {
                    ExpandNode(node, currentModel);
                }
            }
        }

        private void HandleConditionalChain(XElement firstNode, Dictionary<string, object> model)
        {
            // Gather the sequence of connected conditions
            var chain = new List<XElement> { firstNode };
            var current = firstNode.NextNode as XElement;

            while (current != null && (current.Name.LocalName == "@else-if" || current.Name.LocalName == "@else"))
            {
                chain.Add(current);
                current = current.NextNode as XElement;
            }

            XElement winner = null;

            // Evaluate which block wins
            foreach (var link in chain)
            {
                string tag = link.Name.LocalName;
                if (tag == "@if" || tag == "@else-if")
                {
                    string condition = link.Attribute("condition")?.Value;
                    if (Evaluate(condition, model))
                    {
                        winner = link;
                        break;
                    }
                }
                else if (tag == "@else")
                {
                    winner = link;
                    break;
                }
            }

            // If a condition was met, inject its content
            if (winner != null)
            {
                var content = winner.Nodes().ToList();
                // Important: Insert nodes before the first tag in the chain to maintain order
                firstNode.AddBeforeSelf(content);

                // Recursively process the winning content immediately
                foreach (var child in content.OfType<XElement>())
                {
                    ExpandNode(child, model);
                }
            }

            // Cleanup: Remove all tags in the chain from the final XML
            foreach (var link in chain)
            {
                link.Remove();
            }
        }

        private void ExpandComponent(XElement instanceNode, Dictionary<string, object> parentModel)
        {
            string tagName = instanceNode.Name.LocalName;
            string path = Path.Combine(_componentPath, $"{tagName}.suim");

            if (!File.Exists(path)) return;

            XElement definition = XElement.Load(path);
            var componentModel = new Dictionary<string, object>();

            // 1. Parse <model> block for local defaults
            var modelTag = definition.Element("model");
            if (modelTag != null)
            {
                ParseLocalModel(modelTag.Value, componentModel);
                modelTag.Remove();
            }

            // 2. Map instance attributes to the local model
            foreach (var attr in instanceNode.Attributes())
            {
                componentModel[attr.Name.LocalName] = attr.Value;
            }

            // 3. Process the component content with its hydrated model
            ExpandNode(definition, componentModel);

            // 4. Replace custom tag with the result
            instanceNode.ReplaceWith(definition.Nodes());
        }

        private bool Evaluate(string expression, Dictionary<string, object> model)
        {
            if (string.IsNullOrEmpty(expression)) return false;

            // Handle "=="
            if (expression.Contains("=="))
            {
                var parts = expression.Split(new[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
                var key = parts[0].Trim();
                var rawVal = parts[1].Trim().Trim('\'', '\"');

                model.TryGetValue(key, out object modelVal);

                if (rawVal.ToLower() == "null") return modelVal == null;
                return modelVal?.ToString() == rawVal;
            }

            // Handle "!="
            if (expression.Contains("!="))
            {
                var parts = expression.Split(new[] { "!=" }, StringSplitOptions.RemoveEmptyEntries);
                var key = parts[0].Trim();
                var rawVal = parts[1].Trim().Trim('\'', '\"');

                model.TryGetValue(key, out object modelVal);

                if (rawVal.ToLower() == "null") return modelVal != null;
                return modelVal?.ToString() != rawVal;
            }

            // Simple Truthy check
            model.TryGetValue(expression, out object val);
            if (val is bool b) return b;
            return val != null && val.ToString().ToLower() != "false";
        }

        private void ParseLocalModel(string text, Dictionary<string, object> dict)
        {
            var lines = text.Split(new[] { ',', '{', '}', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2)
                    dict[parts[0].Trim()] = parts[1].Trim().Trim('\'', '\"');
            }
        }

        private bool IsPrimitive(string tag) => _primitiveTags.Contains(tag.ToLower());

        private Dictionary<string, object> ObjectToDictionary(object obj)
        {
            if (obj == null) return new Dictionary<string, object>();
            if (obj is Dictionary<string, object> d) return d;
            
            return obj.GetType().GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(obj));
        }
    }
}
