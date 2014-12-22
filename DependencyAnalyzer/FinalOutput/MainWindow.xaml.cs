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

namespace FinalOutput
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow(string xml)
        {
            
            //textBlock.Text = xml;
            
            Grid myGrid = new Grid();
            ScrollViewer sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            /*StackPanel sp = new StackPanel();
            sp.VerticalAlignment = VerticalAlignment.Top;
            sp.HorizontalAlignment = HorizontalAlignment.Left;
            */            
            myGrid.Children.Add(sv);
            /*TextBox text = new TextBox();
            text.TextWrapping = TextWrapping.Wrap;
            text.Text = xml;*/
            sv.Content=xml;
            this.AddChild(myGrid);
            InitializeComponent();            
        }
    }
}
