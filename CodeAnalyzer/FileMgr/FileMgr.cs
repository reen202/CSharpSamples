///////////////////////////////////////////////////////////////////////
// FileMgr.cs - Starting point of execution. Executive class manages //
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
 * Obtains all the files based on particular path and patterns. 
 * 
 * Required Files:
 *   This package has no dependency on other files.
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

namespace CodeAnalysis
{
    public class FileMgr
    {
        private List<string> files = new List<string>();
        private List<string> patterns = new List<string>();
        private bool recurse = false;

        //----< Set the boolean recurse value>-----------------
        public void setRecurse(bool recurse)
        {
            this.recurse = recurse;
        }

        //----< Return the boolean recurse value>-----------------
        public bool getRecurse()
        {
            return recurse;
        }

        //----< Find the list of files to be passed based on pattern and path>-----------------
        public void findFiles(string path)
        {
            //Set default pattern to look for all files
            if (patterns.Count == 0)
            {
                addPattern("*.*");
                Console.WriteLine("Pattern has not been entered. Setting it to *.*");
            }
            //Set default path as current directory
            if (path == null)
            {
                Console.WriteLine("Path is not entered. Setting it to current directory: " + System.IO.Directory.GetCurrentDirectory());
                path = (".");
            }
            try
            {
                foreach (string pattern in patterns)
                {
                    string[] newFiles = Directory.GetFiles(path, pattern);
                    for (int i = 0; i < newFiles.Length; ++i)
                        newFiles[i] = Path.GetFullPath(newFiles[i]);
                    files.AddRange(newFiles);
                }
                //If user has opted for sub-directory search, use recursion to 
                //obtain list of files
                if (recurse)
                {
                    string[] dirs = Directory.GetDirectories(path);
                    foreach (string dir in dirs)
                        findFiles(dir);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception encountered while locating file: ", e);
                Console.WriteLine(e.StackTrace);
            }
        }

        //----< Add file name pattern to the patterns list>-----------------
        public void addPattern(string pattern)
        {
            patterns.Add(pattern);
        }

        //----< Return file list to be analyzed>-----------------
        public List<string> getFiles()
        {
            return files;
        }

        //----< Test Stub >--------------------------------------------------
        #if(FILEMGR_TEST)
        static void Main(string[] args)
        {
            Console.Write("\n Testing FileMgr Class");
            Console.Write("\n =====================\n");

            FileMgr fm = new FileMgr();
            
            //fm.addPattern("*.cs");
            //fm.findFiles("../../");

            //Find the list of file names to be analyzed
            fm.findFiles(".");
            List<string> files = fm.getFiles();
            foreach (string file in files)
            {
                Console.WriteLine("{0}",file);
            }
            Console.WriteLine("\n\n");
            Console.ReadLine();
        }
#endif
    }
}
