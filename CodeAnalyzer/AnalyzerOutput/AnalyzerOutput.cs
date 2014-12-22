///////////////////////////////////////////////////////////////////////
// AnalyzerOutput.cs - Contains analysis results. Specifies layout   //
//for xml                                                            //
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
 * AnalyzerOutput is the class structure that is populated with the output 
 * and the xml layout corresponds exactly to the structure of this class.
 *
 * Note:
 * The test stub present is a dummy one since the classes in this
 * package can be used only used to store output values does not involve any processing.
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
    public class FunctionInfo
    {
        public string functionName;
        public int functionSize;
        public int functionComplexity;
    }

    public class TypeInfo
    {
        public string type;
        public string name;
    }

    public class ElementInfo
    {
        public string elementType;
        public string elementName;

        public List<TypeInfo> typesList = new List<TypeInfo>();
        public List<FunctionInfo> functionsList = new List<FunctionInfo>();
    }

    public class NameSpaceInfo
    {
        public string nameSpaceName;
        public List<ElementInfo> elementList = new List<ElementInfo>();
    }

    public class ComplexityAnalysis
    {
        public List<ComplexityDetails> fileComplexityList { get; set; }
    }

    public class RelationshipAnalysis
    {
        public List<RelationshipDetails> fileRelationshipList { get; set; }
    }

    public class RelationshipInfo
    {
        public string className1;
        public string className2;
        public string relationshipType;
        public string relationshipDetails;
    }

    public class ComplexityDetails
    {
        public string fileName;
        public List<NameSpaceInfo> namespaceList = new List<NameSpaceInfo>();        
    }

    public class RelationshipDetails
    {
        public string fileName;        
        public List<RelationshipInfo> relationshipList = new List<RelationshipInfo>();
    }

    public class AnalyzerOutput
    {
        public ComplexityAnalysis complexityAnalysis { get; set; }
        public RelationshipAnalysis relationshipAnalysis { get; set; }

        //----< Test Stub >--------------------------------------------------
    #if(TEST_ANALYZEROP)
            public static void Main(string[] args)
            {
                Console.WriteLine("Dummy test stub for AnalyzerOutput");
            }
    #endif
        }
}
