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
using OOPLab6.Commands;

namespace OOPLab6
{
    /// <summary>
    /// Логика взаимодействия для SearchUserControl.xaml
    /// </summary>
    public partial class SearchUserControl : UserControl
    {
        public List<Device> deletedDevices = new List<Device>();

        #region Commands

        public static UndoCommandManager undoCommandManager = new UndoCommandManager();

        private UndoCommand addCommand;
        private UndoCommand deleteCommand;
        private Command saveCommand;
        private Command updateCommand;
        private Command applyCommand;
        private Command undoCommand;
        private Command redoCommand;
        public ICommand AddCommand
        {
            get
            {
                return addCommand ??
                      (addCommand = new UndoCommand(
                      obj =>
                      {
                          AddDeviceWindow addDeviceWindow = new AddDeviceWindow();
                          addDeviceWindow.ShowDialog();
                          using (ShopDB db = new ShopDB())
                          {
                              var res = db.GetDevices();

                              if (Devices.Count != res.Count)
                              {
                                  Devices = new ObservableCollection<Device>(res);
                                  return Devices.Last();
                              }
                          }
                          return null;
                      },
                      obj =>
                      {
                          Device selected = obj as Device;
                          if (selected != null)
                          {
                              notifier.ShowInformation($"Был удален товар {selected.Name}");
                              Devices.Remove(selected);
                              deletedDevices.Add(selected);
                              SaveCommand.Execute(null);
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
                          if (old.Count == Devices.Count)
                          {
                              for (int i = 0; i < Devices.Count; i++)
                              {
                                  if (!old[i].Equals(Devices[i]))
                                  {
                                      db.UpdateDevice(Devices[i].ID, Devices[i]);
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
                          Devices = new ObservableCollection<Device>(db.GetDevices());
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
                  (deleteCommand = new UndoCommand(obj =>
                  {
                      Device selected = obj as Device ?? deviceGrid.SelectedItem as Device;
                      if (selected != null)
                      {
                          notifier.ShowInformation($"Был удален товар {selected.Name}");
                          MessageBox.Show(selected.ToString());
                          MessageBox.Show(Devices.Remove(selected).ToString());
                     
                          deletedDevices.Add(selected);
                      }
                      return selected;
                  },
                  obj =>
                  {
                      Device selected = obj as Device;
                      if (selected != null)
                      {
                          notifier.ShowInformation($"Был добавлен товар {selected.Name}");
                          Devices.Add(selected);
                          deletedDevices.Remove(selected);
                      }
                  }
                ));
            }
        }
        public ICommand ApplyCommand
        {
            get
            {
                return applyCommand ??
                  (applyCommand = new Command(obj =>
                  {
                      using (ShopDB db = new ShopDB())
                      {
                          Devices = new ObservableCollection<Device>(db.GetDevices());
                      }

                      var result = new List<Device>(Devices.Where(d => d.ProducerContains(Producer) && d.CountryContains(Country) && d.NameContains(ProductName)));

                      if (CheckBox_IsAvailvable.IsChecked == true)
                          result = new List<Device>(result.Where(d => d.Quantity > 0));

                      if (DownPrice.Value != null && UpPrice.Value != null)
                      {
                          if (DownPrice.Value > UpPrice.Value) UpPrice.Value = DownPrice.Value;
                          result = new List<Device>(result.Where(d => d.Price <= UpPrice.Value && d.Price >= DownPrice.Value));
                      }

                      Devices = new ObservableCollection<Device>(result);
                  }));
            }
        }

        public ICommand UndoCommand
        {
            get
            {
                return undoCommand ??
                  (undoCommand = new Command(obj =>
                  {
                      undoCommandManager.Undo();
                  }));
            }
        }

        public ICommand RedoCommand
        {
            get
            {
                return redoCommand ??
                  (redoCommand = new Command(obj =>
                  {
                      undoCommandManager.Redo();
                  }));
            }
        }
        #endregion

        #region DependecyProperties

        public static readonly DependencyProperty FormProducerProperty = DependencyProperty.Register(
        "Producer",
        typeof(string),
        typeof(SearchUserControl),
        new PropertyMetadata("") { CoerceValueCallback = new CoerceValueCallback(CoerceStringInput) },
        new ValidateValueCallback(IsValidStringInput)
        );

        public static readonly DependencyProperty FormCountryProperty = DependencyProperty.Register(
        "Country",
        typeof(string),
        typeof(SearchUserControl),
        new PropertyMetadata("") { CoerceValueCallback = new CoerceValueCallback(CoerceStringInput) },
        new ValidateValueCallback(IsValidStringInput)
        );

        public static readonly DependencyProperty FormProductNameProperty = DependencyProperty.Register(
        "ProductName",
        typeof(string),
        typeof(SearchUserControl),
        new PropertyMetadata("") { CoerceValueCallback = new CoerceValueCallback(CoerceStringInput) },
        new ValidateValueCallback(IsValidStringInput)
        );

        public static readonly DependencyProperty DevicesProperty = DependencyProperty.Register("Devices", typeof(ObservableCollection<Device>), 
            typeof(SearchUserControl), new PropertyMetadata());
        public ObservableCollection<Device> Devices
        {
            get { return (ObservableCollection<Device>)GetValue(DevicesProperty); }
            set { SetValue(DevicesProperty, value); }
        }
        public string Producer
        {
            get { return (string)GetValue(FormProducerProperty); }
            set { SetValue(FormProducerProperty, value); }
        }
        public string Country
        {
            get { return (string)GetValue(FormCountryProperty); }
            set { SetValue(FormCountryProperty, value); }
        }
        public string ProductName
        {
            get { return (string)GetValue(FormProductNameProperty); }
            set { SetValue(FormProductNameProperty, value); }
        }
        public static bool IsValidStringInput(object value)
        {
            return ((string)value).All(c => Char.IsLetter(c));
        }

        private static object CoerceStringInput(DependencyObject d, object value)
        {
            var inp = (string)value;
            if (inp.Length > 8)
                inp = inp.Substring(0, 8);
            return inp;
        }

        #endregion

        private readonly Notifier notifier;

        public SearchUserControl()
        {
            InitializeComponent();

            using (ShopDB db = new ShopDB())
            {
                Devices = new ObservableCollection<Device>(db.GetDevices());
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
