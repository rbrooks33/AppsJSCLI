using System;
using System.Collections.Generic;
using System.Text;

namespace AppsCLI.Models
{
    public class Component
    {
        public Component()
        {
            Components = new List<Component>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ComponentFolder { get; set; }
        public string TemplateFolder { get; set; }
        public bool Enabled { get; set; }
        public string Color { get; set; }
        public string ModuleType { get; set; }
        public string Framework { get; set; }
        public List<Component> Components { get; set; }
    }
}
