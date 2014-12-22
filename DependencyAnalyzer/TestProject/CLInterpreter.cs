using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeanProperties;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace CLIInterpreter
{
    class CommandLineArgs
    {
        
        public void processCommandLineArgs(String[] args1, CommandLineInputs cli)
        {
            
            /*
            if (args1 != 
                null)
                args1[0] = args1[0];
            for(int i = 0 ; i < args1.Length ; i++)
                args1[0] = args1[0];
            */
            foreach(String arg in args1)
            {
                bool isOptionProcessed = false; 
                isOptionProcessed =  processCLIOptions(arg, cli);

                if (!isOptionProcessed)
                {
                    isOptionProcessed = processDirectoryOption(arg, cli);
                }
                if (!isOptionProcessed)
                {
                    isOptionProcessed = processFileOption(arg, cli);
                }                
                
            }

        }

        public bool processCLIOptions(String arg, CommandLineInputs cli)
        {
            bool isProcessed = false;
            if(arg != null)
            {
                if(String.Equals(arg,AnalyzerConstants.Constants.SUBDIRECTORY_OPTION, StringComparison.OrdinalIgnoreCase))
                {
                    cli.IsSubDirectoryOption = true;
                    isProcessed = true;
                }
                else if(String.Equals(arg,AnalyzerConstants.Constants.XML_OPTION, StringComparison.OrdinalIgnoreCase))
                {
                    cli.IsXMLOption = true;
                    isProcessed = true;

                }
                else if(String.Equals(arg,AnalyzerConstants.Constants.RELATIONSHIP_OPTION, StringComparison.OrdinalIgnoreCase))
                {
                    cli.IsRelationshipOption = true;
                    isProcessed = true;

                }                
            }
            return isProcessed;            
        }

        public bool processDirectoryOption(String arg, CommandLineInputs cli)
        {
            bool isProcessed = false;
            if (Directory.Exists(arg))
            {
                cli.Path = arg;
                isProcessed = true;
            }
            return isProcessed;
        }

        public bool processFileOption(String arg, CommandLineInputs cli)
        {
            string pat = @"(\w)+";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            Match m = r.Match(arg);

            bool isProcessed = false;
            if (m.Success)
            {
                ArrayList files = cli.FilesAndPatterns;
                files.Add(arg);
                cli.FilesAndPatterns = files;
                isProcessed = true;
            }
            return isProcessed;
        }

    }

    
}
