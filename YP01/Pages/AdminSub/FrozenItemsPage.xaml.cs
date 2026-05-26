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
    /// Логика взаимодействия для FrozenItemsPage.xaml
    /// </summary>
    public partial class FrozenItemsPage : Page
    {
        public FrozenItemsPage()
        {
            InitializeComponent();
            load_data();
        }

        private void load_data()
        {
            var frozen_items = new List<FrozenItemDisplay>();

            // Исправлено: Book → LibraryBooks, Title → Dname, User → Accounts, ContentText → Content
            var books = Core.db.LibraryBooks.Where(b => b.IsFrozen).ToList();
            foreach (var b in books)
            {
                frozen_items.Add(new FrozenItemDisplay
                {
                    source_obj = b,
                    item_type = "Книга",
                    title = b.Dname,
                    author = b.Accounts?.DisplayName,
                    description = b.Description,
                    content = b.Content,
                    is_frozen = true
                });
            }

            
          
            /*
            var reviews = Core.db.ReaderReviews.Where(r => r.IsFrozen).ToList();
            foreach (var r in reviews) 
            {
                frozen_items.Add(new FrozenItemDisplay 
                {
                    source_obj = r,
                    item_type = "Отзыв",
                    title = "",
                    author = r.Accounts?.DisplayName,
                    description = "",
                    content = r.Content,
                    is_frozen = true
                });
            }
            */

            // Исправлено: User → Accounts
            var users = Core.db.Accounts.Where(u => u.IsFrozen).ToList();
            foreach (var u in users)
            {
                frozen_items.Add(new FrozenItemDisplay
                {
                    source_obj = u,
                    item_type = "Пользователь",
                    title = "",
                    author = u.DisplayName,
                    description = "",
                    content = "",
                    is_frozen = true
                });
            }

            dg_frozen_items.ItemsSource = frozen_items;
        }

        private void freeze_chb_Checked(object sender, RoutedEventArgs e)
        {
            update_freeze_status(sender, true);
        }

        private void freeze_chb_Unchecked(object sender, RoutedEventArgs e)
        {
            update_freeze_status(sender, false);
        }

        private void update_freeze_status(object sender, bool is_frozen)
        {
            if ((sender as CheckBox)?.DataContext is FrozenItemDisplay item)
            {
                // Исправлено: Book → LibraryBooks
                if (item.source_obj is LibraryBooks book)
                {
                    book.IsFrozen = is_frozen;
                }
                // В твоей БД у отзывов нет IsFrozen
                // else if (item.source_obj is ReaderReviews review) 
                // {
                //     review.IsFrozen = is_frozen;
                // }
                else if (item.source_obj is Accounts user)
                {
                    user.IsFrozen = is_frozen;
                }

                Core.db.SaveChanges();
                load_data(); // Обновляем список
            }
        }
    }

    public class FrozenItemDisplay
    {
        public object source_obj { get; set; }
        public string item_type { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string content { get; set; }
        public bool is_frozen { get; set; }
    }
}
