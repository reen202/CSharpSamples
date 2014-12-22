using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Storage;
using Parser;
using RulesAndActions;

namespace TypeAnalyzer
{
    public class BuildTypeAnalyzer
    {
        Repository repo = new Repository();

        public BuildTypeAnalyzer(CSsemi.CSemiExp semi)
        {
            repo.semi = semi;
        }
        public virtual RulesAndActionParser build()
        {
            RulesAndActionParser parser = new RulesAndActionParser();

            // decide what to show
            AAction.displaySemi = true;
            AAction.displayStack = false;  // this is default so redundant

            // action used for namespaces, classes, and functions
            PushStack push = new PushStack(repo);

            // capture namespace info
            DetectNamespace detectNS = new DetectNamespace();
            detectNS.add(push);
            parser.add(detectNS);

            // capture class info
            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);

            // capture delegate info
            DelegateAction delegateAction = new DelegateAction(repo);
            DetectDelegate detectDelegate = new DetectDelegate();
            detectDelegate.add(delegateAction);
            parser.add(detectDelegate);

            // capture function info
            DetectFunction detectFN = new DetectFunction();
            detectFN.add(push);
            parser.add(detectFN);

            // handle entering anonymous scopes, e.g., if, while, etc.
            DetectAnonymousScope anon = new DetectAnonymousScope();
            anon.add(push);
            parser.add(anon);

            // handle leaving scopes
            DetectLeavingScope leave = new DetectLeavingScope();
            PopStack pop = new PopStack(repo);
            leave.add(pop);
            parser.add(leave);

            

            // parser configured
            return parser;
        }
    }
}
