using AppsCLI.Models;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppsCLI
{
    class Program
    {
        Timer _tm = null;

        AutoResetEvent _autoEvent = null;

        private int _counter = 0;

        DirectoryInfo BaseComponentFolder;

        public static void Main(string[] args)
        {
            Program p = new Program();
            //p.StartTimer();

            p.Execute(null);
        }

        public void StartTimer()
        {
            _autoEvent = new AutoResetEvent(false);
            _tm = new Timer(Execute, _autoEvent, 10000, 10000);
            Console.Read();
        }

        public void Execute(Object stateInfo)
        {
            try
            {

                //if (_counter < 10)
                //{
                Console.WriteLine("Call #" + _counter);
                _counter++;
                //    return;
                //}

                //Console.WriteLine("Final call");
                //_tm.Dispose();

                //Get config (in bin)
                var binFolder = new DirectoryInfo(Environment.CurrentDirectory);
                string configPath = binFolder.FullName + "\\config.json";
                string configString = File.ReadAllText(configPath);

                if (File.Exists(configPath))
                {
                    //Parse config 
                    Config config = JsonConvert.DeserializeObject<Config>(configString);
                    if (Directory.Exists(config.BaseComponentsFolder.Path) 
                        && Directory.Exists(config.BaseTemplatesFolder.Path))
                    {
                        //Get components config
                        this.BaseComponentFolder = new DirectoryInfo(config.BaseComponentsFolder.Path);
                        var diTemplateFolder = new DirectoryInfo(config.BaseTemplatesFolder.Path);

                        string componentsConfigPath = this.BaseComponentFolder.FullName + "\\components.json";
                        if(File.Exists(componentsConfigPath))
                        {
                            //Parse components config
                            string componentsConfigString = File.ReadAllText(componentsConfigPath);
                            var componentList = JsonConvert.DeserializeObject<ComponentList>(componentsConfigString);

                            CreateComponents(componentList.Components, this.BaseComponentFolder, diTemplateFolder);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Either base component or template folder not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem: " + ex.Message);
            }
        }
        private void CreateComponents(List<Component> componentList, DirectoryInfo baseComponentFolder, DirectoryInfo baseTemplateFolder)
        {
            foreach (Component c in componentList)
            {
                var componentFolder = baseComponentFolder;
                var templateFolder = baseTemplateFolder;

                if (Directory.Exists(c.ComponentFolder))
                    componentFolder = new DirectoryInfo(c.ComponentFolder);

                if (Directory.Exists(c.TemplateFolder))
                    templateFolder = new DirectoryInfo(c.TemplateFolder);

                CreateComponent(componentFolder.FullName + "\\" + c.Name, templateFolder.FullName, c.Name);

                if (c.Components.Count > 0)
                {
                    string subComponentFolderPath = baseComponentFolder.FullName + "\\" + c.Name + "\\Components";

                    if (!Directory.Exists(subComponentFolderPath))
                        Directory.CreateDirectory(subComponentFolderPath);

                    var subComponentFolder = new DirectoryInfo(subComponentFolderPath);

                    CreateComponents(c.Components, subComponentFolder, templateFolder);
                }
            }

        }
        private void CreateComponent(string componentPath, string templatesPath, string componentName)
        {
            try
            {
                if (!Directory.Exists(componentPath))
                {
                    Directory.CreateDirectory(componentPath);

                    CreateComponentPage(templatesPath + "\\empty.js", componentPath + "\\" + componentName + ".js", componentName);
                    CreateComponentPage(templatesPath + "\\empty.html", componentPath + "\\" + componentName + ".html", componentName);
                    CreateComponentPage(templatesPath + "\\empty.css", componentPath + "\\" + componentName + ".css", componentName);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem in create component: " + ex.Message);
            }

        }
        private void CreateComponentPage(string templatePath, string componentPagePath, string componentName)
        {
            try
            {
                if (!File.Exists(componentPagePath))
                {
                    string relativePath = componentPagePath.Replace(this.BaseComponentFolder.FullName, "");
                    relativePath = relativePath.Replace("\\", "/"); //switch to html delimiters
                    relativePath = relativePath.Replace(componentName + ".js", ""); //remove trailing file
                    relativePath = relativePath.Substring(1, relativePath.Length - 2); //remove before and after slashes

                    string htmlText = File.ReadAllText(templatePath);
                    htmlText = htmlText.Replace("MyTemplate", componentName);
                    htmlText = htmlText.Replace("MyRelativePath", relativePath);

                    File.WriteAllText(componentPagePath, htmlText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem in create component page: " + ex.Message);
            }
        }

    }
}
