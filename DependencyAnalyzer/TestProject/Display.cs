using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Storage;
using System.Xml;
using BeanProperties;
using System.Collections;
using AnalyzerConstants;
using System.IO;

namespace Display
{
    class ConsolePrinter
    {
        CommandLineInputs cli;

        public ConsolePrinter(CommandLineInputs cli)
        {
            this.cli = cli;
        }
        public void printTypeAnalysisPass(Repository rep)
        {
            this.displayLocations(rep);
            this.displayComplexity(rep);            
        }

        public void displayLocations(Repository rep)
        {
            List<Elem> table = rep.locations;
            Console.WriteLine();
            Console.Write("\n   Printing the location information");
            Console.Write("\n   ======================\n");
            Console.WriteLine("\n{0,10}  {1,40}  {2,5}  {3,5}\n",
                   "Type", "Name", "Begins", "Ends");
            foreach (Elem e in table)  
            {               
                Console.WriteLine("{0,10}, {1,40}, {2,5}, {3,5}",
                    e.type, e.name, e.begin, e.end);
            }
        }

        public void displayComplexity(Repository rep)
        {
            /*if (!cli.IsRelationshipOption)*/
            {
                List<Elem> table = rep.locations;
                Console.WriteLine();
                Console.Write("   Printing the function Complexity");
                Console.Write("\n   ======================\n");
                Console.WriteLine("\n{0,40}  {1,8}  {2,8}\n",
                           "Name", "Size", "Complexity");
                foreach (Elem e in table)
                {
                    if (String.Equals(e.type, "function", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("{0,40}, {1,8}, {2,8}",
                             e.name, e.size, e.scope);
                    }

                }
            }   
        }

        public void displayRelationShip(Repository rep)
        {
            List<Elem> table = rep.locations;
            if (table != null && table.Count > 0)
            {
                Console.WriteLine();
                Console.Write("\n   Printing the RelationShip");
                Console.Write("\n   ======================\n");
                Console.WriteLine("\n{0,10}  {1,30}  {2,10}  {3,40}  {4,15}  {5,10}\n",
                           "Type", "Class1", "Type", "Class2", "Relationship", "Line Number");
                Console.WriteLine(table);
                foreach (Elem elm in table)
                {
                    if ((String.Equals(elm.type, "class", StringComparison.OrdinalIgnoreCase)) ||
                        (String.Equals(elm.type, "interface", StringComparison.OrdinalIgnoreCase)))
                    {
                        String classType = elm.type;
                        String className = elm.name;
                        List<RelationShipElem> relationshipList = elm.relationshipList;
                        foreach (RelationShipElem relationElm in relationshipList)
                        {
                            Console.WriteLine("{0,10}  {1,30}  {2,10}  {3,40}  {4,15}  {5,10}",
                           classType, className, relationElm.type, relationElm.name,
                           relationElm.relationship, relationElm.lineNo);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("   No relationsships found for this file.");
            }
        }
    }

    class XMLPrinter
    {
        CommandLineInputs cli;

        public XMLPrinter(CommandLineInputs cli)
        {
            this.cli = cli;
        }

        public void createXMLOutput()
        {
            XmlWriterSettings xmlWriterSettings = getXMLWriterSettings();
            ICollection keys = SharedDataStorage.data.Keys;
          
            using (XmlWriter writer = XmlWriter.Create("CodeAnalyzer.xml", xmlWriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("CodeAnalyzer");

                foreach (String key in keys)
                {
                    writer.WriteStartElement("File");
                    writer.WriteElementString("FilePath", key);
                    writer.WriteEndElement();

                    Repository repo = (Repository)SharedDataStorage.data[key];
                    List<Elem> locations = repo.locations;
                    writer.WriteStartElement("Elements");
                    foreach (Elem element in locations)
                    {
                        writer.WriteStartElement("Element");
                        writer.WriteElementString("Type", element.type);
                        writer.WriteElementString("Name", element.name);                        
                        writer.WriteElementString("BeginLine", element.begin.ToString());
                        writer.WriteElementString("EndLine", element.end.ToString());
                       
                        if (String.Equals(element.type, Constants.FUNCTION_NAME, StringComparison.OrdinalIgnoreCase)
                            && !cli.IsRelationshipOption)
                        {
                           writer.WriteStartElement("FunctionsSizeAndComplexity");
                           writer.WriteElementString("FunctionSize", element.size.ToString());
                           writer.WriteElementString("FunctionScope", element.scope.ToString());
                           writer.WriteEndElement();
                        }
                        
                        if (String.Equals(element.type, Constants.CLASS_TYPE_NAME, StringComparison.OrdinalIgnoreCase)
                            && cli.IsRelationshipOption)
                        {
                           writer.WriteStartElement("Relationships");
                           List<RelationShipElem> relationshipList = element.relationshipList;
                            foreach(RelationShipElem relElem in relationshipList)
                            {
                                 writer.WriteStartElement("Relationship");
                                 writer.WriteElementString("ElementType", relElem.type);
                                 writer.WriteElementString("ElementName", relElem.name);
                                 writer.WriteElementString("RelationshipType", relElem.relationship);
                                 writer.WriteElementString("LineNumber", relElem.lineNo.ToString());
                                 writer.WriteEndElement();
                            }
                          
                           writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                   
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();               
               
            }
            Console.WriteLine(" ===================================================================================");
            Console.WriteLine(" ===================================================================================");
            Console.WriteLine("   Your XML Output is stored in " + Directory.GetCurrentDirectory()+" Directory");
            Console.WriteLine(" ===================================================================================");
            Console.WriteLine(" ===================================================================================");
        }

        public XmlWriterSettings getXMLWriterSettings()
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = true
            };
            return xmlWriterSettings;
        }
    }

    class Employee
    {
        int _id;
        string _firstName;
        string _lastName;
        int _salary;

        public Employee(int id, string firstName, string lastName, int salary)
        {
            this._id = id;
            this._firstName = firstName;
            this._lastName = lastName;
            this._salary = salary;
        }

        public int Id { get { return _id; } }
        public string FirstName { get { return _firstName; } }
        public string LastName { get { return _lastName; } }
        public int Salary { get { return _salary; } }
    }
}
