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

namespace YP01
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Frame the_root_frame;

        public MainWindow()
        {
            InitializeComponent();
            the_root_frame = this.fr_the_root;
            the_root_frame.Navigate(new Uri("Pages/AuthPage.xaml", UriKind.Relative));
        }
    }
}
