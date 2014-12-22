using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWTools;


namespace CodeAnalysis
{
    public class PackageAnalyzer
    {
        private static Dictionary<string, ServerToMaster> masterData_ = new Dictionary<string, ServerToMaster>();

        public static Dictionary<string, ServerToMaster> masterData
        {
            get { return PackageAnalyzer.masterData_; }
            set { PackageAnalyzer.masterData_ = value; }
        }
        private static BlockingQueue<ServerToMaster> masterDataQueue_ = new BlockingQueue<ServerToMaster>();

        public static BlockingQueue<ServerToMaster> masterDataQueue
        {
            get { return PackageAnalyzer.masterDataQueue_; }
            set { PackageAnalyzer.masterDataQueue_ = value; }
        }

        public static string[] processMasterData()
        {
            string[] xmlList = null;
            ServerToMaster newData = masterDataQueue.deQ();
            if (newData.requestDetails.receivedSrvList == null)
            {
                newData.requestDetails.receivedSrvList = new List<string>();
            }
            newData.requestDetails.receivedSrvList.Add(newData.requestDetails.serverName);
            string clientName = newData.requestDetails.clientName;
            if (!masterData.ContainsKey(clientName))
            {
                masterData[clientName] = newData;
            }
            else
            {
                mergeRelationTable(newData);
                mergeTypeTable(newData);
                masterData[clientName].requestDetails.receivedSrvList.Add(newData.requestDetails.serverName);
            }
            packageAnalysis(newData);
            //displaypackageAnalysis(clientName);
            //displayTypeTable(clientName);
            if (masterData[clientName].requestDetails.serverList.Count == masterData[clientName].requestDetails.receivedSrvList.Count)
            {
                ServerToMaster completeData = masterData[clientName];
                masterData.Remove(clientName);
                XMLDisplay xmlDisplay = new XMLDisplay();
                xmlList = xmlDisplay.WriteOutput(completeData);
                Console.WriteLine(xmlList[0]);
                Console.WriteLine(xmlList[1]);
                return xmlList;               
            }
            return null;
        }

        public static void mergeRelationTable(ServerToMaster newData)
        {
            string clientName = newData.requestDetails.clientName;
            ServerToMaster existingData = masterData[clientName];
            if (newData.relTable != null) { 
                foreach (var c in newData.relTable)
                {
                    if (!existingData.relTable.ContainsKey(c.Key)){
                        existingData.relTable.Add(c.Key, c.Value);
                    }
                }
            }
            //existingData.relTable.Concat(newData.relTable).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
            masterData[clientName].relTable = existingData.relTable;
        }

        public static void mergeTypeTable(ServerToMaster newData)
        {
            string clientName = newData.requestDetails.clientName;
            ServerToMaster existingData = masterData[clientName];
            if (null != newData && null != newData.typeTable && null != newData.typeTable.types)
            {
                foreach (var c in newData.typeTable.types)
                {
                    if (!existingData.typeTable.types.ContainsKey(c.Key))
                    {
                        existingData.typeTable.types.Add(c.Key, c.Value);
                    }
                }
            }
            //existingData.typeTable.types.Concat(newData.typeTable.types).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
            masterData[clientName].typeTable = existingData.typeTable;
        }

        public static void packageAnalysis(ServerToMaster newData)
        {
            string clientName = newData.requestDetails.clientName;
            Dictionary<string, List<ElemRelation>> relTable = masterData[clientName].relTable;
            TypeTable typeTable = masterData[clientName].typeTable;

            var packageDep = new Dictionary<string, List<string>>();
            if (null != relTable)
            {
                foreach (var file in relTable)
                {
                    string fileName = file.Key;
                    List<string> packageList = new List<string>();
                    List<ElemRelation> locations = file.Value;
                    foreach (var rel in locations)
                    {
                        string depClass = rel.toClass;
                        if (typeTable.types.ContainsKey(depClass))
                        {
                            String depFile = typeTable.types[depClass].First().Filename;
                            if (!packageList.Contains(depFile) && !depFile.Equals(fileName))
                            {
                                packageList.Add(depFile);
                            }
                        }
                    }
                    packageDep[fileName] = packageList;
                }
            }
            masterData[clientName].packageAnalysis = packageDep;            
        }

        public static void displaypackageAnalysis(string clientName)
        {
            Console.WriteLine("Package Dependency Analysis");
            foreach (var x in masterData[clientName].packageAnalysis)
            {
                Console.WriteLine("Package Name" + x.Key);
                foreach (string y in x.Value)
                {
                    Console.WriteLine("Dependent Package Name: " + y);
                }
            }
        }

        public static void displayTypeTable(string clientName)
        {
            Console.WriteLine("Type table Info");
            
            TypeTable typetable = masterData[clientName].typeTable;            
            foreach (var x in typetable.types)
            {
                Console.WriteLine("Class Name: " + x.Key);
                foreach (var y in x.Value)
                {
                    Console.WriteLine("FileName: " + y.Filename);
                }
            }
        }       
    }
}
