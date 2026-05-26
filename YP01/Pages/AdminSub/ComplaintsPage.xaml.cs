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
    /// Логика взаимодействия для ComplaintsPage.xaml
    /// </summary>
    public partial class ComplaintsPage : Page
    {
        public ComplaintsPage()
        {
            InitializeComponent();
            load_data();
        }

        private void load_data()
        {
            // Исправлено: Complaint → UserComplaints
            var complaints = Core.db.UserComplaints.ToList().Select(c => new ComplaintDisplayItem
            {
                complaint_obj = c,
                user_name = c.Accounts?.DisplayName ?? "Неизвестный",
                target_type = c.TargetBookId != null ? "Книга" : c.TargetReviewId != null ? "Отзыв" : "Неизвестно",
                target_details = GetTargetDetails(c),
                reason = c.Content
            }).ToList();

            dg_complaints.ItemsSource = complaints;
        }

        private string GetTargetDetails(UserComplaints complaint)
        {
            if (complaint.TargetBookId != null)
            {
                var book = Core.db.LibraryBooks.FirstOrDefault(b => b.Id == complaint.TargetBookId);
                return book?.Dname ?? "Книга не найдена";
            }
            else if (complaint.TargetReviewId != null)
            {
                var review = Core.db.ReaderReviews.FirstOrDefault(r => r.Id == complaint.TargetReviewId);
                return review?.Content ?? "Отзыв не найден";
            }
            return "Неизвестно";
        }

        private void btn_accept_Click(object sender, RoutedEventArgs e)
        {
            if (dg_complaints.SelectedItem is ComplaintDisplayItem item)
            {
                // Замораживаем цель жалобы
                if (item.complaint_obj.TargetBookId != null)
                {
                    var book = Core.db.LibraryBooks.FirstOrDefault(b => b.Id == item.complaint_obj.TargetBookId);
                    if (book != null) book.IsFrozen = true;
                }
                else if (item.complaint_obj.TargetReviewId != null)
                {
                    // В твоей БД у отзывов нет IsFrozen, поэтому удаляем отзыв
                    var review = Core.db.ReaderReviews.FirstOrDefault(r => r.Id == item.complaint_obj.TargetReviewId);
                    if (review != null) Core.db.ReaderReviews.Remove(review);
                }

                // Удаляем жалобу
                Core.db.UserComplaints.Remove(item.complaint_obj);
                Core.db.SaveChanges();
                load_data();
                MessageBox.Show("Жалоба принята. Цель была заморожена/удалена.");
            }
        }

        private void btn_reject_Click(object sender, RoutedEventArgs e)
        {
            if (dg_complaints.SelectedItem is ComplaintDisplayItem item)
            {
                // Просто удаляем жалобу
                Core.db.UserComplaints.Remove(item.complaint_obj);
                Core.db.SaveChanges();
                load_data();
                MessageBox.Show("Жалоба отклонена.");
            }
        }
    }

    public class ComplaintDisplayItem
    {
        public UserComplaints complaint_obj { get; set; }
        public string user_name { get; set; }
        public string target_type { get; set; }
        public string target_details { get; set; }
        public string reason { get; set; }
    }
}
