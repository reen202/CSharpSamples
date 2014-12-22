using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Xml.Serialization;


public static class Algorithms
{
    private static readonly HashAlgorithm SHA1 = new SHA512Managed();

    public static string GetHashFromFile(string fileName)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(fileName))
            {
                return BitConverter.ToString(SHA1.ComputeHash(stream)).Replace("-", string.Empty);
            }
        }
    }

    public static void createXML(string filename, string checksum, DateTime now)
    {
        XDocument xmlDoc = new XDocument();
        xmlDoc.Declaration = new XDeclaration("1.0", "utf-8", "yes");
        XComment comment = new XComment("Demonstration xmlDoc");
        xmlDoc.Add(comment);
        XElement root = new XElement("Metadata");
        xmlDoc.Add(root);
        XElement type = new XElement("Info");
        root.Add(type);

        XElement FileName = new XElement("Filename", filename);
        type.Add(FileName);
        XElement Checksum = new XElement("Checksum", checksum);
        type.Add(Checksum);
        XElement Timestamp = new XElement("Timestamp", now);
        type.Add(Timestamp);



        xmlDoc.Save(Directory.GetCurrentDirectory() + "\\Metadata.xml");
    }

    public static void readXML()
    {
        int count = 0;
        string filename = "", checksum = "", timestamp = "";
        XDocument xDoc = XDocument.Load(Directory.GetCurrentDirectory() + "\\Metadata.xml");
        XDocument doc = XDocument.Parse(xDoc.ToString());
        var qp = from x in
                     doc.Elements("Metadata")
                     .Elements("Info")
                 .Elements()
                 select x;


        foreach (var elem in qp)
        {

            if (count == 0)
            {
                filename = elem.Value;
                count++;
            }

            if (count == 1)
            {
                checksum = elem.Value;
                count++;
            }

            if (count == 2)
            {
                timestamp = elem.Value;
                count = 0;
            }

        }

        // This retrieved data is then compared with metadata file received from the server if commit id 
        // matches with commit id then it will be cache hit else it will be a cache miss

    }


    public static void Main(string[] args)
    {
        string filename = "CsNode.exe";
        string checksumMd5 = GetHashFromFile("C:\\Zzz\\SMAWorkspace\\CsGraph\\CsNode\\bin\\Debug" + "\\" + filename);
        Console.WriteLine("\n\n {0} ", checksumMd5);
        DateTime now = DateTime.Now;
        Console.WriteLine(now);
        createXML(filename, checksumMd5, now);
        readXML();
    }
}