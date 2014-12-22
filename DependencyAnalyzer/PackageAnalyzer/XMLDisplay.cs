using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
//using SWTools;
namespace CodeAnalysis
{
    class XMLDisplay
    {
        private XmlTextWriter txtwriter1_;

        public XmlTextWriter txtwriter1
        {
            get { return txtwriter1_; }
            set { txtwriter1_ = value; }
        }
        private XmlTextWriter txtwriter2_;

        public XmlTextWriter txtwriter2
        {
            get { return txtwriter2_; }
            set { txtwriter2_ = value; }
        }

        public string[] WriteOutput(ServerToMaster serverTomaster)
        {
            ServerToMaster srv = serverTomaster;
            bool packageCheck = false;
            bool typeCheck = false;
            bool bothCheck= false;            
            if (serverTomaster.requestDetails.args.Contains("/P") || serverTomaster.requestDetails.args.Contains("/p"))
            {
                packageCheck = true;
            }
            if (serverTomaster.requestDetails.args.Contains("/R") || serverTomaster.requestDetails.args.Contains("/r"))
            {
                typeCheck = true;
            }
            if ((packageCheck && typeCheck) || (!packageCheck && !typeCheck))
            {
                bothCheck = true;
            }
            StringWriter str1 = new StringWriter();
            StringWriter str2 = new StringWriter();
            txtwriter1 = new XmlTextWriter(str1);
            txtwriter2 = new XmlTextWriter(str2);
            if(typeCheck || bothCheck)
                getRelationshipAnal(srv);
            if(packageCheck || bothCheck)
                getPackageDependencyAnal(srv);

            string[] stringList = {str1.ToString(), str2.ToString()};
            str1.Close(); ;
            str2.Close();
            return stringList;
        }


        //Writes out relationship info
        private void getRelationshipAnal(ServerToMaster data)
        {
            txtwriter1.Formatting = Formatting.Indented;
            txtwriter1.WriteStartDocument();
            txtwriter1.WriteStartElement("Relationships");
            //get list of filenames
            if (null != data.relTable) { 
                foreach (var t in data.relTable)
                {
                    txtwriter1.WriteStartElement("File");
                    txtwriter1.WriteAttributeString("Name", t.Key);

                    List<ElemRelation> relsClass = t.Value;
                    //for each list get the relationships
                    foreach (var rel in relsClass)
                    {

                        txtwriter1.WriteStartElement("Class");
                        txtwriter1.WriteAttributeString("Name", rel.fromClass);
                        txtwriter1.WriteAttributeString("RelatedTo", rel.toClass);
                        txtwriter1.WriteAttributeString("RelType", rel.relationType);
                        txtwriter1.WriteEndElement();

                    }
                    txtwriter1.WriteEndElement();
                }
            }
            txtwriter1.WriteEndElement();
            txtwriter1.WriteEndDocument();
            txtwriter1.Flush();
            txtwriter1.Close();
        }

        //Writes out PackageDependency info
        private void getPackageDependencyAnal(ServerToMaster data)
        {
            txtwriter2.Formatting = Formatting.Indented;
            txtwriter2.WriteStartDocument();
            txtwriter2.WriteStartElement("PackageDependency");
            //get list of packages
            foreach (var t in data.packageAnalysis)
            {
                txtwriter2.WriteStartElement("Package");
                txtwriter2.WriteAttributeString("Name", t.Key);

                List<String> depPackage = t.Value;

                foreach (var rel in depPackage)
                {
                    txtwriter2.WriteStartElement("DependOn");
                    txtwriter2.WriteAttributeString("Name", rel);
                    txtwriter2.WriteEndElement();
                }
                txtwriter2.WriteEndElement();
            }
            txtwriter2.WriteEndElement();
            txtwriter2.WriteEndDocument();
            txtwriter2.Flush();
            txtwriter2.Close();
        }
        static void Main(string[] args)
        {
        }
    }
}