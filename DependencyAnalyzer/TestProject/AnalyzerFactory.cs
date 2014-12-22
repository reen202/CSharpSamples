using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Storage;
using AnalyzerConstants;

namespace AnalyzerFactory
{
   
    public abstract class AbstractRelationshipFactory
    {
        public abstract IRelationshipWorker getRelationshipWorker(String relationshipType);        
    }

    public class RelationshipFactory : AbstractRelationshipFactory
    {        

        public override IRelationshipWorker getRelationshipWorker(String relationshipType)
        {
            
            if(String.Equals(relationshipType,Constants.INHERITANCE_RELATIONSHIP_NAME,StringComparison.OrdinalIgnoreCase))
            {
                return new InheritanceRelationshipWorker();
            }
            else if (String.Equals(relationshipType, Constants.AGGREGATION_RELATIONSHIP_NAME, StringComparison.OrdinalIgnoreCase))
            {
                return new AggregationRelationshipWorker();
            }
            else if (String.Equals(relationshipType, Constants.COMPOSITION_RELATIONSHIP_NAME, StringComparison.OrdinalIgnoreCase))
            {
                return new CompositionRelationshipWorker();
            }
            else if (String.Equals(relationshipType, Constants.USING_RELATIONSHIP_NAME, StringComparison.OrdinalIgnoreCase))
            {
                return new UsingRelationshipWorker();
            }
            else if (String.Equals(relationshipType, Constants.DELEGATE_USING_RELATIONSHIP_NAME, StringComparison.OrdinalIgnoreCase))
            {
                return new DelegateUsingRelationshipWorker();
            }
            else
            {
                return null;
            }
        }
    }

    public class AnalyzerFactoryProducer
    {
        public static AbstractRelationshipFactory getFactory(String type)
        {
            if (String.Equals(type, Constants.RELATIONSHIP_ANALYZER_TYPE, StringComparison.OrdinalIgnoreCase))
            {
                return new RelationshipFactory();
            }
            else
            {
                return null;
            }
        }

    }

    public interface IRelationshipWorker
    {
         void doRelationSpecificAction(Repository repo, CSsemi.CSemiExp semi, RelationShipElem relationshipElem, 
           int indexedPosition);
    }

    public abstract class AbstractRelationshipWorker : IRelationshipWorker
    {
        public abstract void doRelationSpecificAction(Repository repo, CSsemi.CSemiExp semi, RelationShipElem relationshipElem,
           int indexedPosition);

        public Elem searchForEnclosedElements(int lineNo, Repository repo,String elementType)
        {
            Elem enclosedElement = null;
            List<Elem> locations = repo.locations;
            if (locations != null && locations.Count > 0)
            {
                int minimumDifference = -1;
                foreach (Elem element in locations)
                {
                    if (String.Equals(element.type, elementType, StringComparison.OrdinalIgnoreCase))
                    {
                        int startLineNo = element.begin;
                        int endLineNo = element.end;
                        if (lineNo >= startLineNo && endLineNo >= lineNo)
                        {
                            int difference = lineNo - startLineNo;
                            if (minimumDifference == -1)
                            {
                                minimumDifference = difference;
                                enclosedElement = element;
                            }
                            else if (difference < minimumDifference)
                            {
                                minimumDifference = difference;
                                enclosedElement = element;
                            }
                        }
                        
                    }
                }
            }
            return enclosedElement;
        }
    }

    public class InheritanceRelationshipWorker : AbstractRelationshipWorker
    {
        public override void doRelationSpecificAction(Repository repo, CSsemi.CSemiExp semi,
            RelationShipElem relationshipElem,  int indexedPosition)
        {
            String className = semi[indexedPosition - 1];
            List<Elem> locations = repo.locations;
            foreach (Elem element in locations)
            {
                if ((String.Equals(element.name, className, StringComparison.OrdinalIgnoreCase)) &&
                    ((String.Equals(element.type, Constants.CLASS_TYPE_NAME, StringComparison.OrdinalIgnoreCase)) ||
                    (String.Equals(element.type, Constants.INTERFACE_TYPE_NAME, StringComparison.OrdinalIgnoreCase))))
                {
                    String relationElementType = "";
                    if (SharedDataStorage.lookUp(Constants.CLASS_TYPE_NAME, relationshipElem.name))
                    {
                        relationElementType = Constants.CLASS_TYPE_NAME;
                    }
                    if (SharedDataStorage.lookUp(Constants.INTERFACE_TYPE_NAME, relationshipElem.name))
                    {
                        relationElementType = Constants.INTERFACE_TYPE_NAME;
                    }

                    if (!String.Equals(relationElementType, ""))
                    {
                        List<RelationShipElem> relationShipList = element.relationshipList;
                        relationshipElem.type = relationElementType;
                        relationShipList.Add(relationshipElem);
                        element.relationshipList = relationShipList;
                    }
                }
            }
        }
    }

