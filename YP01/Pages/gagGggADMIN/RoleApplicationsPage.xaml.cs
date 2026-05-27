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
    /// Логика взаимодействия для RoleApplicationsPage.xaml
    /// </summary>
    public partial class RoleApplicationsPage : Page
    {
        public RoleApplicationsPage()
        {
            InitializeComponent();
            load_data();
        }

        private void load_data()
        {
            // Исправлено: RoleApplication → ServiceRequests (где RequestTypeId = 1)
            var applications = Core.db.ServiceRequests
                .Where(r => r.RequestTypeId == 1)  // 1 = заявка на роль автора
                .ToList()
                .Select(a => new RoleApplicationDisplayItem
                {
                    application_obj = a,
                    user_name = a.Accounts?.DisplayName ?? "Неизвестный",
                    reason = a.Content ?? ""
                }).ToList();

            dg_applications.ItemsSource = applications;
        }

        private void btn_accept_Click(object sender, RoutedEventArgs e)
        {
            if (dg_applications.SelectedItem is RoleApplicationDisplayItem item)
            {
                // Находим пользователя
                var user = Core.db.Accounts.FirstOrDefault(u => u.Id == item.application_obj.UserId);
                if (user != null)
                {
                    // Назначаем роль "Автор" (RoleId = 2)
                    user.RoleId = 2;
                }

                // Удаляем заявку
                Core.db.ServiceRequests.Remove(item.application_obj);
                Core.db.SaveChanges();
                load_data();
                MessageBox.Show("Заявка принята. Роль 'Автор' назначена.");
            }
        }

        private void btn_reject_Click(object sender, RoutedEventArgs e)
        {
            if (dg_applications.SelectedItem is RoleApplicationDisplayItem item)
            {
                // Просто удаляем заявку
                Core.db.ServiceRequests.Remove(item.application_obj);
                Core.db.SaveChanges();
                load_data();
                MessageBox.Show("Заявка отклонена.");
            }
        }
    }

    public class RoleApplicationDisplayItem
    {
        // Исправлено: RoleApplication → ServiceRequests
        public ServiceRequests application_obj { get; set; }
        public string user_name { get; set; }
        public string reason { get; set; }
    }
}
