///////////////////////////////////////////////////////////////////////
// Display.cs - Handles display and conversion to xml                //
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
 * This package handles display to console and population and storage of xml.
 * 
 * Required Files:
 *   AnalyzerOutput.cs
 * 
 * Note:
 * The test stub for this package is a dummy one since it depends on the entire 
 * analyzer output class to be populated.
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
    public class Display
    {
        //----< Display the type and file complexity results>-----------------
        public void displayFileInfoComplexity(AnalyzerOutput analyzerOutput)
        {
            Console.Write("\n  Type and Function Analysis" + "\n ----------------------------\n");
            //Display results of type and complexity analysis
            if (null != analyzerOutput.complexityAnalysis)
            {
                foreach (ComplexityDetails file in analyzerOutput.complexityAnalysis.fileComplexityList)
                {
                    Console.Write("Displaying Processing for File: " + file.fileName + "\n");
                    if (file.namespaceList != null && file.namespaceList.Count > 0)
                    {
                        foreach (NameSpaceInfo ns in file.namespaceList)
                        {
                            Console.WriteLine("\tNameSpace: " + ns.nameSpaceName);
                            if (null != ns.elementList && ns.elementList.Count > 0)
                            {
                                Console.WriteLine("\tNamespace Summary");
                                foreach (ElementInfo elem in ns.elementList)
                                {
                                    Console.WriteLine("\t\t{0} {1}", elem.elementType, elem.elementName);
                                    if (null != elem.typesList && elem.typesList.Count > 0)
                                    {
                                        Console.WriteLine("\t\t\tType Summary for Class");
                                        foreach (TypeInfo type in elem.typesList)
                                        {
                                            Console.WriteLine("\t\t\t\tType: {0, 10} Name: {1, 10}", type.type, type.name);
                                        }
                                    }
                                    if (null != elem.functionsList && elem.functionsList.Count > 0)
                                    {
                                        Console.WriteLine("\t\t\tFunction Summary for Class");
                                        foreach (FunctionInfo func in elem.functionsList)
                                        {
                                            Console.WriteLine("\t\t\t\tName: {0, 20} Size: {1, 3} Complexity: {2, 3}", func.functionName, func.functionSize, func.functionComplexity);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("\tNo Results to display. Please check if this is a valid C# file");
                    }
                    Console.WriteLine();
                }
            }
        }

        //----< Display the relationship results>-----------------
        public void displayFileInfoRelations(AnalyzerOutput analyzerOutput)
        {
            Console.Write("\n  Relationship Analysis" + "\n ----------------------------\n");
            //Display results of relationship analysis
            if (null != analyzerOutput.relationshipAnalysis)
            {
                foreach (RelationshipDetails file in analyzerOutput.relationshipAnalysis.fileRelationshipList)
                {
                    if (null == analyzerOutput.relationshipAnalysis.fileRelationshipList || analyzerOutput.relationshipAnalysis.fileRelationshipList.Count == 0)
                    {
                        Console.WriteLine("No files to display....");
                        return;
                    }
                    Console.Write("Displaying Processing for File: " + file.fileName + "\n");
                    if (file.relationshipList != null && file.relationshipList.Count > 0)
                    {
                        Console.WriteLine("\tClassName1\t\tClassName2\t\tRelationshipType\t\tRelationshipDetails");
                        foreach (RelationshipInfo rel in file.relationshipList)
                        {
                            Console.WriteLine("\t{0,10} {1, 20} {2, 32} {3, 34}", rel.className1, rel.className2, rel.relationshipType, rel.relationshipDetails);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\tNo Results to display. Please check if this is a valid C# file");
                    }
                    Console.WriteLine();
                }
            }
        }

        //----< Test Stub >--------------------------------------------------
#if(TEST_DISPLAY)
        static void Main(string[] args)
        {
            //Manually populating analyzerOutput object for display
            AnalyzerOutput analyzerOutput = new AnalyzerOutput();
            ComplexityAnalysis complexityAnalysis = new ComplexityAnalysis();
            List<ComplexityDetails> fileInfoList = new List<ComplexityDetails>();
            ComplexityDetails fileInfo = new ComplexityDetails();
            fileInfo.fileName = "Analyzer.Cs";
            List<NameSpaceInfo> namespaceInfoList = new List<NameSpaceInfo>();
            NameSpaceInfo namespaceInfo = new NameSpaceInfo();

            ElementInfo elem = new ElementInfo();
            elem.elementName = "Analyzer";
            elem.elementType = "class";
            FunctionInfo func = new FunctionInfo();
            func.functionName = "doAnalysis";
            func.functionSize = 50;
            func.functionComplexity = 3;
            List<FunctionInfo> funcList = new List<FunctionInfo>();
            funcList.Add(func);
            elem.functionsList = funcList;
            List<ElementInfo> elemList = new List<ElementInfo>();
            elemList.Add(elem);
            namespaceInfo.elementList = elemList;
            namespaceInfo.nameSpaceName = "CodeAnalysis";
            namespaceInfoList.Add(namespaceInfo);
            fileInfo.namespaceList = namespaceInfoList;
            fileInfoList.Add(fileInfo);
            complexityAnalysis.fileComplexityList = fileInfoList;
            Display display = new Display();
            analyzerOutput.complexityAnalysis = complexityAnalysis;
            display.displayFileInfoComplexity(analyzerOutput);
            Console.ReadLine();
        }
#endif
    }
}
