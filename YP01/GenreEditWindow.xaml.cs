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
using System.Windows.Shapes;

namespace YP01
{
    /// <summary>
    /// Логика взаимодействия для GenreEditWindow.xaml
    /// </summary>
    public partial class GenreEditWindow : Window
    {
        private LibraryBooks _book;
        private List<GenreSelectionItem> _genres;

        public GenreEditWindow(LibraryBooks book)
        {
            InitializeComponent();
            _book = book;
            load_genres();
        }

        private void load_genres()
        {
            var all_genres = Core.db.GenreCatalog.ToList();

            // Получаем ID жанров книги через отдельный запрос
            var book_genres_ids = Core.db.BookGenreMap
                .Where(bg => bg.BookId == _book.Id)
                .Select(bg => bg.GenreId)
                .ToList();

            _genres = all_genres.Select(g => new GenreSelectionItem
            {
                id = g.Id,
                name = g.Dname,
                is_selected = book_genres_ids.Contains(g.Id)
            }).ToList();

            genres_items_control.ItemsSource = _genres;
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            // Удаляем старые связи
            var oldGenres = Core.db.BookGenreMap.Where(bg => bg.BookId == _book.Id).ToList();
            foreach (var bg in oldGenres)
            {
                Core.db.BookGenreMap.Remove(bg);
            }

            // Добавляем новые связи
            var selectedIds = _genres.Where(g => g.is_selected).Select(g => g.id).ToList();
            foreach (var genreId in selectedIds)
            {
                var newBookGenre = new BookGenreMap
                {
                    BookId = _book.Id,
                    GenreId = genreId
                };
                Core.db.BookGenreMap.Add(newBookGenre);
            }

            Core.db.SaveChanges();
            MessageBox.Show("Жанры сохранены.");
            this.Close();
        }
    }

    public class GenreSelectionItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool is_selected { get; set; }
    }
}
