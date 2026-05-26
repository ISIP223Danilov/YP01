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
    /// Логика взаимодействия для AuthorPage.xaml
    /// </summary>
    public partial class AuthorPage : Page
    {
        private LibraryBooks _selected_book;

        public AuthorPage()
        {
            InitializeComponent();
            load_data();
        }

        private void load_data()
        {
            if (Core.current_user == null) return;

            // Исправлено: Book → LibraryBooks, Author → AuthorId
            dg_published_books.ItemsSource = Core.db.LibraryBooks
                .Where(b => b.AuthorId == Core.current_user.Id)
                .ToList();
        }

        private void btn_add_book_Click(object sender, RoutedEventArgs e)
        {
            if (Core.current_user == null) return;

            // Исправлено: Book → LibraryBooks, поля под твою БД
            var new_book = new LibraryBooks
            {
                AuthorId = Core.current_user.Id,
                Dname = "Новая книга",
                Description = "",
                CoveringUri = "",
                Content = "",
                IsFrozen = false
            };

            Core.db.LibraryBooks.Add(new_book);
            Core.db.SaveChanges();

            load_data();
            MessageBox.Show("Новая книга создана.");
        }

        private void dg_books_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Core.db.SaveChanges();
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void dg_books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data_grid = sender as DataGrid;
            if (data_grid?.SelectedItem is LibraryBooks book)
            {
                _selected_book = book;
                // Исправлено: ContentText → Content
                tb_book_content.Text = book.Content ?? "";
            }
        }

        private void btn_save_content_Click(object sender, RoutedEventArgs e)
        {
            if (_selected_book != null)
            {
                // Исправлено: ContentText → Content
                _selected_book.Content = tb_book_content.Text;
                Core.db.SaveChanges();
                MessageBox.Show("Содержимое сохранено.");
            }
            else
            {
                MessageBox.Show("Выберите книгу для сохранения содержимого.");
            }
        }

        private void btn_edit_genres_Click(object sender, RoutedEventArgs e)
        {
            if (_selected_book != null)
            {
                var genre_window = new GenreEditWindow(_selected_book);
                genre_window.ShowDialog();
            }
        }

        private void btn_challenge_freeze_Click(object sender, RoutedEventArgs e)
        {
            if (dg_published_books.SelectedItem is LibraryBooks book)
            {
                if (Core.current_user == null || !book.IsFrozen) return;

                string reason = tb_against.Text;
                if (string.IsNullOrWhiteSpace(reason))
                {
                    MessageBox.Show("Причина не может быть пустой.");
                    return;
                }

                // Исправлено: UnfreezeRequest → ServiceRequests (RequestTypeId = 3 для разморозки книги)
                var request = new ServiceRequests
                {
                    UserId = Core.current_user.Id,
                    TargetBookId = book.Id,
                    RequestTypeId = 3,  // 3 = заявка на разморозку книги
                    Content = reason
                };

                Core.db.ServiceRequests.Add(request);
                Core.db.SaveChanges();

                MessageBox.Show("Заявка на снятие заморозки отправлена.");
            }
        }
    }
}
