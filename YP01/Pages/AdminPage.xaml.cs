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
using YP01.Pages.AdminSub;

namespace YP01.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void cbb_admin_section_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbb_admin_section.SelectedItem is ComboBoxItem selected_item)
            {
                string section = selected_item.Content.ToString();
                switch (section)
                {
                    case "Жалобы":
                        fr_admin.Navigate(new ComplaintsPage());
                        break;
                    case "Заявки на разморозку":
                        fr_admin.Navigate(new UnfreezeRequestsPage());
                        break;
                    case "Заявки на роль автора":
                        fr_admin.Navigate(new RoleApplicationsPage());
                        break;
                    case "Замороженные элементы":
                        fr_admin.Navigate(new FrozenItemsPage());
                        break;
                    case "Пользователи":
                        fr_admin.Navigate(new UsersAdminPage());
                        break;
                }
            }
        }
    }
}
