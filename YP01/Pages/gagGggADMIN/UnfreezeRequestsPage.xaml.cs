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
    /// Логика взаимодействия для UnfreezeRequestsPage.xaml
    /// </summary>
    public partial class UnfreezeRequestsPage : Page
    {
        public UnfreezeRequestsPage()
        {
            InitializeComponent();
            load_data();
        }

        private void load_data()
        {
            // Исправлено: UnfreezeRequest → ServiceRequests (RequestTypeId = 2 или 3)
            var requests = Core.db.ServiceRequests
                .Where(r => r.RequestTypeId == 2 || r.RequestTypeId == 3)  // 2 = разморозка аккаунта, 3 = разморозка книги
                .ToList()
                .Select(r => new UnfreezeRequestDisplayItem
                {
                    request_obj = r,
                    user_name = r.Accounts?.DisplayName ?? "Неизвестный",
                    target_type = r.TargetBookId != null ? "Книга" : "Пользователь",
                    target_details = GetTargetDetails(r),
                    reason = r.Content ?? ""
                }).ToList();

            dg_requests.ItemsSource = requests;
        }

        private string GetTargetDetails(ServiceRequests request)
        {
            if (request.TargetBookId != null)
            {
                var book = Core.db.LibraryBooks.FirstOrDefault(b => b.Id == request.TargetBookId);
                return book?.Dname ?? "Книга не найдена";
            }
            else
            {
                // Заявка на разморозку пользователя (сам пользователь)
                var user = Core.db.Accounts.FirstOrDefault(u => u.Id == request.UserId);
                return user?.DisplayName ?? "Пользователь не найден";
            }
        }

        private void btn_accept_Click(object sender, RoutedEventArgs e)
        {
            if (dg_requests.SelectedItem is UnfreezeRequestDisplayItem item)
            {
                if (item.request_obj.TargetBookId != null)
                {
                    // Размораживаем книгу
                    var book = Core.db.LibraryBooks.FirstOrDefault(b => b.Id == item.request_obj.TargetBookId);
                    if (book != null) book.IsFrozen = false;
                }
                else
                {
                    // Размораживаем пользователя
                    var user = Core.db.Accounts.FirstOrDefault(u => u.Id == item.request_obj.UserId);
                    if (user != null) user.IsFrozen = false;
                }

                // Удаляем заявку
                Core.db.ServiceRequests.Remove(item.request_obj);
                Core.db.SaveChanges();
                load_data();
                MessageBox.Show("Заявка принята. Цель разморожена.");
            }
        }

        private void btn_reject_Click(object sender, RoutedEventArgs e)
        {
            if (dg_requests.SelectedItem is UnfreezeRequestDisplayItem item)
            {
                // Просто удаляем заявку
                Core.db.ServiceRequests.Remove(item.request_obj);
                Core.db.SaveChanges();
                load_data();
                MessageBox.Show("Заявка отклонена.");
            }
        }
    }

    public class UnfreezeRequestDisplayItem
    {
        // Исправлено: UnfreezeRequest → ServiceRequests
        public ServiceRequests request_obj { get; set; }
        public string user_name { get; set; }
        public string target_type { get; set; }
        public string target_details { get; set; }
        public string reason { get; set; }
    }
}
