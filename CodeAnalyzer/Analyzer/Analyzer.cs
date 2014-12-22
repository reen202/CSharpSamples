///////////////////////////////////////////////////////////////////////
// Analyzer.cs - Manages code analysis                               //
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
 * Analyzer package is the starting point of all analysis that is performed in the application.
 *
 * The package has doAnalysis() method which is the starting point which calls the parser
 * and depending on whether it is relationship analysis or complexity analysis invokes
 * the corresponding method, either convertFileInfoComplexity() or convertFileInfoRelationship()
 * to map the repository info to the correct layout.
 *
 * Required Files:
 *   AnalyzerOutput.cs, InputParam.cs, Parser.cs
 *   
 * Note:
 * Since only the analysis can be performed using Analyzer, the reference to Display 
 * module has been added to display the analysis results. Else, there is no dependence
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
    public class Analyzer
    {
        //----< Begin code analysis >-----------------
        public static AnalyzerOutput doAnalysis(InputParam inputParam)
        {
            AnalyzerOutput analyzerOutput = new AnalyzerOutput();            
            string[] files = inputParam.files.ToArray();
            Console.WriteLine("The list of files being processed are: ");
            foreach (string filename in files) { 
                if(filename != null)
                Console.WriteLine(filename);
            }
            Console.WriteLine("\n\n");
            //Condition to check the type of analysis- type and function complexity or relationships
            if(!inputParam.relations)
            {
                ComplexityAnalysis complexityAnalysis = analyzeComplexity(files, inputParam);
                analyzerOutput.complexityAnalysis = complexityAnalysis;                
            }
            else
            {
                RelationshipAnalysis relationshipAnalysis = analyzeRelations(files, inputParam);
                analyzerOutput.relationshipAnalysis = relationshipAnalysis;
            }
            return analyzerOutput;
        }

        //----< Convert repository to obtain relationship information>-----------------
        public static RelationshipDetails convertToFileInfoRelations()
        {
            Repository rep = Repository.getInstance();
            List<Elem> table = rep.locations;

            RelationshipDetails fileInfo = new RelationshipDetails();
            List<RelationshipInfo> relationshipInfoList = new List<RelationshipInfo>();
            //outermost loop to process namespaces 
            string className=null;
            int classEndLn = -1;
            int classStLn = -1;
            int fnStLn = -1;
            int fnEndLn = -1;
            for (int i = 0; i < table.Count; ++i)
            {
                //If class name is encountered save the name and line numbers for future use
                RelationshipInfo relationshipInfo = new RelationshipInfo();
                if (table[i].type.Equals("className"))
                {
                    className = table[i].name;
                    classEndLn = table[i].end;
                    classStLn = table[i].begin;                    
                }
                else if (table[i].type.Equals("inheritance"))
                {                    
                    relationshipInfo.className1 = className;
                    relationshipInfo.className2 = table[i].relClass;
                    relationshipInfo.relationshipType = table[i].type;
                    relationshipInfo.relationshipDetails = className + " extends " + table[i].relClass;
                    relationshipInfoList.Add(relationshipInfo);
                }
                //if the struct or enum is encountered inside class scope, it is composition
                else if (table[i].type.Equals("composition"))
                {
                    if (table[i].begin > classStLn && table[i].end < classEndLn) { 
                        relationshipInfo.className1 = className;
                        relationshipInfo.className2 = table[i].relClass;
                        relationshipInfo.relationshipType = table[i].type;
                        relationshipInfo.relationshipDetails = className + " composes " + table[i].relClass;
                        relationshipInfoList.Add(relationshipInfo);
                    }
                }
                //An entry with the name using is encountered either for function or
                // for the actual using relationship
                else if (table[i].type.Equals("using"))
                {
                    if (table[i].name.Equals("function"))
                    {
                        fnStLn = table[i].begin;
                        fnEndLn = table[i].end;
                    }
                    else { 
                        relationshipInfo.className1 = className;
                        relationshipInfo.className2 = table[i].relClass;
                        relationshipInfo.relationshipType = table[i].type;
                        relationshipInfo.relationshipDetails = className + " uses " + table[i].relClass;
                        relationshipInfoList.Add(relationshipInfo);
                    }
                }
                //An entry with the name aggregation is encountered whenever new keyword 
                //has been used in the code. Filter out the instances where it is within function
                else if(table[i].type.Equals("aggregation")){
                    if (!(table[i].begin > fnStLn && table[i].end < fnEndLn) || fnStLn == -1)
                    {
                        relationshipInfo.className1 = className;
                        relationshipInfo.className2 = table[i].relClass;
                        relationshipInfo.relationshipType = table[i].type;
                        relationshipInfo.relationshipDetails = className + " aggregates " + table[i].relClass;
                        relationshipInfoList.Add(relationshipInfo);
                    }
                }
            }
            fileInfo.relationshipList = relationshipInfoList;
            return fileInfo;
        }
        //----< Convert repository to obtain type and function complexity information>-----------------
        public static ComplexityDetails convertToFileInfoComplexity()
        {
            Repository rep = Repository.getInstance();
            List<Elem> table = rep.locations;

            ComplexityDetails fileInfo = new ComplexityDetails();
            List<NameSpaceInfo> namespaceList = new List<NameSpaceInfo>();
            
            //outermost loop to process namespaces 
            for (int i = 0; i < table.Count; ++i)
            {
                int clIndex = -1;
                int clLocationsIndex = -1;
                Elem temp = table[i];
                //If namespace obtained, set the name to NameSpaceInfo class
                if (temp.type == "namespace")
                {
                    NameSpaceInfo nsInfo = new NameSpaceInfo();
                    nsInfo.nameSpaceName = temp.name;                    
                    
                    List<ElementInfo> elemTypeList = new List<ElementInfo>();
                    List<FunctionInfo> functionList = null;
                    List<TypeInfo> typeList = null;
                    //start processing element types from next index
                    for (int j = i+1; j < table.Count; ++j)
                    {
                        Elem tempElemType = table[j];                        
                        ElementInfo elemTypeInfo = new ElementInfo();
                        //Break when the next namespace is obtained. 
                        //Restart the processing.
                        if (tempElemType.type == "namespace")
                        {
                            i = j-1;                            
                            break;
                        }
                        //When class keyword is encountered, save the index for future use
                        if (tempElemType.type == "class")
                        {
                            clLocationsIndex = j;
                            elemTypeInfo.elementName = tempElemType.name;
                            elemTypeInfo.elementType = tempElemType.type;
                            functionList = new List<FunctionInfo>();
                            typeList = new List<TypeInfo>();
                            elemTypeList.Add(elemTypeInfo);
                            clIndex = elemTypeList.Count - 1;
                        }
                        else if (tempElemType.type == "interface")
                        {
                            elemTypeInfo.elementName = tempElemType.name;
                            elemTypeInfo.elementType = tempElemType.type;
                            elemTypeList.Add(elemTypeInfo);
                        }
                        //if struct and enum keyword present inside class scope, 
                        //populate them under type info else populate at the same 
                        //level as class
                        else if (tempElemType.type == "struct" || tempElemType.type == "enum")
                        {
                            if (clIndex != -1 && tempElemType.begin < table[clLocationsIndex].end)
                            {
                                TypeInfo typeInfo = new TypeInfo();
                                typeInfo.name = tempElemType.name;
                                typeInfo.type = tempElemType.type;
                                if(null != typeList)
                                    typeList.Add(typeInfo);
                            }
                            else { 
                                elemTypeInfo.elementName = tempElemType.name;
                                elemTypeInfo.elementType = tempElemType.type;
                                elemTypeList.Add(elemTypeInfo);
                            }
                        }
                        else if (tempElemType.type == "delegate")
                        {
                            TypeInfo typeInfo = new TypeInfo();
                            typeInfo.name = tempElemType.name;
                            typeInfo.type = tempElemType.type;
                            if (null != typeList) { 
                                typeList.Add(typeInfo);
                                elemTypeList[clIndex].typesList = typeList;
                            }
                        }
                        else if (tempElemType.type == "function")
                        {
                            FunctionInfo funcInfo = new FunctionInfo();
                            funcInfo.functionName = tempElemType.name;
                            funcInfo.functionSize = tempElemType.lineCount;
                            funcInfo.functionComplexity = tempElemType.scope;
                            if (null != functionList) { 
                                functionList.Add(funcInfo);
                                elemTypeList[clIndex].functionsList = functionList;
                            }
                        }
                    }
                    nsInfo.elementList = elemTypeList;
                    namespaceList.Add(nsInfo);
                }                
            }
            fileInfo.namespaceList = namespaceList;
            return fileInfo;            
        }

        //----< Analyze type and complexity info>--------------------------------------------------
        public static ComplexityAnalysis analyzeComplexity(string[] files, InputParam inputParam)
        {
            ComplexityAnalysis complexityAnalysis = new ComplexityAnalysis();
            List<ComplexityDetails> fileInfoList = new List<ComplexityDetails>();
            foreach (object file in files)
            {
                //Console.Write("\n  Processing file {0}\n", file as string);
                CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
                semi.displayNewLines = false;
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", files[0]);
                    return null;
                }
                BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
                Parser parser = builder.build();
                try
                {
                    while (semi.getSemi())
                        parser.parse(semi);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                    //continue;
                }
                //Extract complexity info from repository
                ComplexityDetails fileInfo = convertToFileInfoComplexity();
                fileInfo.fileName = file as string;
                fileInfoList.Add(fileInfo);
                semi.close();
            }
            complexityAnalysis.fileComplexityList = fileInfoList;
            return complexityAnalysis;
        }

        //----< Analyze relationship details>--------------------------------------------------
        public static RelationshipAnalysis analyzeRelations(string[] files, InputParam inputParam)
        {
            RelationshipAnalysis relationshipAnalysis = new RelationshipAnalysis();
                List<RelationshipDetails> fileInfoList = new List<RelationshipDetails>();
                foreach (object file in files)
                {
                    CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
                    semi.displayNewLines = false;
                    if (!semi.open(file as string))
                    {
                        Console.Write("\n  Can't open {0}\n\n", files[0]);
                        return null;
                    }
                    BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi, inputParam.relations);
                    Parser parser = builder.build();
                    try
                    {
                        while (semi.getSemi())
                            parser.parse(semi);
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n\n  {0}\n", ex.Message);                     
                    }
                    //Extract relationship info from repository
                    RelationshipDetails fileInfo = convertToFileInfoRelations();
                    if (file != null) { 
                        fileInfo.fileName = file as string;
                        fileInfoList.Add(fileInfo);
                    }
                    semi.close();
                }
                relationshipAnalysis.fileRelationshipList = fileInfoList;
                return relationshipAnalysis;
        }

        //----< Test Stub >--------------------------------------------------
        #if(TEST_ANALYSIS)
            static void Main(string[] args)
            {                
                //string path = "../../";
                Console.Write("\n Testing Analyzer Class");
                Console.Write("\n =====================\n");
                InputParam inputParam = new InputParam();
                inputParam.relations = true;
                inputParam.path = "../..";
                inputParam.recurse = true;
                List<string> files = new List<string>();
                files.Add("C:\\Zzz\\SMAWorkspace\\0912_HelpSession\\Analyzer\\Analyzer.cs");
                inputParam.files = files;
                AnalyzerOutput analyzerOutput= Analyzer.doAnalysis(inputParam);
                Console.WriteLine("Analysis complete");
                Display display = new Display();
                if (!inputParam.relations)
                {
                    display.displayFileInfoComplexity(analyzerOutput);
                }
                else
                {
                    display.displayFileInfoRelations(analyzerOutput);
                }            

                Console.ReadLine();
            }
        #endif
    }    
}
