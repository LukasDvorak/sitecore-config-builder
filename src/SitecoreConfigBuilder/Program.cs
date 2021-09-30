using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Sitecore.Configuration;

namespace SitecoreConfigBuilder
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new ApplicationException("Missing arguments");
                }

                if (args[0] != "-path")
                {
                    throw new ApplicationException("Missing argument '-path'");
                }

                if (args.Length == 1)
                {
                    throw new ApplicationException("Invalid arguments");
                }

                ProcessPath(args[1]);
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            Console.WriteLine("Done. Press key for exit.");
            Console.ReadKey();
        }

        static void ProcessPath(string path)
        {
            path = path.TrimEnd('\\');

            if (!File.Exists(path))
            {
                throw new ApplicationException("Path is not a file");
            }

            var webConfig = new FileInfo(path);
           
            if (!webConfig.Name.Equals("web.config", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ApplicationException("File is not 'web.config'");
            }

            var service = new FilesLoaderService(webConfig);
            var layerProvider = service.GetLayerProvider();
            var appSettings = service.GetAppSettings();

            Console.WriteLine("web.config[configuration][appSettings]:");

            foreach (var key in appSettings.AllKeys)
            {
                var value = appSettings[key];

                if (value.Length > 50)
                {
                    value = value.Substring(0, 50) + " ...";
                }

                Console.WriteLine($" {key} = {value}");
            }

            Thread.Sleep(200);

            var layers = layerProvider.GetLayers();

            var updatedLayers = new List<IConfigurationLayer>();

            Console.WriteLine("Configuration layers:");

            foreach (DefaultConfigurationLayer layer in layers)
            {
                var folder = webConfig.DirectoryName + layer.IncludeFolder.Replace("/", "\\");
                var fixedLayer = new DefaultConfigurationLayer(layer.Name, folder);
                fixedLayer.LoadOrder.AddRange(layer.LoadOrder);
                updatedLayers.Add(fixedLayer);
                Console.WriteLine($" {layer.Name} => {folder}");
                foreach (var item in layer.LoadOrder)
                {
                    var loadOrderPath = (layer.IncludeFolder + item.Path).Replace("/","\\");
                    Console.WriteLine($" - path: \"{loadOrderPath}\", type: {item.Type}, enabled: {item.Enabled}");
                }
            }

            Thread.Sleep(200);
            Console.WriteLine("Getting include files:");
            Thread.Sleep(200);

            var configFiles2 = updatedLayers.SelectMany(x => x.GetConfigurationFiles());

            foreach (var item in configFiles2)
            {
                var configFilePath = item.Replace(webConfig.DirectoryName, "");
                Console.WriteLine($" - {configFilePath}");
            }

            var rootDirName = webConfig.DirectoryName;

            var dir = new DirectoryInfo(rootDirName);

            //new ConfigurationRulesContext()

            var configNode = new ConsoleConfigReader(webConfig, configFiles2, appSettings);

            Thread.Sleep(200);
            Console.WriteLine("Creating confiuguration ...");
            var config = configNode.CreateConfiguration();
            var configFile = rootDirName + "\\showconfig.aspx.xml";
            config.Save(configFile);
            Console.WriteLine($"Configuration saved to '{configFile}'");
            //xmlDocument.DocumentElement).ApplyPatch(configNode)
        }
    }
}