using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Globalization;
using System.Windows.Resources;

namespace OOPLab6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GridPrincipal.Children.Add(new MainUserControl());

            App.Language.LanguageChanged += LanguageChanged;
            App.Theme.ThemeChanged += ThemeChanged;

            CultureInfo currLang = App.Language.Name;
            string currTheme = App.Theme.Name;

            //Заполняем меню смены языка:
            menuLanguage.Items.Clear();
            menuTheme.Items.Clear();

            foreach (var lang in App.Language.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                menuLanguage.Items.Add(menuLang);
            }

            foreach (var theme in App.Theme.Themes)
            {
                MenuItem menuThem = new MenuItem();
                menuThem.Header = theme;
                menuThem.Tag = theme;
                menuThem.IsChecked = theme == currTheme;
                menuThem.Click += ChangeThemeClick;
                menuTheme.Items.Add(menuThem);
            }


        }

        private Command closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                return closeCommand ??
                  (closeCommand = new Command(obj =>
                  {
                      this.Close();
                  }));
            }
        }

        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language.Name;

            foreach (MenuItem menuItem in menuLanguage.Items)
            {
                CultureInfo cultureInfo = menuItem.Tag as CultureInfo;
                menuItem.IsChecked = cultureInfo?.Equals(currLang) ?? false;
            }
        }

        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            CultureInfo lang = menuItem?.Tag as CultureInfo;
            App.Language.Name = lang ?? App.Language.Name;
        }

        private void ThemeChanged(Object sender, EventArgs e)
        {
            string currTheme = App.Theme.Name;

            foreach (MenuItem menuItem in menuTheme.Items)
            {
                string theme = menuItem.Tag as string;
                menuItem.IsChecked = theme?.Equals(currTheme) ?? false;
            }
        }

        private void ChangeThemeClick(Object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string theme = menuItem?.Tag as string;
            App.Theme.Name = theme ?? App.Theme.Name;

        }

        private SearchUserControl searchUserControl;
        
        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            ListViewMenu.SelectedItem = null;
            switch (index)
            {
                case 0:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new MainUserControl());
                    break;
                case 1:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new CartUserControl());
                    break;
                case 2:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(searchUserControl ?? ( searchUserControl = new SearchUserControl()));
                    break;
                default:
                    break;
            }
            MoveCursorMenu(index);
        }

        private void MoveCursorMenu(int index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, (52 + (60 * index)), 0, 0);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


    }
}
