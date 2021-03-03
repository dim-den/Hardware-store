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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace OOPLab6
{
    /// <summary>
    /// Логика взаимодействия для AddDeviceWindow.xaml
    /// </summary>
    public partial class AddDeviceWindow : Window
    {
        private readonly Notifier notifier;

        private Command addCommand;
        private Command closeCommand;
        public ICommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new Command(obj =>
                  {
                      try
                      {
                          Device device = new Device();
                          device.Name = TextBox_Name.Text;
                          device.ImagePath = TextBox_ImagePath.Text;
                          device.Description = TextBox_Description.Text;
                          device.Producer = TextBox_Producer.Text;
                          device.Country = TextBox_Country.Text;
                          device.Quantity = Convert.ToInt32(TextBox_Quantity.Text);
                          device.Purhased = Convert.ToInt32(TextBox_Purchased.Text);
                          device.Price = Convert.ToInt32(TextBox_Price.Text);

                          if (device.HaveEmptyFields()) throw new Exception();

                          using (ShopDB db = new ShopDB())
                          {
                              if (db.InsertDevice(device)) notifier.ShowSuccess("Товар был добавлен в базу данных!");
                              else notifier.ShowError("Ошибка добавления в базу данных");
                              this.Close();
                          }
                      }
                      catch
                      {
                          notifier.ShowError("Введены некорретные данные!");
                      }
                  }));
            }
        }
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
        public AddDeviceWindow()
        {
            InitializeComponent();

            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 5,
                    offsetY: 5);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        private void Grid_MousDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

    }
}
