using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using CodeAnalysis;
using HandCraftedService;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        String AbsPath_Server1, AbsPath_Server2;
        List<String> Server1_FilePathList = new List<string>();
        List<String> Server2_FilePathList = new List<string>();
        public static int number_clients;
        List<String> Server1_DirectoryPathList = new List<string>();
        List<String> Server2_DirectoryPathList = new List<string>();
        /*
         String Server1_Directory = Directory.GetParent(Directory.GetParent(
             Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString();
         String Server2_Directory = Directory.GetParent(Directory.GetParent(
             Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString();
          */
        String Server1_Directory = Directory.GetCurrentDirectory();
        String Server2_Directory = Directory.GetCurrentDirectory();

        //  String mainWindowName = "Remote Dependency Analyser";
        bool nested_directory_search_server1 = false, nested_directory_search_server2 = false;
        bool type_dependency_analysis_only = false;
        bool package_dependency_analysis_only = false;

        OpenFileDialog ofd1 = new OpenFileDialog();
        OpenFileDialog ofd2 = new OpenFileDialog();

        FolderBrowserDialog dlg1 = new FolderBrowserDialog();
        FolderBrowserDialog dlg2 = new FolderBrowserDialog();

        List<string> commandLineString_Server1 = new List<string>();
        List<string> commandLineString_Server2 = new List<string>();


        public static Dictionary<String, RequestDetails> clientData;
        private string clientName;
        private string clientUrl;
        private List<string> serverList;


        public MainWindow(string clientName, string clientUrl)
        {
            InitializeComponent();
            this.clientName = clientName;
            this.clientUrl = DependencyConstants.localhostUrlPrefix + clientUrl +
            "/" + DependencyConstants.clientPath;
            Console.WriteLine("Opening window for " + clientName);
        }



        private void AddButton1_Click(object sender, RoutedEventArgs e)
        {


            ofd1.Title = "\n  Select Files on Server 1";
            ofd1.InitialDirectory = Server1_Directory;
            ofd1.Multiselect = true;
            ofd1.ShowDialog();
            if (ofd1.FileNames != null)
            {
                foreach (String fileName in ofd1.FileNames)
                {
                    AbsPath_Server1 = fileName;
                    if (Server1_FilePathList == null)
                    {
                        Server1_FilePathList = new List<string>();
                    }
                    Server1_FilePathList.Add(AbsPath_Server1);
                    listBox1.Items.Add(Server1_FilePathList[Server1_FilePathList.Count - 1]);
                    foreach (String s in Server1_FilePathList)
                    {
                        Console.WriteLine("Paths: " + s);
                        Console.WriteLine("---------------------");
                    }
                    listBox1.Items.Refresh();
                }
            }

        }

        private void AddButton2_Click(object sender, RoutedEventArgs e)
        {
            //selectedPath_Server2 = Server2_Directory;

            ofd2.Title = "\n  Select Files on Server 2";
            ofd2.InitialDirectory = Server2_Directory;
            ofd2.Multiselect = true;
            ofd2.ShowDialog();
            if (ofd2.FileNames != null)
            {
                foreach (String fileName in ofd2.FileNames)
                {
                    Console.WriteLine("Server 2 files " + fileName);
                    AbsPath_Server2 = fileName;
                    if (Server2_FilePathList == null)
                    {
                        Server2_FilePathList = new List<string>();
                    }
                    Server2_FilePathList.Add(AbsPath_Server2);
                    listBox2.Items.Add(Server2_FilePathList[Server2_FilePathList.Count - 1]);
                    foreach (String s in Server2_FilePathList)
                    {
                        Console.WriteLine("Paths: " + s);
                        Console.WriteLine("---------------------");
                    }
                    listBox2.Items.Refresh();
                }
            }


        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }

        private void RemoveButton1_Click(object sender, RoutedEventArgs e)
        {
            String selectedItem = (String)listBox1.SelectedItem;
            if (selectedItem != null)
                Server1_FilePathList.Remove(selectedItem);
            listBox1.Items.Remove(listBox1.SelectedItem);
            foreach (String s in Server1_FilePathList)
                Console.WriteLine(s);

            listBox1.Items.Refresh();

        }

        private void RemoveButton2_Click(object sender, RoutedEventArgs e)
        {
            String selectedItem = (String)listBox2.SelectedItem;
            if (selectedItem != null)
                Server2_FilePathList.Remove(selectedItem);
            listBox2.Items.Remove(listBox2.SelectedItem);
            foreach (String s in Server2_FilePathList)
                Console.WriteLine(s);
            listBox2.Items.Refresh();

        }

        private void enableActions1(object sender, RoutedEventArgs e)
        {

            Select_Files_1.IsEnabled = true;
            Select_Directory_1.IsEnabled = true;
            //PatternCheckBox1.IsEnabled = true;

            /*listBox1.IsEnabled = true;
            addButtonServer1.IsEnabled = true;
            removeButtonServer1.IsEnabled = true;*/
        }

        private void disableActions1(object sender, RoutedEventArgs e)
        {
            /*listBox1.IsEnabled = false;
            addButtonServer1.IsEnabled = false;
            removeButtonServer1.IsEnabled = false;*/
            Select_Files_1.IsChecked = false;
            Select_Files_1.IsEnabled = false;
            Select_Directory_1.IsChecked = false;
            Select_Directory_1.IsEnabled = false;
            //PatternCheckBox1.IsChecked = false;
            //PatternCheckBox1.IsEnabled = false;
            if (Server1_DirectoryPathList != null)
            {
                foreach (String s in Server1_DirectoryPathList)
                    listBox1.Items.Remove(s);
                listBox1.Items.Refresh();
                Server1_DirectoryPathList = null;

                foreach (String s1 in Server1_FilePathList)
                    listBox1.Items.Remove(s1);
                listBox3.Items.Refresh();
                Server1_FilePathList = null;
            }
        }

        private void enableActions2(object sender, RoutedEventArgs e)
        {
            Select_Files_2.IsEnabled = true;
            Select_Directory_2.IsEnabled = true;
            
        }

        private void disableActions2(object sender, RoutedEventArgs e)
        {
            Select_Files_2.IsChecked = false;
            Select_Directory_2.IsChecked = false;
            //PatternCheckBox2.IsChecked = false;
            Select_Files_2.IsEnabled = false;
            Select_Directory_2.IsEnabled = false;
            //PatternCheckBox2.IsEnabled = false;
            if (Server2_DirectoryPathList != null)
            {
                foreach (String s in Server2_DirectoryPathList)
                    listBox2.Items.Remove(s);
                listBox2.Items.Refresh();
                Server2_DirectoryPathList = null;

                foreach (String s1 in Server2_FilePathList)
                    listBox4.Items.Remove(s1);
                listBox4.Items.Refresh();
                Server2_FilePathList = null;
            }

        }

        private void onSubmit(object sender, RoutedEventArgs e)
        {
            clientData = new Dictionary<string, RequestDetails>();
            serverList = new List<string>();
            if ((Server1_FilePathList == null) && (Server2_FilePathList == null) && (Server1_DirectoryPathList == null) && (Server2_DirectoryPathList == null))
            {
                System.Windows.MessageBoxResult msgBox = System.Windows.MessageBox.Show("No files/directories chosen to process from either servers");
                return;
            }
            else if ((Server1_FilePathList == null || Server1_FilePathList.Count == 0)
                && (Server2_FilePathList == null || Server2_FilePathList.Count == 0)
                && (Server1_DirectoryPathList == null || Server1_DirectoryPathList.Count == 0)
                && (Server2_DirectoryPathList == null || Server2_DirectoryPathList.Count == 0))
            {
                System.Windows.MessageBoxResult msgBox = System.Windows.MessageBox.Show("No files/directories chosen to process from either servers");
                return;
            }

            if (Server1_DirectoryPathList != null && Server1_DirectoryPathList.Count != 0)
            {
                foreach (String s1 in Server1_DirectoryPathList)
                {
                    commandLineString_Server1.Add(s1);
                }
            }
            if (Server1_FilePathList != null && Server1_FilePathList.Count != 0)
            {
                foreach (String s2 in Server1_FilePathList)
                {
                    String[] tokens;
                    String relativePath = "";
                    if (!s2.ElementAt(s2.Length - 1).Equals("\\"))
                    {
                        tokens = s2.Split('\\');

                        for (int c = 0; c < tokens.Length; c++)
                            Console.WriteLine("Tokens: " + tokens[c]);
                        relativePath = tokens[tokens.Length - 1];
                    }
                    Console.WriteLine("Relative path extracted server 2: " + relativePath);
                    commandLineString_Server1.Add(relativePath);
                }
            }
            else
            {
                commandLineString_Server1.Add("*.cs");
            }
            if (nested_directory_search_server1 == true)
                commandLineString_Server1.Add("/S");
            if (package_dependency_analysis_only == true)
                commandLineString_Server1.Add("/P");
            if (type_dependency_analysis_only == true)
                commandLineString_Server1.Add("/R");

            Console.WriteLine("String server 1 : " + commandLineString_Server1.ToString());
            RequestDetails requestDetails = new RequestDetails();
            requestDetails.clientName = this.clientName;
            requestDetails.serverName = "Server1";
            requestDetails.clientUrl = this.clientUrl;
            requestDetails.args = commandLineString_Server1.ToArray();
            //  clientData.Add("Server1", requestDetails);


            if (Server2_DirectoryPathList != null && Server2_DirectoryPathList.Count != 0)
            {
                foreach (String s3 in Server2_DirectoryPathList)
                {
                    commandLineString_Server2.Add(s3);
                }
            }
            if (Server2_FilePathList != null && Server2_FilePathList.Count != 0)
            {
                foreach (String s4 in Server2_FilePathList)
                {
                    String[] tokens;
                    String relativePath = "";
                    if (!s4.ElementAt(s4.Length - 1).Equals("\\"))
                    {
                        tokens = s4.Split('\\');

                        for (int c = 0; c < tokens.Length; c++)
                            Console.WriteLine("Tokens: " + tokens[c]);
                        relativePath = tokens[tokens.Length - 1];
                    }
                    Console.WriteLine("Relative path extracted server 2: " + relativePath);
                    commandLineString_Server2.Add(relativePath);
                }
            }
            else
            {
                commandLineString_Server2.Add("*.cs");
            }
            if (nested_directory_search_server2 == true)
                commandLineString_Server2.Add("/S");
            if (package_dependency_analysis_only == true)
                commandLineString_Server2.Add("/P");
            if (type_dependency_analysis_only == true)
                commandLineString_Server2.Add("/R");
            
            Console.WriteLine("String server 2 : " + commandLineString_Server2.ToString());
            RequestDetails requestDetails1 = new RequestDetails();
            requestDetails1.clientUrl = this.clientUrl;
            requestDetails1.clientName = this.clientName;
            requestDetails1.serverName = "Server2";
            requestDetails1.args = commandLineString_Server2.ToArray();
            // clientData.Add("Server2", requestDetails1);

            Console.WriteLine("String server 1 reqDetails : " + requestDetails.args.Count());
            Console.WriteLine("String server 2 reqDetails : " + requestDetails1.args.Count());
            if (requestDetails.args.Count() > 0)
            {
                Console.WriteLine("Server 1 is selected");
                clientData.Add("Server1", requestDetails);
                serverList.Add("Server1");
            }
            if (requestDetails1.args.Count() > 0)
            {
                Console.WriteLine("Server 2 is selected");
                clientData.Add("Server2", requestDetails1);
                serverList.Add("Server2");
            }
            if (clientData.ContainsKey("Server1"))
            {
                clientData["Server1"] = requestDetails;
                clientData["Server1"].serverList = serverList;
            }
            if (clientData.ContainsKey("Server2"))
            {
                clientData["Server2"] = requestDetails1;
                clientData["Server2"].serverList = serverList;
            }

            Console.WriteLine("The count is " + clientData.Count);
            Client1.queryServers(clientData);
        }

        private void Nested_Search_Checked_1(object sender, RoutedEventArgs e)
        {
            nested_directory_search_server1 = true;
            Console.WriteLine("nested search: " + nested_directory_search_server1);
        }

        private void Type_Dependencies_Only_Checked(object sender, RoutedEventArgs e)
        {
            type_dependency_analysis_only = true;
            Console.WriteLine("type dependency : " + type_dependency_analysis_only);
        }

        private void Package_Dependencies_Only_Checked(object sender, RoutedEventArgs e)
        {
            package_dependency_analysis_only = true;
            Console.WriteLine("package dependency only: " + package_dependency_analysis_only);
        }


        private void Type_Dependencies_Only_Unchecked(object sender, RoutedEventArgs e)
        {
            type_dependency_analysis_only = false;
            Console.WriteLine("type dependency analysis : " + type_dependency_analysis_only);
        }

        private void Package_Dependencies_Only_Unchecked(object sender, RoutedEventArgs e)
        {
            package_dependency_analysis_only = false;
            Console.WriteLine("package dependency analysis : " + package_dependency_analysis_only);
        }

        private void Nested_Search_Checked_2(object sender, RoutedEventArgs e)
        {
            nested_directory_search_server2 = true;
            Console.WriteLine("nested search: " + nested_directory_search_server2);
        }


        private void Select_Directory_1_Checked(object sender, RoutedEventArgs e)
        {
            listBox3.IsEnabled = true;
            browse1.IsEnabled = true;
            Nested_Directory_Search_1.IsEnabled = true;
        }

        private void Select_Directory_2_Checked(object sender, RoutedEventArgs e)
        {
            listBox4.IsEnabled = true;
            browse2.IsEnabled = true;
            Nested_Directory_Search_2.IsEnabled = true;
        }

        private void Select_Files_1_Checked(object sender, RoutedEventArgs e)
        {
            listBox1.IsEnabled = true;
            addButtonServer1.IsEnabled = true;
            removeButtonServer1.IsEnabled = true;
            //PatternCheckBox1.IsEnabled = true;
        }

        private void Select_Files_2_Checked(object sender, RoutedEventArgs e)
        {
            listBox2.IsEnabled = true;
            addButtonServer2.IsEnabled = true;
            removeButtonServer2.IsEnabled = true;
        }

        private void Select_Files_1_Unchecked(object sender, RoutedEventArgs e)
        {

            Server1_FilePathList = null;
            Server1_FilePathList = new List<string>();
            listBox1.IsEnabled = false;
            addButtonServer1.IsEnabled = false;
            removeButtonServer1.IsEnabled = false;
            Select_Files_1.IsEnabled = true;
            Select_Directory_1.IsEnabled = true;
            //PatternCheckBox1.IsEnabled = true;
            if (listBox1 != null || listBox1.Items.Count != 0)
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                    listBox1.Items.RemoveAt(i);
                listBox1.Items.Refresh();
            }
            //Console.WriteLine("Pattern box 1 value " + PatternCheckBox1.IsEnabled);
        }

        private void Select_Directory_1_Unchecked(object sender, RoutedEventArgs e)
        {
            Server1_DirectoryPathList = null;
            Server1_DirectoryPathList = new List<string>();
            listBox3.IsEnabled = false;
            browse1.IsEnabled = false;
            Nested_Directory_Search_1.IsChecked = false;
            Nested_Directory_Search_1.IsEnabled = false;
            Select_Files_1.IsEnabled = true;
            Select_Directory_1.IsEnabled = true;
            //PatternCheckBox1.IsEnabled = true;
            //PatternCheckBox1.IsChecked = false;
            if (listBox3 != null || listBox3.Items.Count != 0)
            {
                for (int i = 0; i < listBox3.Items.Count; i++)
                    listBox3.Items.RemoveAt(i);
                listBox3.Items.Refresh();
            }

        }

        private void Browse1_Clicked(object sender, RoutedEventArgs e)
        {
            dlg1.ShowNewFolderButton = false;
            dlg1.Description = "\n  Select Folders on Server 1";
            dlg1.SelectedPath = Server1_Directory;
            DialogResult dr = dlg1.ShowDialog();
            if (dr.ToString() == "OK")
            {
                Server1_Directory = dlg1.SelectedPath;
                if (Server1_DirectoryPathList == null)
                {
                    Server1_DirectoryPathList = new List<string>();
                }
                Server1_DirectoryPathList.Add(Server1_Directory);
                listBox3.Items.Add(Server1_DirectoryPathList[Server1_DirectoryPathList.Count - 1]);
                foreach (String s in Server1_DirectoryPathList)
                    Console.WriteLine(s);
                listBox3.Items.Refresh();
            }
        }

        private void Nested_Search_Unchecked_1(object sender, RoutedEventArgs e)
        {
            nested_directory_search_server1 = false;
            Console.WriteLine("nested search: " + nested_directory_search_server1);

        }

        private void Nested_Search_Unchecked_2(object sender, RoutedEventArgs e)
        {
            nested_directory_search_server1 = false;
            Console.WriteLine("nested search: " + nested_directory_search_server2);
        }

        private void Select_Files_2_Unchecked(object sender, RoutedEventArgs e)
        {
            Server2_FilePathList = null;
            Server2_FilePathList = new List<string>();
            listBox2.IsEnabled = false;
            addButtonServer2.IsEnabled = false;
            removeButtonServer2.IsEnabled = false;
            Select_Files_2.IsEnabled = true;
            Select_Directory_2.IsEnabled = true;
            //PatternCheckBox2.IsEnabled = true;
            if (listBox2 != null || listBox2.Items.Count != 0)
            {
                for (int i = 0; i < listBox2.Items.Count; i++)
                    listBox2.Items.RemoveAt(i);
                listBox2.Items.Refresh();
            }

        }

        private void Select_Directory_2_Unchecked(object sender, RoutedEventArgs e)
        {
            Server2_DirectoryPathList = null;
            Server2_DirectoryPathList = new List<string>();
            listBox4.IsEnabled = false;
            browse2.IsEnabled = false;
            Nested_Directory_Search_2.IsChecked = false;
            Nested_Directory_Search_2.IsEnabled = false;
            Select_Files_2.IsEnabled = true;
            Select_Directory_2.IsEnabled = true;            
            if (listBox4 != null || listBox4.Items.Count != 0)
            {
                for (int i = 0; i < listBox4.Items.Count; i++)
                    listBox4.Items.RemoveAt(i);
                listBox4.Items.Refresh();
            }

        }

        private void Browse2_Clicked(object sender, RoutedEventArgs e)
        {
            dlg2.ShowNewFolderButton = false;
            dlg2.Description = "\n  Select Folders on Server 2";
            dlg2.SelectedPath = Server2_Directory;
            DialogResult dr = dlg2.ShowDialog();
            if (dr.ToString() == "OK")
            {
                Server2_Directory = dlg2.SelectedPath;
                if (Server2_DirectoryPathList == null)
                {
                    Server2_DirectoryPathList = new List<string>();
                }
                Server2_DirectoryPathList.Add(Server2_Directory);
                listBox4.Items.Add(Server2_DirectoryPathList[Server2_DirectoryPathList.Count - 1]);
                foreach (String s in Server2_DirectoryPathList)
                    Console.WriteLine(s);
                listBox4.Items.Refresh();
            }
        }
    }
}