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
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();
            load_data();
        }

        private void load_data()
        {
            var genres = Core.db.GenreCatalog.ToList();
            var all_genre = new GenreCatalog { Id = 0, Dname = "Все жанры" };
            genres.Insert(0, all_genre);
            cbb_genre.ItemsSource = genres;
            cbb_genre.DisplayMemberPath = "Dname";
            cbb_genre.SelectedIndex = 0;

            var sections = Core.db.CollectionTypes.ToList();
            cbb_reading_section.ItemsSource = sections;
            cbb_reading_section.DisplayMemberPath = "Dname";
            if (sections.Any()) cbb_reading_section.SelectedIndex = 0;

            update_books_dg();
        }

        private void filter_Changed(object sender, RoutedEventArgs e)
        {
            if (dg_books == null) return;
            update_books_dg();
        }

        private void update_books_dg()
        {
            var query = Core.db.LibraryBooks.Where(b => !b.IsFrozen).AsQueryable();

            if (!string.IsNullOrWhiteSpace(tb_search_title.Text))
            {
                var search_title = tb_search_title.Text.ToLower();
                query = query.Where(b => b.Dname.ToLower().Contains(search_title));
            }

            if (!string.IsNullOrWhiteSpace(tb_search_author.Text))
            {
                var search_author = tb_search_author.Text.ToLower();
                query = query.Where(b => b.Accounts.DisplayName.ToLower().Contains(search_author));
            }

            // ИСПРАВЛЕНО: фильтрация по жанрам через BookGenreMap
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
                title = b.Dname,
                author_name = b.Accounts?.DisplayName ?? "Неизвестный автор",
                cover_path = string.IsNullOrWhiteSpace(b.CoveringUri) ? "/Images/placeholder.png" : b.CoveringUri,
                average_rating = b.ReaderReviews.Any() ? Math.Round(b.ReaderReviews.Average(r => r.Rating), 1) : 0
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

        private void btn_add_to_list_Click(object sender, RoutedEventArgs e)
        {
            if (Core.current_user == null)
            {
                MessageBox.Show("Авторизуйтесь для добавления книг в список.");
                return;
            }

            if (dg_books.SelectedItem is BookDisplayItem selected_item && cbb_reading_section.SelectedItem is CollectionTypes selected_section)
            {
                var book = selected_item.book_obj;
                var existing_entry = Core.db.UserBookCollections
                    .FirstOrDefault(rl => rl.UserId == Core.current_user.Id && rl.BookId == book.Id);

                if (existing_entry != null)
                {
                    if (existing_entry.CollectionTypeId != selected_section.Id)
                    {
                        existing_entry.CollectionTypeId = selected_section.Id;
                        Core.db.SaveChanges();
                        MessageBox.Show($"Книга перемещена в список '{selected_section.Dname}'.");
                    }
                }
                else
                {
                    var new_entry = new UserBookCollections
                    {
                        UserId = Core.current_user.Id,
                        BookId = book.Id,
                        CollectionTypeId = selected_section.Id
                    };
                    Core.db.UserBookCollections.Add(new_entry);
                    Core.db.SaveChanges();
                    MessageBox.Show($"Книга добавлена в список '{selected_section.Dname}'.");
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу и список.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    public class BookDisplayItem
    {
        public LibraryBooks book_obj { get; set; }
        public string title { get; set; }
        public string author_name { get; set; }
        public string cover_path { get; set; }
        public double average_rating { get; set; }
    }
}
