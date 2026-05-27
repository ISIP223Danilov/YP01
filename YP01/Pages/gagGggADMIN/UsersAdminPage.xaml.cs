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

namespace YP01.Pages.AdminSub
{
    /// <summary>
    /// Логика взаимодействия для UsersAdminPage.xaml
    /// </summary>
    public partial class UsersAdminPage : Page
    {
        public List<UserRoles> roles { get; set; }

        public UsersAdminPage()
        {
            InitializeComponent();
            // Исправлено: Role → UserRoles
            roles = Core.db.UserRoles.ToList();
            DataContext = this;
            load_data();
        }

        private void load_data()
        {
            // Исправлено: User → Accounts
            dg_users.ItemsSource = Core.db.Accounts.ToList();
        }

        private void role_cbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && (sender as ComboBox)?.DataContext is Accounts user)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Core.db.SaveChanges();
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void btn_change_password_Click(object sender, RoutedEventArgs e)
        {
            // Исправлено: User → Accounts
            if (dg_users.SelectedItem is Accounts user)
            {
                if (!string.IsNullOrWhiteSpace(tb_new_password.Text))
                {
                    user.Password = tb_new_password.Text;
                    Core.db.SaveChanges();
                    MessageBox.Show($"Пароль для {user.DisplayName} изменен.");
                    tb_new_password.Text = "";
                }
                else
                {
                    MessageBox.Show("Введите новый пароль.");
                }
            }
        }

        private void dg_users_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Core.db.SaveChanges();
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void chb_is_frozen_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Core.db.SaveChanges();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
