using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Windows.Input;
using System.Windows.Resources;

namespace OOPLab6
{
    /// <summary>
    /// Логика взаимодействия для MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        private List<Device> Devices { get; set; }
        private readonly Notifier notifier;

        public MainUserControl()
        {
            InitializeComponent();

            using (ShopDB db = new ShopDB())
            {
                Devices = db.GetDevices();
            }
            deviceList.ItemsSource = Devices;

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

        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Device device = (Device)deviceList.SelectedItem;
            CartUserControl.Devices.Add(device);
            notifier.ShowSuccess($"Товар {device.Name} добавлен в корзину!");
        }
    }
}
