///////////////////////////////////////////////////////////////////////
// Exec.cs - Starting point of execution. Executive class manages    //
// all the other classes                                             //
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
 * Application execution starts with this package. 
 * It calls the CmdLnInterpreter to parse the command line parameters, 
 * invokes, FileMgr to get the file list, invokes analyzer to analyze 
the code and calls display module to display the output.
 * 
 * Required Files:
 *   Analyzer.cs, CmdLnInterpreter.cs, AnalyzerOutput.cs, Display.cs, FileMgr.cs, InputParam.cs
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
using System.Xml;
using System.Xml.Serialization;

namespace CodeAnalysis
{
    class Exec
    {
        static void Main(string[] args)
        {
            try
            {
                CommandLineParser clInterpreter = new CommandLineParser();
                CommandLineParser.showCommandLine(args);
                //Invoke CommandLineParser to parse the command line arguments
                InputParam inputParam = clInterpreter.processCommandline(args);
                getFiles(inputParam);
                //Call Analyzer to start the analysis of files.
                AnalyzerOutput analyzerOutput = Analyzer.doAnalysis(inputParam);
                Display display = new Display();
                if (!inputParam.relations)
                {
                    display.displayFileInfoComplexity(analyzerOutput);
                }
                else
                {
                    display.displayFileInfoRelations(analyzerOutput);
                }
                //Populate the xml output
                if (inputParam.xml)
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    using (XmlWriter writer = XmlWriter.Create("CodeAnalyzer.xml", settings))
                    {
                        if (inputParam.relations)
                        {
                            XmlSerializer x = new XmlSerializer(analyzerOutput.relationshipAnalysis.GetType());
                            x.Serialize(writer, analyzerOutput.relationshipAnalysis, null);
                        }
                        else
                        {
                            XmlSerializer x = new XmlSerializer(analyzerOutput.complexityAnalysis.GetType());
                            x.Serialize(writer, analyzerOutput.complexityAnalysis, null);
                        }
                    }
                    Console.WriteLine("Analysis results saved in CodeAnalyzer.xml");
                    Console.WriteLine("File can be found at: " + System.IO.Directory.GetCurrentDirectory());
                }
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("\n {0}", e.Message);
                Console.ReadLine();
            }
        }

        //----< Get the list of files to be analyzed>-----------------
        static public void getFiles(InputParam inputParam)
        {
            FileMgr fm = new FileMgr();
            //Set the boolean for subdirectory search
            fm.setRecurse(inputParam.recurse);
            if (inputParam.patterns != null)
            {
                foreach (string pattern in inputParam.patterns)
                    fm.addPattern(pattern);
            }
            fm.findFiles(inputParam.path);
            inputParam.files = fm.getFiles();
        }
    }
}
