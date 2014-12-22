///////////////////////////////////////////////////////////////////////
// InputParam.cs - Used to store the parsed command line parameters  //
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
 * This package is used to store data obtained from command line parameters.
 * 
 * Note:
 * The test stub present in this package is a dummy one since it is only used to store 
 * command line values and does not involve any processing.
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

namespace CodeAnalysis
{
    public class InputParam
    {
        private Boolean _recurse;

        public Boolean recurse
        {
            get { return _recurse; }
            set { _recurse = value; }
        }

        private Boolean _xml;

        public Boolean xml
        {
            get { return _xml; }
            set { _xml = value; }
        }

        private Boolean _relations;

        public Boolean relations
        {
            get { return _relations; }
            set { _relations = value; }
        }

        private List<string> _patterns;

        public List<string> patterns
        {
            get { return _patterns; }
            set { _patterns = value; }
        }

        private string _path;

        public string path
        {
            get { return _path; }
            set { _path = value; }
        }

        private List<string> _files;

        public List<string> files
        {
            get { return _files; }
            set { _files = value; }
        }
        
        //----< Test Stub >--------------------------------------------------
        #if(TEST_INPUT)
                static void Main(string[] args)
                {
                    Console.WriteLine("Dummy test stub for InputParam");
                }
        #endif
    }
}

