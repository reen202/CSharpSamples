///////////////////////////////////////////////////////////////////////////
// IRuleAndAction.cs - Interfaces & abstract bases for rules and actions //
// ver 1.1                                                               //
// Language:    C#, 2008, .Net Framework 4.0                             //
// Platform:    Dell Precision T7400, Win7, SP1                          //
// Application: Demonstration for CSE681, Project #2, Fall 2011          //
// Author:      Jim Fawcett, CST 4-187, Syracuse University              //
//              (315) 443-3948, jfawcett@twcny.rr.com                    //
///////////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following classes:
 *   IRule   - interface contract for Rules
 *   ARule   - abstract base class for Rules that defines some common ops
 *   IAction - interface contract for rule actions
 *   AAction - abstract base class for actions that defines common ops
 */
/* Required Files:
 *   IRuleAndAction.cs
 *   
 * Build command:
 *   Interfaces and abstract base classes only so no build
 *   
 * Maintenance History:
 * --------------------
 * ver 1.1 : 11 Sep 2011
 * - added properties displaySemi and displayStack
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 * Note:
 * This package does not have a test stub as it contains only interfaces
 * and abstract classes.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using AnalyzerConstants;

namespace RulesAndActions
{
  /////////////////////////////////////////////////////////
  // contract for actions used by parser rules

  public interface IAction
  {
    void doAction(CSsemi.CSemiExp semi);
    void doAction(CSsemi.CSemiExp semi, RelationShipElem elem,  int fetchedIndex);
  }
  /////////////////////////////////////////////////////////
  // abstract action base supplying common functions

  public abstract class AAction : IAction
  {
    static int _scopeCounter = 0;
    static bool _isScopeCounterSet = false;
    static bool displaySemi_ = false;   // default
    static bool displayStack_ = false;  // default

    public abstract void doAction(CSsemi.CSemiExp semi);
    public abstract void doAction(CSsemi.CSemiExp semi, RelationShipElem elem, int fetchedIndex);

    public static int scopeCounter
    {
        get { return _scopeCounter; }
        set { _scopeCounter = value; }
    }

    public static bool isScopeCounterSet
    {
        get { return _isScopeCounterSet; }
        set { _isScopeCounterSet = value; }
    }

    public static bool displaySemi 
    {
      get { return displaySemi_; }
      set { displaySemi_ = value; }
    }
    public static bool displayStack 
    {
      get { return displayStack_; }
      set { displayStack_ = value; }
    }

    public static void increaseScopeCounter()
    {
        scopeCounter++;
    }

    public static void resetScopeCounter()
    {
        scopeCounter = 0;
    }

    public virtual void display(CSsemi.CSemiExp semi)
    {
      if(displaySemi)
        for (int i = 0; i < semi.count; ++i)
          Console.Write("{0} ", semi[i]);
    }
  }
  /////////////////////////////////////////////////////////
  // contract for parser rules

  public interface IRule
  {
    bool test(CSsemi.CSemiExp semi);
    void add(IAction action);
  }
  /////////////////////////////////////////////////////////
  // abstract rule base implementing common functions

  public abstract class ARule : IRule
  {
    private List<IAction> actions;
    public ARule()
    {
      actions = new List<IAction>();
    }
    public void add(IAction action)
    {
      actions.Add(action);
    }
    abstract public bool test(CSsemi.CSemiExp semi);
    public void doActions(CSsemi.CSemiExp semi)
    {
      foreach (IAction action in actions)
      action.doAction(semi);
    }

    public void doActions(CSsemi.CSemiExp semi, RelationShipElem elem,  int fetchedIndex)
    {
        foreach (IAction action in actions)
            action.doAction(semi, elem,fetchedIndex);
    }

   
    public int indexOfType(CSsemi.CSemiExp semi)
    {
      int indexCL = semi.Contains("class");
      int indexIF = semi.Contains("interface");
      int indexST = semi.Contains("struct");
      int indexEN = semi.Contains("enum");

      int index = Math.Max(indexCL, indexIF);
      index = Math.Max(index, indexST);
      index = Math.Max(index, indexEN);
      return index;
    }

    public String checkNonComposedConstantsPresence(String passedDataType)
    {   String fetchedData = null;
    foreach (String dataType in Constants.NON_COMPOSED_CONSTANTS)
        {
            if(String.Equals(passedDataType,dataType))
            {
                fetchedData = dataType;
            }
        }
        return fetchedData;
    }

    public String checkModifierPresence(String passedModiferType)
    {   String fetchedData = null;
        foreach(String dataType in Constants.ACCESS_MODIFIERS)
        {
            if(String.Equals(passedModiferType,dataType))
            {
                fetchedData = dataType;
            }
        }
        return fetchedData;
    }

      public String checkOtherKeywordsPresence(String passedModiferType)
    {   String fetchedData = null;
        foreach(String dataType in Constants.NON_COMPOSED_MODIFIERS)
        {
            if(String.Equals(passedModiferType,dataType))
            {
                fetchedData = dataType;
            }
        }
        return fetchedData;
    }
}
  }


