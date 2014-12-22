/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *   Executable  - This is the entry point of the program which calls the 
 *   appropriate control to perform the Code Analyzer Task
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeAnalyzerFacade;

namespace Exec
{
    class Executable
    {
        public static void Main(string[] args)
        {
            CodeAnalyzerFacade.CodeAnalyzer codeAnalyzer= new 
                CodeAnalyzerFacade.CodeAnalyzer();
            codeAnalyzer.work(args);
            Console.ReadLine();
        }
       
    }
}
