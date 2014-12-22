using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeanProperties;
using FileManager;
using CLIInterpreter;
using System.Collections;
using Parser;
using Storage;
using Display;
using TypeAnalyzer;
using RelationshipAnalyzer;

namespace CodeAnalyzerFacade
{


    public struct Book
    {
        public decimal price;
        public string title;
        public string author;
    }
    class CodeAnalyzer
    {

          Book a;

        private CommandLineInputs _cli;

        public CommandLineInputs cli
        {
            get { return _cli; }
            set { _cli = value; }
        }

        public void work(String[] args)
        {
            
            CLIInterpreter.CommandLineArgs cla = new CLIInterpreter.CommandLineArgs();
            
            cli = new CommandLineInputs();

            cla.processCommandLineArgs(args,cli);

            FileMgr fm = new FileMgr(cli.FilesAndPatterns);
            fm.findFiles(cli.Path, cli.IsSubDirectoryOption);
            doCodeAnalysis(fm.getFiles());

            Console.WriteLine(cli);
            Console.WriteLine(fm);
        }

        public void doCodeAnalysis(ArrayList files)
        {
            Console.Write("\n  Demonstrating Code Analyzer");
            Console.Write("\n  ======================\n");
            Repository globalRepo = new Repository();
            readAndParseFilesForTypes(files);
            if (cli.IsRelationshipOption)
            {
                readAndParseFilesForRelationship(files);
            }
            if (cli.IsXMLOption)
            {
                XMLPrinter xp = new XMLPrinter(cli);
                xp.createXMLOutput();
            }
            
            Console.ReadLine();
        }

        public void readAndParseFilesForTypes(ArrayList files)
        {
            foreach (string file in files)
            {

                CSsemi.CSemiExp semi = openFileWithSemi(file);

                RulesAndActionParser parser = registerParserForTypeAnalysis(semi);

                iterateAllSemiForParsing(semi, parser);               

                Repository rep = Repository.getInstance();

                SharedDataStorage.data.Add(file, rep);

                ConsolePrinter cp = new ConsolePrinter(cli);

                cp.printTypeAnalysisPass(rep);              

                printAndRelease(semi);
            }
        }

        public void readAndParseFilesForRelationship(ArrayList files)
        {
            foreach (string file in files)
            {

                CSsemi.CSemiExp semi = openFileWithSemi(file);

                Repository repo = (Repository)SharedDataStorage.data[file];

                RulesAndActionParser parser = registerParserForRelationAnalysis(semi,repo);

                iterateAllSemiForParsing(semi, parser);

                SharedDataStorage.data.Remove(file);
                SharedDataStorage.data.Add(file, repo);

                ConsolePrinter cp = new ConsolePrinter(cli);

                cp.displayRelationShip(repo);

                printAndRelease(semi);
            }
        }


        public CSsemi.CSemiExp openFileWithSemi(string file)
        {
            Console.Write("\n  Processing file {0}\n", file as string);

            CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
            semi.displayNewLines = false;
            if (!semi.open(file as string))
            {
                Console.Write("\n  Can't open {0}\n\n", file);
                return null;
            }
            return semi;
        }


        public void iterateAllSemiForParsing(CSsemi.CSemiExp semi, RulesAndActionParser parser)
        {
            try
            {
                while (semi.getSemi())
                    parser.parse(semi);

            }
            catch (Exception ex)
            {
                Console.Write("\n\n  {0}\n", ex.Message);
            }
        }


        public RulesAndActionParser registerParserForTypeAnalysis(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  Type and Function Analysis");
            Console.Write("\n ----------------------------\n");

            BuildTypeAnalyzer builder = new BuildTypeAnalyzer(semi);
            RulesAndActionParser parser = builder.build();
            return parser;
        }

        public RulesAndActionParser registerParserForRelationAnalysis(CSsemi.CSemiExp semi, Repository repo)
        {
            Console.Write("\n RelationShip Analysis");
            Console.Write("\n ----------------------------\n");

            BuildRelationAnalyzer builder = new BuildRelationAnalyzer(semi,repo);
            RulesAndActionParser parser = builder.build();
            return parser;
        }

        public void printAndRelease(CSsemi.CSemiExp semi)
        {
            Console.WriteLine();          
            semi.close();
        }

    }
}
