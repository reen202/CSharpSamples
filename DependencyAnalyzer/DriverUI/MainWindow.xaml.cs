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
using WpfApplication1;
using System.Diagnostics;

namespace DriverUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _number_clients;
        WpfApplication1.MainWindow newForm;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void number_clients_changed(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine("Data in UI " + _number_clients);
            string s = ClientCount.Text;
            int val;
            bool result = Int32.TryParse(s, out val);
            if (result == true)
            {
                _number_clients = val;
                WpfApplication1.MainWindow.number_clients = val;
            }
            Console.WriteLine("Number of clients : " + _number_clients);

        }

        private void onSubmit(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <=_number_clients; i++)
            {
                //ProcessStartInfo ui = new ProcessStartInfo();

                //ui.FileName = @"..\..\..\WpfApplication1\bin\Debug\UserInterface.exe"; 

                //Process.Start(ui);
                string clientName= "Client" + i;
                newForm = new WpfApplication1.MainWindow(clientName, "808"+i);
                newForm.Title = clientName;
                newForm.Show();

                this.Close();
            }
        }


    }
}
