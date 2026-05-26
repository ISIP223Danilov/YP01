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
    /// Логика взаимодействия для BookPage.xaml
    /// </summary>
    public partial class BookPage : Page
    {
        private LibraryBooks _current_book;

        public BookPage(LibraryBooks book)
        {
            InitializeComponent();
            _current_book = Core.db.LibraryBooks.FirstOrDefault(b => b.Id == book.Id);
            load_book_info();
            load_reviews();
        }

        private void load_book_info()
        {
            if (_current_book == null) return;

            tbl_title.Text = _current_book.Dname;
            tbl_author.Text = $"Автор: {_current_book.Accounts?.DisplayName ?? "Неизвестный"}";
            tbl_description.Text = _current_book.Description;

            // ИСПРАВЛЕНО: получаем жанры через BookGenreMap
            var bookGenres = Core.db.BookGenreMap.Where(bg => bg.BookId == _current_book.Id).Select(bg => bg.GenreCatalog).ToList();
            if (bookGenres.Any())
            {
                tbl_genres.Text = "Жанры: " + string.Join(", ", bookGenres.Select(g => g.Dname));
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(_current_book.CoveringUri))
                {
                    img_book_cover.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_current_book.CoveringUri, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    img_book_cover.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Images/placeholder.png", UriKind.Absolute));
                }
            }
            catch { }

            if (Core.current_user != null && Core.current_user.UserRoles?.Dname == "Администратор")
            {
                chb_freeze_book.Visibility = Visibility.Visible;
                chb_freeze_book.IsChecked = _current_book.IsFrozen;
                chb_freeze_review.Visibility = Visibility.Visible;
            }
        }

        private void load_reviews()
        {
            if (_current_book == null) return;

            var reviews = Core.db.ReaderReviews
                .Where(r => r.BookId == _current_book.Id)
                .ToList()
                .Select(r => new ReviewDisplayItem
                {
                    review_obj = r,
                    user_name = r.Accounts?.DisplayName ?? "Неизвестный",
                    rating = r.Rating,
                    text = r.Content
                }).ToList();

            dg_reviews.ItemsSource = reviews;
        }

        private void btn_read_Click(object sender, RoutedEventArgs e)
        {
            if (_current_book == null) return;
            var read_window = new ReadBookWindow(_current_book.Content ?? "Текст отсутствует.");
            read_window.Show();
        }

        private void freeze_book_chb_Checked(object sender, RoutedEventArgs e)
        {
            if (_current_book == null || Core.current_user?.UserRoles?.Dname != "Администратор") return;
            _current_book.IsFrozen = true;
            Core.db.SaveChanges();
            MessageBox.Show("Книга заморожена.");
        }

        private void freeze_book_chb_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_current_book == null || Core.current_user?.UserRoles?.Dname != "Администратор") return;
            _current_book.IsFrozen = false;
            Core.db.SaveChanges();
            MessageBox.Show("Книга разморожена.");
        }

        private void btn_submit_complaint_Click(object sender, RoutedEventArgs e)
        {
            if (Core.current_user == null)
            {
                MessageBox.Show("Авторизуйтесь для отправки жалобы.");
                return;
            }

            var reason = tb_complaint.Text.Trim();
            if (string.IsNullOrWhiteSpace(reason))
            {
                MessageBox.Show("Введите причину жалобы.");
                return;
            }

            var new_complaint = new UserComplaints
            {
                UserId = Core.current_user.Id,
                Content = reason
            };

            if (rb_complaint_book.IsChecked == true)
            {
                new_complaint.TargetBookId = _current_book.Id;
            }
            else if (rb_complaint_author.IsChecked == true)
            {
                new_complaint.TargetBookId = _current_book.Id;
            }
            else if (rb_complaint_review.IsChecked == true)
            {
                if (dg_reviews.SelectedItem is ReviewDisplayItem selected_item)
                {
                    new_complaint.TargetReviewId = selected_item.review_obj.Id;
                }
                else
                {
                    MessageBox.Show("Выберите отзыв для жалобы.");
                    return;
                }
            }

            Core.db.UserComplaints.Add(new_complaint);
            try
            {
                Core.db.SaveChanges();
                MessageBox.Show("Жалоба отправлена.");
                tb_complaint.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке жалобы: {ex.Message}");
            }
        }

        private void btn_submit_review_Click(object sender, RoutedEventArgs e)
        {
            if (Core.current_user == null)
            {
                MessageBox.Show("Авторизуйтесь для отправки отзыва.");
                return;
            }

            if (Core.db.ReaderReviews.Any(r => r.UserId == Core.current_user.Id && r.BookId == _current_book.Id))
            {
                MessageBox.Show("У вас уже есть отзыв к этой книге.");
                return;
            }

            var text = tb_review.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Введите текст отзыва.");
                return;
            }

            if (cbb_rating.SelectedItem is ComboBoxItem selected_item && int.TryParse(selected_item.Content.ToString(), out int rating))
            {
                var new_review = new ReaderReviews
                {
                    UserId = Core.current_user.Id,
                    BookId = _current_book.Id,
                    Content = text,
                    Rating = rating,
                    Datetime = DateTime.Now
                };

                Core.db.ReaderReviews.Add(new_review);
                Core.db.SaveChanges();
                MessageBox.Show("Отзыв добавлен.");
                tb_review.Text = "";
                load_reviews();
            }
        }

        private void freeze_review_chb_Checked(object sender, RoutedEventArgs e)
        {
            if (Core.current_user == null || Core.current_user.UserRoles?.Dname != "Администратор") return;
            if (dg_reviews.SelectedItem is ReviewDisplayItem selected_item)
            {
                Core.db.ReaderReviews.Remove(selected_item.review_obj);
                Core.db.SaveChanges();
                MessageBox.Show("Отзыв удалён.");
                load_reviews();
            }
        }

        private void freeze_review_chb_Unchecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Отзыв можно только удалить. Восстановление невозможно.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dg_reviews_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_reviews.SelectedItem is ReviewDisplayItem selected_item)
            {
                chb_freeze_review.Checked -= freeze_review_chb_Checked;
                chb_freeze_review.Unchecked -= freeze_review_chb_Unchecked;
                chb_freeze_review.IsChecked = false;
                chb_freeze_review.Checked += freeze_review_chb_Checked;
                chb_freeze_review.Unchecked += freeze_review_chb_Unchecked;
            }
        }
    }

    public class ReviewDisplayItem
    {
        public ReaderReviews review_obj { get; set; }
        public string user_name { get; set; }
        public int rating { get; set; }
        public string text { get; set; }
    }
}
