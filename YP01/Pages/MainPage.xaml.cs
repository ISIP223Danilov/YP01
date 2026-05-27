using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace YP01.Pages
{
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

            // ТВОЯ ЛОКАЛЬНАЯ КАРТИНКА (лежит в папке Images)
            var default_image = new Uri("pack://application:,,,/image/popa.png", UriKind.Absolute);

            items.Add(new SidebarItem { title = "Каталог книг", page_uri = new Uri("Pages/CatalogPage.xaml", UriKind.Relative), image_uri = default_image });
            items.Add(new SidebarItem { title = "Списки книг", page_uri = new Uri("Pages/BookListsPage.xaml", UriKind.Relative), image_uri = default_image });
            items.Add(new SidebarItem { title = "Оставить отзыв", page_uri = new Uri("Pages/LeaveReviewPage.xaml", UriKind.Relative), image_uri = default_image });

            if (Core.current_user != null)
            {
                if (Core.current_user.RoleId == 3)
                {
                    items.Add(new SidebarItem { title = "Администрирование", page_uri = new Uri("Pages/AdminPage.xaml", UriKind.Relative), image_uri = default_image });
                }

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