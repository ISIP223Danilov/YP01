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
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public class ReviewDisplayItemBook
    {
        public string book_title { get; set; }
        public int rating { get; set; }
        public string text { get; set; }
        public DateTime created_at { get; set; }
    }

    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            load_data();
        }

        private void load_data()
        {
            if (Core.current_user == null)
            {
                MessageBox.Show("Пользователь не авторизован.");
                return;
            }

            tbl_display_name.Text = Core.current_user.DisplayName;
            tbl_login.Text = Core.current_user.Login;
            tbl_email.Text = Core.current_user.Email;

            // Исправлено: Role → UserRoles
            var role = Core.db.UserRoles.FirstOrDefault(r => r.Id == Core.current_user.RoleId);
            tbl_role.Text = role != null ? role.Dname : "Неизвестно";  // Исправлено: Name → Dname

            // Исправлено: Core.current_user.Role → Core.current_user.RoleId
            if (Core.current_user.RoleId == 1)
            {
                sp_author_application.Visibility = Visibility.Visible;
            }

            if (Core.current_user.IsFrozen)
            {
                sp_freeze_warning.Visibility = Visibility.Visible;
            }

            load_reviews();
        }

        private void load_reviews()
        {
            // Исправлено: Review → ReaderReviews, Book1 → LibraryBooks
            var user_reviews = Core.db.ReaderReviews
                .Where(r => r.UserId == Core.current_user.Id)  // r.User → r.UserId, убрал !r.IsFrozen (нет поля)
                .Select(r => new ReviewDisplayItemBook
                {
                    book_title = r.LibraryBooks.Dname,  // r.Book1.Title → r.LibraryBooks.Dname
                    rating = r.Rating,
                    text = r.Content,  // r.Text → r.Content
                    created_at = r.Datetime  // r.CreatedAt → r.Datetime
                })
                .OrderByDescending(r => r.created_at)
                .ToList();

            dg_reviews.ItemsSource = user_reviews;
        }

        private void btn_submit_author_application_Click(object sender, RoutedEventArgs e)
        {
            string reason = tb_author_reason.Text.Trim();
            if (string.IsNullOrEmpty(reason))
            {
                MessageBox.Show("Пожалуйста, введите обоснование для заявки.");
                return;
            }

            // Исправлено: RoleApplication → ServiceRequests (RequestTypeId = 1)
            var application = new ServiceRequests
            {
                UserId = Core.current_user.Id,
                RequestTypeId = 1,  // 1 = заявка на роль автора
                Content = reason
            };

            Core.db.ServiceRequests.Add(application);
            Core.db.SaveChanges();

            MessageBox.Show("Заявка на роль Автора успешно отправлена.");
            tb_author_reason.Text = string.Empty;
        }

        private void btn_submit_unfreeze_application_Click(object sender, RoutedEventArgs e)
        {
            string reason = tb_unfreeze_reason.Text.Trim();
            if (string.IsNullOrEmpty(reason))
            {
                MessageBox.Show("Пожалуйста, введите обоснование для заявки.");
                return;
            }

            // Исправлено: UnfreezeRequest → ServiceRequests (RequestTypeId = 2 для разморозки аккаунта)
            var request = new ServiceRequests
            {
                UserId = Core.current_user.Id,
                RequestTypeId = 2,  // 2 = заявка на разморозку аккаунта
                Content = reason
            };

            Core.db.ServiceRequests.Add(request);
            Core.db.SaveChanges();

            MessageBox.Show("Заявка на оспаривание заморозки успешно отправлена.");
            tb_unfreeze_reason.Text = string.Empty;
        }
    }
}
