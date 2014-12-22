using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzerConstants
{
    class Constants
    {
        public static readonly String DEFAULT_PATH = "../../";
        public static readonly String DEFAULT_PATTERN = ".cs";
        public static readonly String SUBDIRECTORY_OPTION = "\\S";
        public static readonly String RELATIONSHIP_OPTION = "\\R";
        public static readonly String XML_OPTION = "\\X";
        public static readonly String CLASS_TYPE_NAME = "class";
        public static readonly String DELEGATE_TYPE_NAME = "delegate";
        public static readonly String INTERFACE_TYPE_NAME = "interface";
        public static readonly String FUNCTION_NAME = "function";
        public static readonly String SWITCH_CASES_NAME = "case";
        public static readonly String TERNARY_OPERATOR_SYMBOL = "?";
        public static readonly String NEW_OPERATOR = "new";
        public static readonly String EQUAL_TO_OPERATOR = "=";
        public static readonly String STRUCT_OPERATOR = "struct";
        public static readonly String ENUM_OPERATOR = "enum";
        public static readonly String RELATIONSHIP_ANALYZER_TYPE = "relationship";
        public static readonly String INHERITANCE_RELATIONSHIP_NAME = "inheritance";
        public static readonly String AGGREGATION_RELATIONSHIP_NAME = "aggregation";
        public static readonly String COMPOSITION_RELATIONSHIP_NAME = "composition";
        public static readonly String USING_RELATIONSHIP_NAME = "using";
        public static readonly String DELEGATE_USING_RELATIONSHIP_NAME = "delegate_using";  
        public static readonly String[] STANDARD_DATA_TYPES = { "bool", "byte", "sbyte", "char", 
                                         "decimal", "float", "int", "uint", 
                                         "long", "ulong", "object", "short",
                                         "ushort", "string", "String" };
        public static readonly String[] NON_COMPOSED_CONSTANTS = { "bool", "byte", "sbyte", "char", 
                                         "decimal", "float", "int", "uint", 
                                         "long", "ulong", "object", "short",
                                         "ushort", "string", "String","using","{" };
        public static readonly String[] ACCESS_MODIFIERS = {"public","private",
                                                   "internal", "protected",
                                                          "final","static", "readonly"};
        public static readonly String[] NON_COMPOSED_MODIFIERS = {"=","new",
                                                   ":","readonly","static","\n","\t","using"};
   
    }
}
