using System;

public class Class1
{
    
    public static void Hello(string s)
    {
        Console.WriteLine("  Hello, {0}!", s);
    }
    public delegate void Del(string message);
        public override void doRelationSpecificAction(Repository repo, CSsemi.CSemiExp semi, RelationShipElem relationshipElem, int indexedPosition)
        {
           
            Del a = new Del(Hello);
            Del a = Hello;

        }
	
}
