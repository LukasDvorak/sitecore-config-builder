using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SitecoreConfigBuilder
{
    public class Arguments
    {
        [Option("-path", Required = true, HelpText = "Provide path to App_Config folder of to a zip file")]
        public string Path { get; set; }
    }
}
