using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace BeanProperties
{
    class CommandLineInputs
    {
        private String path;
        private ArrayList filesAndPatterns = new ArrayList();      
        private String pattern;
        private bool isSubDirectoryOption;
        private bool isXMLOption;
        private bool isRelationshipOption;

        public bool IsRelationshipOption
        {
            get { return isRelationshipOption; }
            set { isRelationshipOption = value; }
        }

        public String Pattern
        {
            get { return pattern; }
            set { pattern = value; }
        }

        public String Path
        {
            get { return path; }
            set { path = value; }
        }

        public ArrayList FilesAndPatterns
        {
            get { return filesAndPatterns; }
            set { filesAndPatterns = value; }
        }

        public bool IsSubDirectoryOption
        {
            get { return isSubDirectoryOption; }
            set { isSubDirectoryOption = value; }
        }


        public bool IsXMLOption
        {
            get { return isXMLOption; }
            set { isXMLOption = value; }
        }

        public override String ToString()
        {
            StringBuilder objectString = new StringBuilder("The Command Line Inputs are : ");
            objectString.AppendLine("The Directory Path is : " + this.Path);
            objectString.AppendLine("The File Path is : ");
            objectString.AppendLine(getFileListString(this.FilesAndPatterns));
            objectString.AppendLine("The \\S option for subdirectory  is : " 
                + this.isSubDirectoryOption);
            objectString.AppendLine("The \\R option for relationship  is : "
                + this.isRelationshipOption);
            objectString.AppendLine("The \\X option for XML output is : "
                + this.isXMLOption);
            return objectString.ToString();
        }

        public String getFileListString(ArrayList files)
        {
            StringBuilder filePathString = new StringBuilder();
            foreach(String fileName in files)
            {
                filePathString.AppendLine("FileName or Pattern  :" + fileName); 
            }
            return filePathString.ToString();
        }
    }
}