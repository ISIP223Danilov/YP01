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
    /// Логика взаимодействия для BookListsPage.xaml
    /// </summary>
    public partial class BookListsPage : Page
    {
        public BookListsPage()
        {
            InitializeComponent();
            load_data();
        }

        private void load_data()
        {
            // Исправлено: ReadingSection - CollectionTypes, Name - Dname
            var sections = Core.db.CollectionTypes.ToList();
            cbb_current_section.ItemsSource = sections;
            cbb_current_section.DisplayMemberPath = "Dname";
            if (sections.Any()) cbb_current_section.SelectedIndex = 0;

            cbb_target_section.ItemsSource = sections;
            cbb_target_section.DisplayMemberPath = "Dname";
            if (sections.Any()) cbb_target_section.SelectedIndex = 0;

            // Исправлено: Genre - GenreCatalog, Name - Dname
            var genres = Core.db.GenreCatalog.ToList();
            var all_genre = new GenreCatalog { Id = 0, Dname = "Все жанры" };
            genres.Insert(0, all_genre);
            cbb_genre.ItemsSource = genres;
            cbb_genre.DisplayMemberPath = "Dname";
            cbb_genre.SelectedIndex = 0;

            update_books_dg();
        }

        private void cbb_current_section_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_books == null) return;
            update_books_dg();
        }

        private void filter_Changed(object sender, RoutedEventArgs e)
        {
            if (dg_books == null) return;
            update_books_dg();
        }

        private void update_books_dg()
        {
            if (Core.current_user == null || cbb_current_section.SelectedItem == null)
            {
                dg_books.ItemsSource = null;
                return;
            }

            int current_user_id = Core.current_user.Id;
            // Исправлено: ReadingSection - CollectionTypes
            int selected_section_id = ((CollectionTypes)cbb_current_section.SelectedItem).Id;

            // Исправлено: ReadingList - UserBookCollections, rl.User - rl.UserId, rl.Section - rl.CollectionTypeId
            // rl.Book1 - rl.LibraryBooks
            var query = Core.db.UserBookCollections
                .Where(rl => rl.UserId == current_user_id && rl.CollectionTypeId == selected_section_id && !rl.LibraryBooks.IsFrozen)
                .Select(rl => rl.LibraryBooks)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tb_search_title.Text))
            {
                var search_title = tb_search_title.Text.ToLower();
                query = query.Where(b => b.Dname.ToLower().Contains(search_title));  // Title - Dname
            }

            if (!string.IsNullOrWhiteSpace(tb_search_author.Text))
            {
                var search_author = tb_search_author.Text.ToLower();
                query = query.Where(b => b.Accounts.DisplayName.ToLower().Contains(search_author));  // b.User - b.Accounts
            }

            // Исправлено: фильтрация по жанрам через BookGenreMap
            if (cbb_genre.SelectedItem is GenreCatalog selected_genre && selected_genre.Id != 0)
            {
                var bookIdsWithGenre = Core.db.BookGenreMap
                    .Where(bg => bg.GenreId == selected_genre.Id)
                    .Select(bg => bg.BookId)
                    .ToList();
                query = query.Where(b => bookIdsWithGenre.Contains(b.Id));
            }

            var books_list = query.ToList().Select(b => new BookDisplayItem
            {
                book_obj = b,
                title = b.Dname,  // Title - Dname
                author_name = b.Accounts?.DisplayName ?? "Неизвестный автор",  // b.User - b.Accounts
                cover_path = string.IsNullOrWhiteSpace(b.CoveringUri) ? "/Images/placeholder.png" : b.CoveringUri,  // CoverPath → CoveringUri
                average_rating = b.ReaderReviews.Any() ? Math.Round(b.ReaderReviews.Average(r => r.Rating), 1) : 0  // b.Review → b.ReaderReviews
            }).ToList();

            if (cbb_sort.SelectedIndex == 1)
            {
                books_list = books_list.OrderBy(b => b.title).ToList();
            }
            else if (cbb_sort.SelectedIndex == 2)
            {
                books_list = books_list.OrderByDescending(b => b.average_rating).ToList();
            }

            dg_books.ItemsSource = books_list;
        }

        private void btn_open_book_Click(object sender, RoutedEventArgs e)
        {
            if (dg_books.SelectedItem is BookDisplayItem selected_item)
            {
                MainWindow.the_root_frame.Navigate(new BookPage(selected_item.book_obj));
            }
            else
            {
                MessageBox.Show("Выберите книгу для открытия.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_move_to_list_Click(object sender, RoutedEventArgs e)
        {
            if (Core.current_user == null)
            {
                MessageBox.Show("Авторизуйтесь для изменения списков книг.");
                return;
            }

            if (dg_books.SelectedItem is BookDisplayItem selected_item && cbb_target_section.SelectedItem is CollectionTypes selected_section)
            {
                var book = selected_item.book_obj;
                // Исправлено: ReadingList - UserBookCollections, rl.User - rl.UserId, rl.Book - rl.BookId
                var existing_entry = Core.db.UserBookCollections
                    .FirstOrDefault(rl => rl.UserId == Core.current_user.Id && rl.BookId == book.Id);

                if (existing_entry != null)
                {
                    if (existing_entry.CollectionTypeId != selected_section.Id)
                    {
                        existing_entry.CollectionTypeId = selected_section.Id;
                        Core.db.SaveChanges();
                        update_books_dg();
                        MessageBox.Show($"Книга перемещена в список '{selected_section.Dname}'.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу и новый список.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void filter_Changed(object sender, TextChangedEventArgs e)
        {

        }
    }
}
