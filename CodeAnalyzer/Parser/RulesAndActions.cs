///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// ver 2.2                                                           //
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
 * ver 2.3 : 09 Oct 2014
 * Added the following rules, their definitions and their corresponding actions in PushStack
 * and PopStack
 * - DetectStruct
 * - DetectDelegate
 * - DetectEnum
 * - DetectBracelessScope
 * - DetectClasInherit
 * - DetectComposition
 * - DetectUsing
 * - DetectAggregation
 * Added a new boolean member variable under Repository - isOpRel to determine whether the 
 * analysis performed is relationship analysis or function complexity analysis
 * 
 * 
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

namespace CodeAnalysis
{
  public class Elem  // holds scope information
  {
    public string type { get; set; }
    public string name { get; set; }
    public int begin { get; set; }
    public int end { get; set; }
    public int lineCount { get; set; }
    public int scope { get; set; }
    public string relClass { get; set; }
    public string opType { get; set; }

    public override string ToString()
    {
      StringBuilder temp = new StringBuilder();
      temp.Append("{");
      temp.Append(String.Format("{0,-10}", type)).Append(" : ");
      temp.Append(String.Format("{0,-10}", name)).Append(" : ");
      temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
      temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
      temp.Append("}");
      return temp.ToString();
    }
  }

  public class Repository
  {
    ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
    List<Elem> locations_ = new List<Elem>();
    public Boolean isOpRel { get; set; }
    static Repository instance;
    
    
    
    public Repository()
    {
      instance = this;
    }

    public static Repository getInstance()
    {
      return instance;
    }
    // provides all actions access to current semiExp

    public CSsemi.CSemiExp semi
    {
      get;
      set;
    }

    // semi gets line count from toker who counts lines
    // while reading from its source

    public int lineCount  // saved by newline rule's action
    {
      get { return semi.lineCount; }
    }
    public int prevLineCount  // not used in this demo
    {
      get;
      set;
    }
    // enables recursively tracking entry and exit from scopes

    public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
    {
      get { return stack_; } 
    }
    // the locations table is the result returned by parser's actions
    // in this demo

