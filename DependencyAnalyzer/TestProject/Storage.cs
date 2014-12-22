using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Storage
{
    /**
     * Class SharedDataStorage contains the Map whose key is the file absolute path and value is Repostory object.
     * Why do we require Map ? What are its advantage - 
     * 1. You do not need to browse through the items of other files while iterating location table during pop stack operation.
     * 2. You can retreive the Element object easily in the second pass since you can use filename as the key.
     * 3. While validation Aggregation and Composition, you need to know the line number information of functions since
     *    new keyword can be put in the functions. Since you already know which file this Aggregated or Composed object is, you
     *    can get the line number information of the function in the file. Search for the information of function line numbers
     *    with filepath as the key would help to search quickly from the locations table in the Repository
     * */
    class SharedDataStorage
    {
       
        static Hashtable _data = new Hashtable();
       
        public static Hashtable data
        {
            get { return SharedDataStorage._data; }
            set { SharedDataStorage._data = value; }
        }

       

        public static bool lookUp(string type, string name)
        {
            
            bool exists = false;
            ICollection keys = data.Keys;
            foreach(String key in keys)
            {
                Repository repo = (Repository)data[key];
                List<Elem> locations = repo.locations;
                foreach (Elem e in locations)
                {
                   if(String.Equals(name,e.name,StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(type, e.type, StringComparison.OrdinalIgnoreCase))                   
                    {
                        exists = true;
                        break;
                    }
                }
            }
            return exists;
        }

    }

    public class RelationShipElem
    {
        //type which can be class or interface or struct or namespace or enum
        private String _type;
        public String type
        {
            get { return _type; }
            set { _type = value; }
        }

        //name of the type
        private String _name;
        public String name
        {
            get{ return _name;}
            set{ _name = value;}
        }

        //type of relationship
        private String _relationship;
        public String relationship
        {
            get { return _relationship; }
            set { _relationship = value; }
        }

       
        private int _lineNo;
        public int lineNo
        {
            get { return _lineNo;}
            set{ _lineNo = value;}
        }       

    }


    public class Elem  // holds scope information
    {
        public string type { get; set; }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }
        private int _size;
        public int size
        {
            get { return _size; }
            set { _size = value; }
        }

        private int _scope;
        public int scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        private List<RelationShipElem> _relationshipList = new List<RelationShipElem>();

        internal List<RelationShipElem> relationshipList
        {
            get { return _relationshipList; }
            set { _relationshipList = value; }
        }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");
            temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
            temp.Append(String.Format("{0,-5}", size.ToString()));    // size 
            temp.Append(String.Format("{0,-5}", scope.ToString()));    // scope
            temp.Append("}");
            return temp.ToString();
        }
    }


    public class Repository
    {
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        List<Elem> locations_ = new List<Elem>();
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

    public class ScopeStack<E>
    {
        List<E> stack_ = new List<E>();
        E lastPopped_;

        //----< push element onto stack >------------------------------------

        public void push(E elem)
        {
            stack_.Add(elem);
        }
        //----< pop element off of stack >-----------------------------------

        public E pop()
        {
            int len = stack_.Count;
            if (len == 0)
                throw new Exception("empty scope stack");
            E elem = stack_[len - 1];
            stack_.RemoveAt(len - 1);
            lastPopped_ = elem;
            return elem;
        }
        //----< remove all elements from stack >-----------------------------

        public void clear()
        {
            stack_.Clear();
        }
        //----< index into stack contents >----------------------------------

        public E this[int i]
        {
            get
            {
                if (i < 0 || stack_.Count <= i)
                    throw new Exception("scope stack index out of range");
                return stack_[i];
            }
            set
            {
                if (i < 0 || stack_.Count <= i)
                    throw new Exception("scope stack index out of range");
                stack_[i] = value;
            }
        }
        //----< number of elements on stack property >-----------------------

        public int count
        {
            get { return stack_.Count; }
        }
        //----< get lastPopped >---------------------------------------------

        public E lastPopped()
        {
            return lastPopped_;
        }
        //----< display using element ToString() method() >------------------

        public void display()
        {
            for (int i = 0; i < count; ++i)
            {
                Console.Write("\n  {0}", stack_[i].ToString());
            }
        }
    }
}
