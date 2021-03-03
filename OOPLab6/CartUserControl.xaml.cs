using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace OOPLab6
{
    /// <summary>
    /// Логика взаимодействия для CartUserControl.xaml
    /// </summary>
    public partial class CartUserControl : UserControl
    {

        private Command buyCommand;
        public ICommand BuyCommand
        {
            get
            {
                return buyCommand ??
                  (buyCommand = new Command(obj =>
                  {
                      foreach (var device in Devices)
                      {
                          if (device.Quantity > 0)
                          {
                              using (ShopDB db = new ShopDB())
                              {
                                  device.Purhased++;
                                  device.Quantity--;
                                  db.UpdateDevice(device.ID, device);
                                  notifier.ShowInformation("Спасибо за покупку!");
                              }
                          }
                          else
                          {
                              notifier.ShowError($"Покупка невозможна, товар {device.Name} закончился :(");
                          }
                      }
                      Devices.Clear();
                      Text_ItemCount.Text = Text_OverallPrice.Text = "0";
                  }));
            }
        }

        public CartUserControl()
        {
            InitializeComponent();
            deviceList.ItemsSource = Devices;
            Text_ItemCount.Text = Devices.Count.ToString();
            Text_OverallPrice.Text = (Devices.Count == 0 ? "0" : Devices.Sum(d => d.Price).ToString()) + "$";

            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        public static ObservableCollection<Device> Devices = new ObservableCollection<Device>();
        private readonly Notifier notifier;
    }
}
