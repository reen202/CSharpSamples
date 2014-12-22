///////////////////////////////////////////////////////////////////////
// CmdLnInterpreter.cs - Handles parsing of command line parameters  //
// ver 1.0                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: CSE681 Project #2, Fall 2014                         //
// Author:      Rinita Raju, Syracuse University                     //
//              (315) 728-8952, riraju@syr.edu                       //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * CmdLnInterpreter package is the starting point of all analysis that is performed in the application.
 *
 * This package handles parsing of command line parameters to extract the correct values and populate
 * the InputParam class accordingly
 *
 * Required Files:
 *   InputParam.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 1.0 : 09 Oct 2014
 * - first release
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace CodeAnalysis
{
    public class CommandLineParser
    {
        //----< Process the command line inputs>-----------------
        public InputParam processCommandline(string[] arguments)
        {
            InputParam inputParam = new InputParam();
            if (arguments.Length > 0)
            {
                List<string> patterns = new List<string>();
                foreach (string arg in arguments)
                {
                    if (arg.Trim().Equals("/S", StringComparison.InvariantCultureIgnoreCase))
                    {
                        inputParam.recurse = true;
                    }
                    else if (arg.Trim().Equals("/X", StringComparison.InvariantCultureIgnoreCase))
                    {
                        inputParam.xml = true;
                    }
                    else if (arg.Trim().Equals("/R", StringComparison.InvariantCultureIgnoreCase))
                    {
                        inputParam.relations = true;
                    }
                    //If path encountered, populate corresponding member variable
                    else if (arg.Trim().Equals(".") || arg.Trim().Contains("..") || arg.Trim().Contains("/"))
                    {
                        if (null == inputParam.path)
                        {
                            inputParam.path = arg;
                        }
                    }
                    else
                    {
                        patterns.Add(arg);
                    }
                }
                inputParam.patterns = patterns;
            }
            return inputParam;
        }

        //----< Display the command line inputs>-----------------
        public static void showCommandLine(string[] args)
        {
            Console.Write("\n  Commandline args are:\n");
            foreach (string arg in args)
            {
                Console.Write("  {0}", arg);
            }
            //Console.Write("\n\n  current directory: {0}", System.IO.Directory.GetCurrentDirectory());
            Console.Write("\n\n");
        }

        //----< Test Stub >--------------------------------------------------
        #if(TEST_CMDLN)
                static void Main(string[] args)
                {
                    string[] commandLineArgs= {"/x", "/r", ".", "*.cs", "/s"};
                    showCommandLine(commandLineArgs);
                    CommandLineParser commandLn = new CommandLineParser();
                    InputParam inputParam = commandLn.processCommandline(commandLineArgs);
                    Console.WriteLine("Search Subdirectores? "+ inputParam.recurse);
                    Console.WriteLine("Write xml? " + inputParam.xml);
                    Console.WriteLine("Analyze Relationships? " + inputParam.relations);
                    Console.WriteLine("Path: "+ inputParam.path);
                    Console.WriteLine("Patterns: ");
                    foreach (string pattern in inputParam.patterns)
                    {
                        Console.WriteLine("{0}", pattern);
                    }
                    Console.ReadLine();
                }
            #endif
    }

}
