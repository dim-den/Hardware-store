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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Collections.ObjectModel;

namespace OOPLab6
{
    /// <summary>
    /// Логика взаимодействия для SearchUserControl.xaml
    /// </summary>
    public partial class SearchUserControl : UserControl
    {
        ObservableCollection<Device> devices = new ObservableCollection<Device>();
        List<Device> deletedDevices = new List<Device>();

        private Command addCommand;
        private Command saveCommand;
        private Command updateCommand;
        private Command deleteCommand;
        private Command applyCommand;
        public ICommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new Command(obj =>
                  {
                      AddDeviceWindow addDeviceWindow = new AddDeviceWindow();
                      addDeviceWindow.ShowDialog();
                      using (ShopDB db = new ShopDB())
                      {
                          devices = new ObservableCollection<Device>(db.GetDevices());
                          deviceGrid.ItemsSource = devices;
                      }
                  }));
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ??
                  (saveCommand = new Command(obj =>
                  {
                      int updated = 0;

                      using (ShopDB db = new ShopDB())
                      {
                          deletedDevices.ForEach(device => db.DeleteDevice(device));

                          var old = db.GetDevices();
                          if (old.Count == devices.Count)
                          {
                              for (int i = 0; i < devices.Count; i++)
                              {
                                  if (!old[i].Equals(devices[i]))
                                  {
                                      db.UpdateDevice(devices[i].ID, devices[i]);
                                      updated++;
                                  }
                              }
                          }
                      }

                      notifier.ShowInformation($"Было внесено {deletedDevices.Count + updated} изменений в базу данных");
                      deletedDevices.Clear();
                  }));
            }
        }
        public ICommand UpdateCommand
        {
            get
            {
                return updateCommand ??
                  (updateCommand = new Command(obj =>
                  {
                      using (ShopDB db = new ShopDB())
                      {
                          devices = new ObservableCollection<Device>(db.GetDevices());
                          deviceGrid.ItemsSource = devices;
                      }
                      deletedDevices.Clear();
                      notifier.ShowInformation("Таблица обновлена");
                  }));
            }
        }
        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new Command(obj =>
                  {
                      Device selected = deviceGrid.SelectedItem as Device;
                      if (selected != null)
                      {
                          notifier.ShowInformation($"Был удален товар {selected.Name}");
                          devices.Remove(selected);
                          deletedDevices.Add(selected);
                      }
                  }));
            }
        }
        public ICommand ApplyCommand
        {
            get
            {
                return applyCommand ??
                  (applyCommand = new Command(obj =>
                  {
                      List<Device> result = new List<Device>(devices);
                      if (TextBox_Producer.Text != null && TextBox_Producer.Text != "")
                          result = new List<Device>(result.Where(d => d.Producer.Contains(TextBox_Producer.Text)));

                      if (TextBox_Country.Text != null && TextBox_Country.Text != "")
                          result = new List<Device>(result.Where(d => d.Country.Contains(TextBox_Country.Text)));

                      if (TextBox_Name.Text != null && TextBox_Name.Text != "")
                          result = new List<Device>(result.Where(d => d.Name.Contains(TextBox_Name.Text)));

                      if (CheckBox_IsAvailvable.IsChecked == true)
                          result = new List<Device>(result.Where(d => d.Quantity > 0));

                      if (DownPrice.Value != null && UpPrice.Value != null)
                      {
                          if (DownPrice.Value > UpPrice.Value) UpPrice.Value = DownPrice.Value;
                          result = new List<Device>(result.Where(d => d.Price <= UpPrice.Value && d.Price >= DownPrice.Value));
                      }
                      deviceGrid.ItemsSource = result;
                  }));
            }
        }

        private readonly Notifier notifier;
        public SearchUserControl()
        {
            InitializeComponent();

            using (ShopDB db = new ShopDB())
            {
                devices = new ObservableCollection<Device>(db.GetDevices());
                deviceGrid.ItemsSource = devices;
            }

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


    }
}
