///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// ver 2.1                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: Demonstration for CSE681, Project #2, Fall 2011      //
// Author:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 2.2 : 24 Sep 2011
 * - modified Semi package to extract compile directives (statements with #)
 *   as semiExpressions
 * - strengthened and simplified DetectFunction
 * - the previous changes fixed a bug, reported by Yu-Chi Jen, resulting in
 * - failure to properly handle a couple of special cases in DetectFunction
 * - fixed bug in PopStack, reported by Weimin Huang, that resulted in
 *   overloaded functions all being reported as ending on the same line
 * - fixed bug in isSpecialToken, in the DetectFunction class, found and
 *   solved by Zuowei Yuan, by adding "using" to the special tokens list.
 * - There is a remaining bug in Toker caused by using the @ just before
 *   quotes to allow using \ as characters so they are not interpreted as
 *   escape sequences.  You will have to avoid using this construct, e.g.,
 *   use "\\xyz" instead of @"\xyz".  Too many changes and subsequent testing
 *   are required to fix this immediately.
 * ver 2.1 : 13 Sep 2011
 * - made BuildCodeAnalyzer a public class
 * ver 2.0 : 05 Sep 2011
 * - removed old stack and added scope stack
 * - added Repository class that allows actions to save and 
 *   retrieve application specific data
 * - added rules and actions specific to Project #2, Fall 2010
 * ver 1.1 : 05 Sep 11
 * - added Repository and references to ScopeStack
 * - revised actions
 * - thought about added folding rules
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 * Planned Modifications (not needed for Project #2):
 * --------------------------------------------------
 * - add folding rules:
 *   - CSemiExp returns for(int i=0; i<len; ++i) { as three semi-expressions, e.g.:
 *       for(int i=0;
 *       i<len;
 *       ++i) {
 *     The first folding rule folds these three semi-expression into one,
 *     passed to parser. 
 *   - CToker returns operator[]( as four distinct tokens, e.g.: operator, [, ], (.
 *     The second folding rule coalesces the first three into one token so we get:
 *     operator[], ( 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Storage;
using AnalyzerConstants;
using AnalyzerFactory;

namespace RulesAndActions
{

    /////////////////////////////////////////////////////////
    // pushes scope info on stack when entering new scope

    public class RelationshipStorage : AAction
    {
        Repository repo_;

        public RelationshipStorage(Repository repo)
        {
            repo_ = repo;
        }

        public override void doAction(CSsemi.CSemiExp semi)
        {
        }

        public override void doAction(CSsemi.CSemiExp semi, RelationShipElem relationshipElem, int indexedPosition)
        {
            // Calling the abstract factory
            AbstractRelationshipFactory relationshipFactory = AnalyzerFactoryProducer.getFactory(Constants.RELATIONSHIP_ANALYZER_TYPE);
            IRelationshipWorker relationshipworker = relationshipFactory.getRelationshipWorker(relationshipElem.relationship);
            relationshipworker.doRelationSpecificAction(repo_, semi, relationshipElem, indexedPosition);

        }
    }

    /////////////////////////////////////////////////////////
    // pushes scope info on stack when entering new scope

    public class PushStack : AAction
    {
        Repository repo_;

        public PushStack(Repository repo)
        {
            repo_ = repo;
        }

        public override void doAction(CSsemi.CSemiExp semi, RelationShipElem elem, int fetchedIndex)
        {
        }

        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem = new Elem();
            elem.type = semi[0];  // expects type
            elem.name = semi[1];  // expects name
            elem.begin = repo_.semi.lineCount - 1;
            elem.end = 0;
            repo_.stack.push(elem);
            if (elem.type == "control" || elem.name == "anonymous")
                return;
            if (String.Equals(elem.type, "function", StringComparison.OrdinalIgnoreCase))
            {
                isScopeCounterSet = true;
            }
            repo_.locations.Add(elem);

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }
    }
    /////////////////////////////////////////////////////////
    // pops scope info from stack when leaving scope

    public class PopStack : AAction
    {
        Repository repo_;

        public PopStack(Repository repo)
        {
            repo_ = repo;
        }

        public override void doAction(CSsemi.CSemiExp semi, RelationShipElem elem, int fetchedIndex)
        {
        }


        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem;

            try
            {
                elem = repo_.stack.pop();
                if (isScopeCounterSet)
                {
                    increaseScopeCounter();
                }
                for (int i = 0; i < repo_.locations.Count; ++i)
                {
                    Elem temp = repo_.locations[i];
                    if (elem.type == temp.type)
                    {
                        if (elem.name == temp.name)
                        {
                            if ((repo_.locations[i]).end == 0)
                            {
                                (repo_.locations[i]).end = repo_.semi.lineCount;
                                (repo_.locations[i]).size = (repo_.locations[i]).end - (repo_.locations[i]).begin;
                                if (String.Equals(elem.type, Constants.FUNCTION_NAME, StringComparison.OrdinalIgnoreCase))
                                {
                                    (repo_.locations[i]).scope = scopeCounter;
                                    resetScopeCounter();
                                    isScopeCounterSet = false;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            local.Add(elem.type).Add(elem.name);
            if (local[0] == "control")
                return;

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount);
                Console.Write("leaving  ");
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                Console.Write("{0}", indent);
                this.display(local); // defined in abstract action
            }
        }
    }
      /////////////////////////////////////////////////////////
    // pops scope info from stack when leaving scope

    public class DelegateAction : AAction
    {
        Repository repo_;

        public DelegateAction(Repository repo)
        {
            repo_ = repo;
        }

        public override void doAction(CSsemi.CSemiExp semi, RelationShipElem elem, int fetchedIndex)
        {

        }


        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem delegateElement = new Elem();
            delegateElement.type = semi[0];  // expects type
            delegateElement.name = semi[1];  // expects name
            delegateElement.begin = repo_.semi.lineCount - 1;
            delegateElement.end = repo_.semi.lineCount - 1;
            repo_.locations.Add(delegateElement);

        }
    }


    ///////////////////////////////////////////////////////////
    // action to print function signatures - not used in demo
       public class PrintFunction : AAction
    {
        Repository repo_;

        public PrintFunction(Repository repo)
        {
            repo_ = repo;
        }
        public override void display(CSsemi.CSemiExp semi)
        {
            Console.Write("\n    line# {0}", repo_.semi.lineCount - 1);
            Console.Write("\n    ");
            for (int i = 0; i < semi.count; ++i)
                if (semi[i] != "\n" && !semi.isComment(semi[i]))
                    Console.Write("{0} ", semi[i]);
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }

        public override void doAction(CSsemi.CSemiExp semi, RelationShipElem elem, int fetchedIndex)
        {
        }

    }
    /////////////////////////////////////////////////////////
    // concrete printing action, useful for debugging

    public class Print : AAction
    {
        Repository repo_;

        public Print(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }
        public override void doAction(CSsemi.CSemiExp semi, RelationShipElem elem, int fetchedIndex)
        {
        }


    }
    /////////////////////////////////////////////////////////
    // rule to detect namespace declarations

    public class DetectNamespace : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("namespace");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect class definitions

    public class DetectClass : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int indexCL = semi.Contains(Constants.CLASS_TYPE_NAME);
            int indexIF = semi.Contains(Constants.INTERFACE_TYPE_NAME);
            int indexST = semi.Contains(Constants.STRUCT_OPERATOR);
            int indexEN = semi.Contains(Constants.ENUM_OPERATOR);

            int index = Math.Max(indexCL, indexIF);
            index = Math.Max(index, indexST);
            index = Math.Max(index, indexEN);
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // rule to dectect delegate definitions

    public class DetectDelegate : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int indexDel = semi.Contains(Constants.DELEGATE_TYPE_NAME);

            if (indexDel != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(Constants.DELEGATE_TYPE_NAME).Add(semi[indexDel + 2]);
                doActions(local);
                return true;
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // rule to dectect inheritance relation

    public class DetectInheritance : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int elementIndex = -1;
            int colonIndex = semi.Contains(":");
            if (colonIndex != -1)
            {
                int indexCL = semi.Contains(Constants.CLASS_TYPE_NAME);
                int indexIF = semi.Contains(Constants.INTERFACE_TYPE_NAME);
                //check if colon is not for switch case
                int indexCase = semi.Contains(Constants.SWITCH_CASES_NAME);
                //check if colon is not ternary operator
                int indexTernary = semi.Contains(Constants.TERNARY_OPERATOR_SYMBOL);
                elementIndex = Math.Max(indexCL, indexIF);
                if (elementIndex != -1 && colonIndex != -1 && indexCase == -1 && indexTernary == -1)
                {
                    if (colonIndex - 1 >= 0 && colonIndex - 2 >= 0)
                    {
                        RelationShipElem element = new RelationShipElem();
                        element.relationship = Constants.INHERITANCE_RELATIONSHIP_NAME;
                        element.name = semi[colonIndex + 1];
                        element.lineNo = semi.lineCount - 1;
                        doActions(semi, element, colonIndex);
                        return true;
                    }

                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }

    /////////////////////////////////////////////////////////
    // rule to dectect aggregation relation 
    public class DetectAggregation : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int newIndex = semi.Contains(Constants.NEW_OPERATOR);
            int lineNo = semi.lineCount - 1;


            if (newIndex != -1)
            {
                RelationShipElem element = new RelationShipElem();
                element.relationship = Constants.AGGREGATION_RELATIONSHIP_NAME;
                element.name = semi[newIndex + 1];
                element.lineNo = semi.lineCount - 1;
                doActions(semi, element, newIndex);
                if (element.type != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    /////////////////////////////////////////////////////////
    // rule to dectect composition relation 
    public class DetectComposition : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = -1;
            Boolean isSpecialKeyword = true;
            if (semi.Contains(Constants.STRUCT_OPERATOR) != -1)
            {
                index = semi.Contains(Constants.STRUCT_OPERATOR);
            }
            else if (semi.Contains(Constants.ENUM_OPERATOR) != -1)
            {
                index = semi.Contains(Constants.ENUM_OPERATOR);
            }
            else
            {
                isSpecialKeyword = checkforReferencedComposedTypesFromSemis(semi);
            }

            int lineNo = semi.lineCount - 1;

            if (index != -1 || (!isSpecialKeyword))
            {
                RelationShipElem element = new RelationShipElem();
                element.relationship = Constants.COMPOSITION_RELATIONSHIP_NAME;
                if (index != -1)
                {
                    element.name = semi[index + 1];
                }
                else if (!isSpecialKeyword && semi.count <= 4 && semi.count > 2)
                {
                    element.name = semi[0];
                }
                element.lineNo = semi.lineCount - 1;
                doActions(semi, element, index);
                if (element.type != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



        public bool checkforReferencedComposedTypesFromSemis(CSsemi.CSemiExp semi)
        {
            String dataType = null;
            Boolean isSpecialKeyword = false;
          
            for (int loopIndex = 0; loopIndex < semi.count; loopIndex++)
            {
                dataType = semi[loopIndex];
                if (checkNonComposedConstantsPresence(dataType) != null)
                {
                    isSpecialKeyword = true;
                    semi.remove(loopIndex);
                    loopIndex--;
                }
                else if (checkModifierPresence(dataType) != null)
                {
                    semi.remove(loopIndex);
                    loopIndex--;
                }
                else if (checkOtherKeywordsPresence(dataType) != null)
                {
                    semi.remove(loopIndex);
                    loopIndex--;
                }
            }
            return isSpecialKeyword;
        }
    }


    /////////////////////////////////////////////////////////
    // rule to dectect delegate using relation 
    public class DetectDelegateUsingRelationShip : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            if (semi.count > 3)
            {
                int equalToIndex = semi.Contains(Constants.EQUAL_TO_OPERATOR);
                int lineNo = semi.lineCount - 1;


                if (equalToIndex != -1 && equalToIndex - 2 >= 0)
                {
                    RelationShipElem element = new RelationShipElem();
                    element.relationship = Constants.DELEGATE_USING_RELATIONSHIP_NAME;
                    element.name = semi[equalToIndex - 2];
                    element.lineNo = semi.lineCount - 1;
                    doActions(semi, element, equalToIndex);
                    if (String.Equals(element.type , Constants.DELEGATE_USING_RELATIONSHIP_NAME,StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                   
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }


    /////////////////////////////////////////////////////////
    // rule to dectect function definitions

    public class DetectUsing : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using", "switch" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }

        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.FindFirst("(");
            if (semi[semi.count - 1] != "{")
            {
                return false;
            }

            if (index > 0 && !isSpecialToken(semi[index - 1]))
            {
                int startBracketIndex = semi.Contains("(");
                int endBracketIndex = semi.Contains(")");
                List<String> functionParameterTypeList = new List<String>();
                if (startBracketIndex != -1 && endBracketIndex != -1)
                {
                    for (int loopIndex = startBracketIndex; loopIndex < endBracketIndex; loopIndex++)
                    {
                        if (loopIndex - 1 > 0 && String.Equals(semi[loopIndex - 1], ","))
                        {
                            functionParameterTypeList.Add(semi[loopIndex]);
                        }
                    }
                    Boolean typeIdentified = false;
                    foreach (String usingElem in functionParameterTypeList)
                    {
                        RelationShipElem element = new RelationShipElem();
                        element.relationship = Constants.USING_RELATIONSHIP_NAME;
                        element.name = usingElem;
                        element.lineNo = semi.lineCount - 1;
                        doActions(semi, element, startBracketIndex);
                        if (element.type != null)
                        {
                            typeIdentified = true;
                        }
                    }
                    if (typeIdentified)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // rule to dectect function definitions

    public class DetectFunction : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using", "switch" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }

        public bool testBracelessScope(string token)
        {
            string[] bracelessScope = { "if", "for", "foreach", "while" };
            foreach (string stoken in bracelessScope)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.FindFirst("(");
            if (semi[semi.count - 1] != "{")
            {
                if (index - 1 >= 0 && AAction.isScopeCounterSet && (testBracelessScope(semi[index - 1])))
                {
                    AAction.increaseScopeCounter();
                }
                return false;
            }

            if (index > 0 && !isSpecialToken(semi[index - 1]))
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                local.Add("function").Add(semi[index - 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }


    /////////////////////////////////////////////////////////
    // detect entering anonymous scope
    // - expects namespace, class, and function scopes
    //   already handled, so put this rule after those
    public class DetectAnonymousScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("{");
            if (index != -1)
            {


                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("control").Add("anonymous");
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // detect leaving scope

    public class DetectLeavingScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("}");
            if (index != -1)
            {
                doActions(semi);
                return true;
            }
            return false;
        }
    }

}

