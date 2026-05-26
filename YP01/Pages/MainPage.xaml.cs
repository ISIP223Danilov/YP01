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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public static Frame the_frame;

        public class SidebarItem
        {
            public string title { get; set; }
            public Uri page_uri { get; set; }
            public Uri image_uri { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
            the_frame = this.fr_the_main;

            var items = new List<SidebarItem>();

            var default_image = new Uri("https://coinsbolhov.ru/upload/cache_images/registered/28/23/282320/282320_1073_1073.jpg");

            items.Add(new SidebarItem { title = "Каталог книг", page_uri = new Uri("Pages/CatalogPage.xaml", UriKind.Relative), image_uri = default_image });
            items.Add(new SidebarItem { title = "Списки книг", page_uri = new Uri("Pages/BookListsPage.xaml", UriKind.Relative), image_uri = default_image });

            if (Core.current_user != null)
            {
                // ИСПРАВЛЕНО: Role → RoleId (3 = Администратор)
                if (Core.current_user.RoleId == 3)
                {
                    items.Add(new SidebarItem { title = "Администрирование", page_uri = new Uri("Pages/AdminPage.xaml", UriKind.Relative), image_uri = default_image });
                }

                // ИСПРАВЛЕНО: Role → RoleId (2 = Автор)
                if (Core.current_user.RoleId == 2)
                {
                    items.Add(new SidebarItem { title = "Страница автора", page_uri = new Uri("Pages/AuthorPage.xaml", UriKind.Relative), image_uri = default_image });
                }

                if (Core.current_user.IsFrozen)
                {
                    items.Add(new SidebarItem { title = "Предупреждение о заморозке аккаунта", page_uri = new Uri("Pages/FreezeWarningPage.xaml", UriKind.Relative), image_uri = default_image });
                }

                items.Add(new SidebarItem { title = "Профиль", page_uri = new Uri("Pages/ProfilePage.xaml", UriKind.Relative), image_uri = default_image });
            }

            sidebar_items_control.ItemsSource = items;
            the_frame.Navigate(new Uri("Pages/CatalogPage.xaml", UriKind.Relative));
        }

        private void sidebar_btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Uri uri)
            {
                the_frame.Navigate(uri);
            }
        }
    }
}
