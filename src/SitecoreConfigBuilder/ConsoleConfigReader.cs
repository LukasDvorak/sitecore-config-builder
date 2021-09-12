using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Xml;

namespace SitecoreConfigBuilder
{
    public class ConsoleConfigReader : ConfigReader
    {
        protected readonly FileInfo WebConfigFile;

        protected static string[] _directories = new[] { "Sitecore", "Modules", "Include" };

        public ConsoleConfigReader()
        {
        }

        public ConsoleConfigReader(FileInfo webConfigFile, IEnumerable<string> includeFiles) : this(includeFiles)
        {
            this.WebConfigFile = webConfigFile;
        }

        public ConsoleConfigReader(IEnumerable<string> includeFiles) : base(includeFiles)
        {
        }

        public static IEnumerable<string> GetIncludeFiles(FileInfo webConfigFile)
        {
            var includeFiles = new List<string>();
            var appConfigFolder = webConfigFile.DirectoryName + "\\App_Config";

            for (int i = 0; i < _directories.Length; i++)
            {
                var dirPath = appConfigFolder + "/" + _directories[i];

                if (Directory.Exists(dirPath))
                {
                    var files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);
                    includeFiles.AddRange(files);
                }
            }

            return includeFiles;
        }

        public XmlDocument CreateConfiguration()
        {
            ConsoleConfigReader.ZeroConfiguration = true;
            XmlNode configNode = this.GetConfigurationNode();
            Assert.IsNotNull((object)configNode, "Could not read Sitecore configuration.");
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild((XmlNode)xmlDocument.CreateElement("sitecore"));
            this.GetConfigPatcher((XmlNode)xmlDocument.DocumentElement).ApplyPatch(configNode);
            this.ExpandIncludeFiles((XmlNode)xmlDocument.DocumentElement, new Hashtable());
            this.LoadIncludeFiles((XmlNode)xmlDocument.DocumentElement);
            this.ReplaceEnvironmentVariables((XmlNode)xmlDocument.DocumentElement);
            this.ReplaceGlobalVariables((XmlNode)xmlDocument.DocumentElement);
            return xmlDocument;
        }

        protected XmlNode GetConfigurationNode()
        {
            var xml = XmlUtil.LoadXmlFile(this.WebConfigFile.FullName).DocumentElement;
            //Assert.ArgumentNotNull((object)xpath, nameof(xpath));
            //if (ConsoleConfigReader.ZeroConfiguration)
            //    return (XmlNode)Sitecore.Xml.XmlUtil.LoadXml("<sitecore />").DocumentElement;
            //var reader = ConfigurationManager.GetSection("sitecore") as ConsoleConfigReader;
            //if (reader == null)
            //    return (XmlNode)null;
            //return xpath.Length > 0 ? reader.GetNode(xpath) : reader._section;
            foreach (XmlNode item in xml.ChildNodes)
            {
                if (item.Name == "sitecore")
                {
                    var sitecoreConfig = item.Attributes["configSource"].Value;
                    var sitecoreXml = XmlUtil.LoadXmlFile(this.WebConfigFile.DirectoryName + "\\" + sitecoreConfig).DocumentElement;
                    return sitecoreXml;
                }
            }

            return null;
        }

        protected override void LoadIncludeFiles(ConfigPatcher patcher, IEnumerable<string> files)
        {
            Assert.ArgumentNotNull((object)patcher, nameof(patcher));

            foreach (string file in files)
            {
                try
                {
                    Console.WriteLine("Loading included file: " + file);
                    patcher.ApplyPatch(file);
                }
                catch (Exception ex)
                {
                    throw new Sitecore.Exceptions.ConfigurationException("An error occurred during applying the patch file: " + file, ex);
                }

                this.ExpandIncludeFiles(patcher.Document, new Hashtable());
            }
        }

        protected override XmlDocument LoadXmlFile(string path)
        {
            var fixedPath = this.WebConfigFile.DirectoryName + path.Replace("/", "\\");
            return base.LoadXmlFile(fixedPath);
        }
    }
}
