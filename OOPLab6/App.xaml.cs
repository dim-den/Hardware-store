using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using OOPLab6.AppSettings;

namespace OOPLab6
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Theme theme;
        private static Language language;
        public static Theme Theme
        {
            get => theme ?? (theme = new Theme());            
        }
        public static Language Language
        {
            get => language ?? (language = new Language());
        }

        public App()
        {
            InitializeComponent();
        }
    }
}
