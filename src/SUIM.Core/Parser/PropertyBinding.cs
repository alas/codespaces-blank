using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Stride.Core.Mathematics; // Assuming you use Stride math

namespace SUIM.Core.Binding
{
    /// <summary>
    /// Manages the connection between a Model property and a UI Element property.
    /// </summary>
    public class PropertyBinding
    {
        public object Model { get; }
        public PropertyInfo ModelProperty { get; }
        public object TargetElement { get; }
        public PropertyInfo TargetProperty { get; }

        public PropertyBinding(object model, string modelPropName, object target, string targetPropName)
        {
            Model = model;
            TargetElement = target;
            
            ModelProperty = model.GetType().GetProperty(modelPropName);
            TargetProperty = target.GetType().GetProperty(targetPropName, 
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (ModelProperty == null)
                throw new Exception($"SUIM Binding Error: Property '{modelPropName}' not found on Model {model.GetType().Name}");
            if (TargetProperty == null)
                throw new Exception($"SUIM Binding Error: Property '{targetPropName}' not found on Element {target.GetType().Name}");
        }

        public void Apply()
        {
            var value = ModelProperty.GetValue(Model);
            TargetProperty.SetValue(TargetElement, value);
        }
    }

    /// <summary>
    /// The Hydrator turns expanded XML into live SUIMElement objects with active bindings.
    /// </summary>
    public class SUIMHydrator
    {
        public SUIMElement Hydrate(XElement node, object model)
        {
            // 1. Create the instance based on Tag Name
            string tagName = node.Name.LocalName;
            SUIMElement element = CreateElementInstance(tagName);

            // 2. Process Attributes (Static vs @Bound)
            foreach (var attr in node.Attributes())
            {
                string name = attr.Name.LocalName;
                string val = attr.Value;

                if (val.StartsWith("@"))
                {
                    // Dynamic Binding: <grid width="@myVar" />
                    string modelPropName = val.Substring(1);
                    var binding = new PropertyBinding(model, modelPropName, element, name);
                    element.Bindings.Add(binding);
                    binding.Apply(); // Initial Sync
                }
                else
                {
                    // Static Value: <grid width="100" />
                    SetStaticProperty(element, name, val);
                }
            }

            // 3. Recurse for children
            foreach (var childNode in node.Elements())
            {
                var childElement = Hydrate(childNode, model);
                element.Children.Add(childElement);
            }

            return element;
        }

        private void SetStaticProperty(object target, string propName, string value)
        {
            var prop = target.GetType().GetProperty(propName, 
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            
            if (prop == null) return;

            // Basic Type Conversion
            object convertedValue = prop.PropertyType switch
            {
                var t when t == typeof(float) => float.Parse(value),
                var t when t == typeof(int) => int.Parse(value),
                var t when t == typeof(bool) => bool.Parse(value),
                var t when t == typeof(string) => value,
                _ => value
            };

            prop.SetValue(target, convertedValue);
        }

        private SUIMElement CreateElementInstance(string tagName)
        {
            // This is a simple factory. In a larger project, you could use a Dictionary<string, Type>
            return tagName.ToLower() switch
            {
                "vstack" => new Layout.VStack(),
                "hstack" => new Layout.HStack(), // Assuming you've created this
                "button" => new Elements.SUIMButton(),
                "label" => new Elements.SUIMLabel(),
                "grid" => new Layout.SUIMGrid(),
                _ => new Layout.SUIMBox() // Fallback generic container
            };
        }
    }
}
