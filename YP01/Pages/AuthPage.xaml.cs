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

namespace YP01.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void mode_Checked(object sender, RoutedEventArgs e)
        {
            if (sp_register_fields == null) return;

            if (rb_register.IsChecked == true)
            {
                sp_register_fields.Visibility = Visibility.Visible;
            }
            else
            {
                sp_register_fields.Visibility = Visibility.Collapsed;
            }
        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            string login = tb_login.Text;
            string password = pb_password.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми.");
                return;
            }

            // РЕЖИМ ВХОДА
            if (rb_login.IsChecked == true)
            {
                // Исправлено: Core.db.User - Core.db.Accounts
                var user = Core.db.Accounts.FirstOrDefault(u => u.Login == login && u.Password == password);
                if (user != null)
                {
                    Core.current_user = user;
                    MainWindow.the_root_frame.Navigate(new Uri("Pages/MainPage.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.");
                }
            }
            // РЕЖИМ РЕГИСТРАЦИИ
            else
            {
                string email = tb_email.Text;
                string display_name = tb_display_name.Text;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(display_name))
                {
                    MessageBox.Show("Email и отображаемое имя не могут быть пустыми.");
                    return;
                }

                // Исправлено: Core.db.User → Core.db.Accounts
                if (Core.db.Accounts.Any(u => u.Login == login))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                    return;
                }

                // Исправлено: User → Accounts
                var new_user = new Accounts
                {
                    Login = login,
                    Password = password,
                    Email = email,
                    DisplayName = display_name,
                    RoleId = 1,  // 1 = Читатель
                    IsFrozen = false
                    // CreatedAt нет в твоей БД
                };

                try
                {
                    Core.db.Accounts.Add(new_user);
                    Core.db.SaveChanges();

                    Core.current_user = new_user;
                    MainWindow.the_root_frame.Navigate(new Uri("Pages/MainPage.xaml", UriKind.Relative));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при регистрации: " + ex.Message);
                }
            }
        }
    }
}
