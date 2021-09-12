﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CommandLine;
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

            Console.ReadKey();
        }

        static void ProcessPath(string path)
        {
            path = path.TrimEnd('\\');

            if (!File.Exists(path))
            {
                throw new ApplicationException("Path is not directory or zip file");
            }

            //if (!includeFiles.Any())
            //{
            //    return;
            //}

            var f = new FileInfo(path);
           
            if (!f.Name.Equals("web.config", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ApplicationException("File is not 'web.config'");
            }

            var dirName = f.DirectoryName;

            var dir = new DirectoryInfo(dirName);
            var includeFiles = ConsoleConfigReader.GetIncludeFiles(f);

            var configNode = new ConsoleConfigReader(f, includeFiles);
            var config = configNode.CreateConfiguration();
            config.Save(dirName + "\\showconfig.aspx.xml");
            //xmlDocument.DocumentElement).ApplyPatch(configNode)
        }
    }
}