    public List<Elem> locations
    {
      get { return locations_; }
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
    public override void doAction(CSsemi.CSemiExp semi)
    {
      Elem elem = new Elem();
      if (repo_.isOpRel)
      {
          ////Logic to handle relationships
          elem.begin = repo_.semi.lineCount - 1;
          elem.end = 0;      
          if (semi[0].Equals("className"))
          {
              elem.type = semi[0];
              elem.name = semi[1];              
              repo_.stack.push(elem);
          }
          else if (semi[0].Equals("inheritance"))
          {
              elem.type = semi[0];
              elem.relClass = semi[1];
          }
          else if (semi[0].Equals("composition"))
          {
              elem.type = semi[0];
              //TODO:
              elem.name = semi[1];
              elem.relClass = semi[1];              
              repo_.stack.push(elem);
              
          }
          else if (semi[0].Equals("control"))
          {
              elem.type = semi[0];
              elem.name = semi[1];
              repo_.stack.push(elem);
          }
          else if (semi[0].Equals("aggregation"))
          {
              elem.type = semi[0];
              elem.name = semi[1];
              elem.relClass = semi[1];
              elem.begin = repo_.semi.lineCount;
              elem.end = repo_.semi.lineCount;
          }
          else if (semi[0].Equals("using"))
          {
              elem.type = semi[0];
              elem.name = "function";
              for (int i = 1; i < semi.count; i++)
              {
                  Elem elemUsing = new Elem();
                  elemUsing.type = semi[0];
                  elemUsing.name = semi[1];
                  elemUsing.relClass = semi[i];
                  repo_.locations.Add(elemUsing);
              }
              elem.begin = repo_.semi.lineCount - 1;
              elem.end = 0;
              repo_.stack.push(elem);
              repo_.locations.Add(elem);

          }
          if (!semi[0].Equals("control") && !semi[0].Equals("using")) { 
            repo_.locations.Add(elem);
          }
          return;
      }
      elem.type = semi[0];  // expects type
      elem.name = semi[1];  // expects name
      elem.begin = repo_.semi.lineCount - 1;
      elem.end = 0;
      if (elem.type != "delegate" && elem.name != "braceless")
      {
          repo_.stack.push(elem);
      }
      //if (elem.type == "control" || elem.name == "anonymous")
        //return;
      repo_.locations.Add(elem);

      if (AAction.displaySemi)
      {
        Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
        Console.Write("entering ");
        string indent = new string(' ', 2 * repo_.stack.count);
        Console.Write("{0}", indent);
        this.display(semi); // defined in abstract action
      }
      if(AAction.displayStack)
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
    public override void doAction(CSsemi.CSemiExp semi)
    {
      Elem elem;
      try
      {
        elem = repo_.stack.pop();
        for (int i = 0; i < repo_.locations.Count; ++i )
        {
          Elem temp = repo_.locations[i];
          if (elem.type == temp.type)
          {
            if (elem.name == temp.name)
            {
              if ((repo_.locations[i]).end == 0)
              {
                (repo_.locations[i]).end = repo_.semi.lineCount;
                (repo_.locations[i]).lineCount = (repo_.locations[i]).end - (repo_.locations[i]).begin;
                break;
              }
            }
          }
        }

        //Processing for anonymous/braceless scope
        if (elem.type == "function")
        {
            int scope = 1;
            int functionIndex = 0;
            for (int i = 0; i < repo_.locations.Count; ++i)
            {
                Elem temp = repo_.locations[i];
                if (temp.type == elem.type && temp.name == elem.name)
                {
                    functionIndex = i;
                    break;
                }
            }
            for (int i = functionIndex+1; i < repo_.locations.Count; ++i)
            {
                scope++;
            }
            //repo_.setLocations(repo_.locations.GetRange(0, functionIndex+1));
            repo_.locations.RemoveRange(functionIndex + 1, repo_.locations.Count - functionIndex - 1);
            elem.scope = scope;
            repo_.locations[functionIndex].scope = scope;
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
      //if(local[0] == "control")
        //return;

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
      int indexCL = semi.Contains("class");
      int indexIF = semi.Contains("interface");
      int indexST = semi.Contains("struct");

      int index = Math.Max(indexCL, indexIF);
      index = Math.Max(index, indexST);
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
  // rule to detect struct declarations

  public class DetectStruct: ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int index = semi.Contains("struct");
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
  // rule to detect delegate declarations

  public class DetectDelegate : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int index = semi.Contains("delegate");
          if (index != -1)
          {
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              // create local semiExp with tokens for type and name
              local.displayNewLines = false;
              local.Add(semi[index]).Add(semi[index + 2]);
              doActions(local);
              return true;
          }
          return false;
      }
  }

  /////////////////////////////////////////////////////////
  // rule to detect enum declarations

  public class DetectEnum : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int index = semi.Contains("enum");
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
  // rule to dectect function definitions

  public class DetectFunction : ARule
  {
    public static bool isSpecialToken(string token)
    {
      string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
      foreach (string stoken in SpecialToken)
        if (stoken == token)
          return true;
      return false;
    }
    public override bool test(CSsemi.CSemiExp semi)
    {
      if (semi[semi.count - 1] != "{")
        return false;

      int index = semi.FindFirst("(");
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
  
  // detect braceless scope
  public class DetectBracelessScope : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          string[] SpecialToken = { "if", "for", "foreach", "while", "else"};
          int index1 = semi.FindFirst("(");
          int index2 = semi.FindLast(")");
          Boolean isBraceless = false;
          if (index1 != -1 && index2 != -1) { 
              foreach (string stoken in SpecialToken)
                  if (stoken == semi[index1-1])
                      if(semi[index2+1] != "{")
                        isBraceless = true;
          }
          if (isBraceless)
          {
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              // create local semiExp with tokens for type and name
              local.displayNewLines = false;
              local.Add("control").Add("braceless");
              doActions(local);
              return true;
          }
          return false;
      }
  }
  // detect inheritance
  public class DetectClasInherit : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int classIndex = semi.FindFirst("class");
          if (classIndex != -1)
          {
              CSsemi.CSemiExp local1 = new CSsemi.CSemiExp();
              local1.displayNewLines = false;
              local1.Add("className").Add(semi[classIndex + 1]);
              doActions(local1);

              int colonIndex = semi.FindFirst(":");
              if (colonIndex != -1)
              {                                    
                  CSsemi.CSemiExp local2 = new CSsemi.CSemiExp();
                  // create local semiExp with tokens for type and name                  
                  local2.Add("inheritance").Add(semi[colonIndex + 1]);                  
                  doActions(local2);
                  return true;
              }
          }
          return false;
      }
  }
  // detect composition
  public class DetectComposition : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int index1 = semi.Contains("enum");
          int index2 = semi.Contains("struct");
          int index = Math.Max(index1, index2);

          if (index != -1)
          {
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              // create local semiExp with tokens for type and name
              local.displayNewLines = false;
              local.Add("composition").Add(semi[index + 1]);
              doActions(local);
              return true;
          }
          return false;
      }
  }
    //detect using
  public class DetectUsing: ARule
  {
      public static bool isSpecialToken(string token)
      {
          string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
          foreach (string stoken in SpecialToken)
              if (stoken == token)
                  return true;
          return false;
      }
      public override bool test(CSsemi.CSemiExp semi)
      { 
          if (semi[semi.count - 1] != "{")
              return false;

          int indexOpen = semi.FindFirst("(");
          
          if (indexOpen > 0 && !isSpecialToken(semi[indexOpen - 1]))
          {              
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              int indexClose = semi.FindFirst(")");
              local.Add("using");
              //, "<", ">", "List", "[", "]" 
              for (int i = indexOpen + 1; i <= indexClose - 1; i++)
              {
                  /*if(!isInbuiltType(semi[i])){
                      local.Add(semi[i]);                      
                      Console.WriteLine("Semi[{0}] is {1}", i, semi[i]);                      
                  }*/
                  
                  if (isInbuiltType(semi[i]))
                  {
                      if (semi[i + 1].Equals("["))
                      {
                          int index = semi.FindFirst("]");
                          if(index!=-1)
                            i = index;
                      }
                      else if (semi[i + 1].Equals("<"))
                      {
                          int index = semi.FindFirst(">");
                          if(index!=-1)
                            i = index;
                      }
                  }
                  else
                  {
                      if (semi[i + 1].Equals("["))
                      {
                          int index = semi.FindFirst("]");
                          if (index != -1)
                              i = index;
                      }
                      else if (semi[i + 1].Equals("<"))
                      {
                          int index = semi.FindFirst(">");
                          if (index != -1)
                              i = index;
                      }
                      local.Add(semi[i]);                      
                  }
                  if (i < indexClose - 2) { 
                        i++;
                  }
                  else
                  {
                      break;
                  }
              }
              //Add(semi[indexOpen + 2]);
                  //local.Add("function").Add(semi[index - 1]);
                  //doActions(local);
                doActions(local);

              return true;
          }
          return false;
      }