    public class AggregationRelationshipWorker : AbstractRelationshipWorker
    {
        public override void doRelationSpecificAction(Repository repo, CSsemi.CSemiExp semi, RelationShipElem relationshipElem, int indexedPosition)
        {
            int lineNo = semi.lineCount;
            //search for the Class containing new keyword
            Elem enclosedClass = searchForEnclosedElements(lineNo, repo,Constants.CLASS_TYPE_NAME);
            if (enclosedClass != null)
            {
                //check if the identified new keyword is not contained in functions
                /*Elem enclosedFunction = searchForEnclosedElements(lineNo, repo, Constants.FUNCTION_NAME);
                if (enclosedFunction == null)
                {
                 * */
                    String aggregatedClassName = relationshipElem.name;
                    String relationElementType = "";
                    if(SharedDataStorage.lookUp(Constants.CLASS_TYPE_NAME, aggregatedClassName))
                    {
                        relationElementType = Constants.CLASS_TYPE_NAME;
                    }
                    if (SharedDataStorage.lookUp(Constants.DELEGATE_TYPE_NAME, aggregatedClassName))
                    {
                        relationElementType = Constants.DELEGATE_TYPE_NAME;
                    }
                    if (!String.Equals(relationElementType, ""))
                    {
                        List<RelationShipElem> relationShipList = enclosedClass.relationshipList;
                        relationshipElem.type = relationElementType;
                        relationShipList.Add(relationshipElem);
                        enclosedClass.relationshipList = relationShipList;
                    }
                /*   
                }
                 * */   
            }
        }        
    }

    public class CompositionRelationshipWorker : AbstractRelationshipWorker
    {
        public override void doRelationSpecificAction(Repository repo, CSsemi.CSemiExp semi, RelationShipElem relationshipElem, int indexedPosition)
        {
            int lineNo = semi.lineCount;
            //search for the Class containing new keyword
            Elem enclosedClass = searchForEnclosedElements(lineNo, repo, Constants.CLASS_TYPE_NAME);
            if (enclosedClass != null)
            {
                
                    String composedElementName = relationshipElem.name;
                    String relationElementType = "";
                    if (SharedDataStorage.lookUp(Constants.ENUM_OPERATOR, composedElementName))
                    {
                        relationElementType = Constants.ENUM_OPERATOR;
                    }
                    else if (SharedDataStorage.lookUp(Constants.STRUCT_OPERATOR, composedElementName))
                    {
                        relationElementType = Constants.STRUCT_OPERATOR;
                    }
                    if (!String.Equals(relationElementType, ""))
                    {
                        List<RelationShipElem> relationShipList = enclosedClass.relationshipList;
                        relationshipElem.type = relationElementType;
                        relationShipList.Add(relationshipElem);
                        enclosedClass.relationshipList = relationShipList;
                    }
                
            }
        }
    }

    public class UsingRelationshipWorker : AbstractRelationshipWorker
    {
        public override void doRelationSpecificAction(Repository repo, CSsemi.CSemiExp semi, RelationShipElem relationshipElem, int indexedPosition)
        {
            int lineNo = semi.lineCount;
            //search for the Class containing new keyword
            Elem enclosedClass = searchForEnclosedElements(lineNo, repo, Constants.CLASS_TYPE_NAME);
            if (enclosedClass != null)
            {

                String composedElementName = relationshipElem.name;
                String relationElementType = "";
                if (SharedDataStorage.lookUp(Constants.CLASS_TYPE_NAME, composedElementName))
                {
                    relationElementType = Constants.CLASS_TYPE_NAME;
                }
                else if (SharedDataStorage.lookUp(Constants.INTERFACE_TYPE_NAME, composedElementName))
                {
                    relationElementType = Constants.INTERFACE_TYPE_NAME;
                }
                else if (SharedDataStorage.lookUp(Constants.ENUM_OPERATOR, composedElementName))
                {
                    relationElementType = Constants.ENUM_OPERATOR;
                }
                else if (SharedDataStorage.lookUp(Constants.STRUCT_OPERATOR, composedElementName))
                {
                    relationElementType = Constants.STRUCT_OPERATOR;
                }
                if (!String.Equals(relationElementType, ""))
                {
                    List<RelationShipElem> relationShipList = enclosedClass.relationshipList;
                    relationshipElem.type = relationElementType;
                    relationShipList.Add(relationshipElem);
                    enclosedClass.relationshipList = relationShipList;
                }

            }
        }
    }

    public class DelegateUsingRelationshipWorker : AbstractRelationshipWorker
    {
        public override void doRelationSpecificAction(Repository repo, CSsemi.CSemiExp semi, RelationShipElem relationshipElem, int indexedPosition)
        {
            int lineNo = semi.lineCount;
            //search for the Class containing new keyword
            Elem enclosedClass = searchForEnclosedElements(lineNo, repo, Constants.CLASS_TYPE_NAME);
            if (enclosedClass != null)
            {

                String composedElementName = relationshipElem.name;
                String relationElementType = "";
                if (SharedDataStorage.lookUp(Constants.DELEGATE_TYPE_NAME, composedElementName))
                {
                    relationElementType = Constants.DELEGATE_TYPE_NAME;
                }                
                if (!String.Equals(relationElementType, ""))
                {
                    List<RelationShipElem> relationShipList = enclosedClass.relationshipList;
                    relationshipElem.type = relationElementType;
                    relationshipElem.relationship = Constants.USING_RELATIONSHIP_NAME;
                    relationShipList.Add(relationshipElem);
                    enclosedClass.relationshipList = relationShipList;
                }

            }
        }
    }
    
   
}
