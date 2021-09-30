using System.Collections.Specialized;
using System.IO;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.IO;

namespace SitecoreConfigBuilder
{
    public class FilesLoaderService
    {
        protected readonly FileInfo WebConfig;

        public FilesLoaderService(FileInfo webConfigFile)
        {
            this.WebConfig = webConfigFile;
        }

        public IConfigurationLayerProvider GetLayerProvider()
        {
            var layersConfigFile = this.WebConfig.DirectoryName + LayeredConfigurationFiles.DefaultLayersConfigurationPath.Replace("/","\\");

            if (File.Exists(layersConfigFile))
            {
                string str = FileUtil.MapPath(layersConfigFile);
                
                using (FileStream fileStream = FileUtil.OpenRead(str))
                {
                    return new LayeredConfigurationProviderSerializer().Deserialize((Stream)fileStream);
                }
            }

            return new LegacyConfigurationLayerProvider();
        }

        public NameValueCollection GetAppSettings()
        {
            var xml = new XmlDocument();
            xml.Load(this.WebConfig.FullName);
            var appSettings = xml["configuration"]["appSettings"];

            var appSettingsCollection = new NameValueCollection();

            foreach (XmlNode item in appSettings.ChildNodes)
            {
                if (item.Name != "#comment")
                {
                    var key = item.Attributes["key"].InnerText;
                    var value = item.Attributes["value"].InnerText;
                    appSettingsCollection.Add(key, value);
                }
            }

            return appSettingsCollection;
        }
    }
}
