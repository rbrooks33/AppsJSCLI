using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppsCLI.Models
{
    public class Config
    {
        public BaseComponentsFolder BaseComponentsFolder { get; set; }
        public BaseTemplatesFolder BaseTemplatesFolder { get; set; }
    }
    public class BaseComponentsFolder
    {
        public string Path { get; set; }
        public string Description { get;set; }
    }
    public class BaseTemplatesFolder
    {
        public string Path { get; set; }
        public string Description { get; set; }
    }
}
