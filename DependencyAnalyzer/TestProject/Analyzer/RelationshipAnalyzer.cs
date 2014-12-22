using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Storage;
using Parser;
using RulesAndActions;

namespace RelationshipAnalyzer
{
    
    public class BuildRelationAnalyzer
        {
        enum Days { Sun, Mon, Tue, Wed, Thu, Fri, Sat };
            Repository _repo;

            public BuildRelationAnalyzer(CSsemi.CSemiExp semi, Repository repo)
            {
                _repo = repo;
                repo.semi = semi;
            }
            public virtual RulesAndActionParser build()
            {
                RulesAndActionParser parser = new RulesAndActionParser();
                
                // decide what to show
                AAction.displaySemi = false;
                AAction.displayStack = false;  // this is default so redundant

                // action used for namespaces, classes, and functions
                RelationshipStorage relationStorage = new RelationshipStorage(_repo);              

                // capture Inheritance info
                DetectInheritance detectInheritance = new DetectInheritance();
                detectInheritance.add(relationStorage);
                parser.add(detectInheritance);


                // capture Aggregation info
                DetectAggregation detectAggregation = new DetectAggregation();
                detectAggregation.add(relationStorage);
                parser.add(detectAggregation);

                //capture Using info
                DetectUsing detectUsing = new DetectUsing();
                detectUsing.add(relationStorage);
                parser.add(detectUsing);

                //capture Using Delgate info
                DetectDelegateUsingRelationShip detectDelegateUsingRelationShip = new DetectDelegateUsingRelationShip();
                detectDelegateUsingRelationShip.add(relationStorage);
                parser.add(detectDelegateUsingRelationShip);

                //capture Composition info
                DetectComposition detectComposition = new DetectComposition();
                detectComposition.add(relationStorage);
                parser.add(detectComposition);

                

                // parser configured
                return parser;
            }
        }    
}
