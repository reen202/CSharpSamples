using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeAnalysis;

namespace PackageAnalyzer
{
    class DependencyAnalysis
    {
        //Method to perform package analysis provided type table and relationship table
        public static Dictionary<string, List<string>> packageAnalysis(Dictionary<string, List<ElemRelation>> relTable, TypeTable typeTable)
        {
            var packageDep = new Dictionary<string, List<string>>();
            if (null != relTable)
            {                
                //Iterate through each file in relationship table
                foreach (var file in relTable)
                {
                    string fileName = file.Key;
                    List<string> packageList = new List<string>();
                    List<ElemRelation> locations = file.Value;
                    //For each relationship identified in a file identify the
                    //package name of the class and add to package list if not already present
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
            //Return the package dependency analysis with package name and corresponding
            //dependent packages.
            return packageDep;
        }

        public List<string> getDependencies(string packageName, Dictionary<string, List<string>> packageDependency){
            return packageDependency[packageName];

        }
    }
}
