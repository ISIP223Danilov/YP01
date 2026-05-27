using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace YP01.Pages
{
    public partial class LeaveReviewPage : Page
    {
        private LibraryBooks _selectedBook;

        public LeaveReviewPage()
        {
            InitializeComponent();
            LoadBooks();
        }

        private void LoadBooks()
        {
            var books = Core.db.LibraryBooks.Where(b => !b.IsFrozen).ToList();
            cbb_books.ItemsSource = books;
            if (books.Any())
            {
                cbb_books.SelectedIndex = -1;
            }
        }

        private void cbb_books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbb_books.SelectedItem is LibraryBooks book)
            {
                _selectedBook = book;

                // ОТКРЫВАЕМ СТРАНИЦУ КНИГИ
                MainPage.the_frame.Navigate(new BookPage(_selectedBook));
            }
        }

        private void btn_submit_review_Click(object sender, RoutedEventArgs e)
        {
            // Этот метод больше не нужен, так как отзыв оставляется на странице книги
            // Но оставим на всякий случай
            if (_selectedBook != null)
            {
                MainPage.the_frame.Navigate(new BookPage(_selectedBook));
            }
        }
    }
}
