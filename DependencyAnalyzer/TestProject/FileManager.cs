using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using AnalyzerConstants;

namespace FileManager
{
    class FileMgr
    {
        private ArrayList files = new ArrayList();
        private ArrayList patterns = new ArrayList();

        public FileMgr(ArrayList files, ArrayList patterns)
        {
            this.files = files;
            this.patterns = patterns;
        }

        public FileMgr( ArrayList patterns)
        {            
            this.patterns = patterns;
        }

        public void addPattern(string pattern)
        {
            patterns.Add(pattern);

        }

        public void findFiles(String path, bool isSubDir)
        {
            //if path is not provided then default it to code directory which is ../../
            if (path == null)
            {
                path = Constants.DEFAULT_PATH;
            }
            //if pattern is not provided then default it to the pattern of choosing .cs files
            if (patterns.Count == 0)
            {
                addPattern(Constants.DEFAULT_PATTERN);
            }

            foreach (string pattern in patterns)
            {
                try
                {                
                    string[] newFiles = Directory.GetFiles(path, pattern);
                    for (int i = 0; i < newFiles.Length; i++)
                    {
                        newFiles[i] = Path.GetFullPath(newFiles[i]);                   
                    }
                    files.AddRange(newFiles);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Not able to retreive file information for path = " + path +
                        " and pattern " + pattern);
                    Console.WriteLine(" Exception occurred " + e);
                }
            }

            if (isSubDir)
            {
                String[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    findFiles(dir, isSubDir);
                }
            }
            

        }

        public ArrayList getFiles()
        {
            return files;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("Input Files are ");
            foreach (String file in this.files)
            {
                sb.AppendLine(file);
            }

            return sb.ToString();
        }       

    }
}