      public static bool isInbuiltType(string token)
      {
          string[] inbuiltTypes = { "int", "string", "String", "bool", "Boolean", "double", "Double", "long", "float"};
          foreach (string stoken in inbuiltTypes)
              if (stoken == token)
                  return true;
          return false;
      }
  }

  // detect aggregation
  public class DetectAggregation: ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int index1 = semi.Contains("new");          
          if (index1 != -1)
          {
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              // create local semiExp with tokens for type and name

              if (!semi[index1].Equals("<"))
              {
                  local.Add("aggregation").Add(semi[index1+1]);
              }
              else{
                  int index2 = semi.FindFirst(">");
                  local.Add("aggregation").Add(semi[index2+1]);
              }
              local.displayNewLines = false;
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
 
  public class BuildCodeAnalyzer
  {
    Repository repo = new Repository();
    public Boolean relationships {get; set;}
    
    public BuildCodeAnalyzer(CSsemi.CSemiExp semi)
    {
      repo.semi = semi;
    }
    public BuildCodeAnalyzer(CSsemi.CSemiExp semi, Boolean relationships)
    {
        repo.semi = semi;
        this.relationships = relationships;
        repo.isOpRel = relationships;
    }
    public virtual Parser build()
    {
      Parser parser = new Parser();

      // decide what to show
      AAction.displaySemi = false;
      AAction.displayStack = false;  // this is default so redundant

      // action used for namespaces, classes, and functions
      PushStack push = new PushStack(repo);
      if (!relationships)
      {
          // capture namespace info
          DetectNamespace detectNS = new DetectNamespace();
          detectNS.add(push);
          parser.add(detectNS);

          // capture class info
          DetectClass detectCl = new DetectClass();
          detectCl.add(push);
          parser.add(detectCl);

          // capture function info
          DetectFunction detectFN = new DetectFunction();
          detectFN.add(push);
          parser.add(detectFN);

          //capture struct info
          DetectStruct detectSt = new DetectStruct();
          detectSt.add(push);
          parser.add(detectSt);

          //capture delegate info
          DetectDelegate detectDel = new DetectDelegate();
          detectDel.add(push);
          parser.add(detectDel);

          //capture enum info
          DetectEnum detectEn = new DetectEnum();
          detectEn.add(push);
          parser.add(detectEn);

          // handle entering anonymous scopes, e.g., if, while, etc.
          DetectAnonymousScope anon = new DetectAnonymousScope();
          anon.add(push);
          parser.add(anon);

          // handle entering braceless scopes, e.g., if, while, etc.
          DetectBracelessScope brace = new DetectBracelessScope();
          brace.add(push);
          parser.add(brace);

          // handle leaving scopes
          DetectLeavingScope leave = new DetectLeavingScope();
          PopStack pop = new PopStack(repo);
          leave.add(pop);
          parser.add(leave);
      }
      else
      {
          //handle inheritance            
          DetectClasInherit inherit= new DetectClasInherit();
          inherit.add(push);
          parser.add(inherit);
          
          //handle composition
          DetectComposition detComp = new DetectComposition();
          detComp.add(push);
          parser.add(detComp);

          //handle using
          DetectUsing usingRel = new DetectUsing();
          usingRel.add(push);
          parser.add(usingRel);

          //handle entering scope
          DetectAnonymousScope detAnon = new DetectAnonymousScope();
          detAnon.add(push);
          parser.add(detAnon);

          //handle aggregation
          DetectAggregation aggre= new DetectAggregation();
          aggre.add(push);
          parser.add(aggre);

          // handle leaving scopes
          DetectLeavingScope leave = new DetectLeavingScope();
          PopStack pop = new PopStack(repo);
          leave.add(pop);
          parser.add(leave);
      }

      // parser configured
      return parser;
    }
  }
}

