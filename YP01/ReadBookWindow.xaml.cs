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
using System.Windows.Shapes;

namespace YP01
{
    /// <summary>
    /// Логика взаимодействия для ReadBookWindow.xaml
    /// </summary>
    public partial class ReadBookWindow : Window
    {
        public ReadBookWindow(string content)
        {
            InitializeComponent();
            tbl_book_content.Text = content;
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
