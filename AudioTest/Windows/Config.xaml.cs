using AudioTest;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace AudioTest {
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : MetroWindow, INotifyPropertyChanged {
        public BindingList<AudioDevice> Devices { get; set; } = new BindingList<AudioDevice>();
        public Config() {
            InitializeComponent();
            Devices = ConfigManager.ReadDevices();
            GetAudioDevices();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void GetAudioDevices() {
            BindingList<AudioDevice> devices = new BindingList<AudioDevice>();

            // Create a new MMDeviceEnumerator
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            // Create a MMDeviceCollection of every devices that are enabled
            MMDeviceCollection DeviceCollection = DevEnum.EnumerateAudioEndPoints(EDataFlow.eAll, EDeviceState.DEVICE_STATE_ACTIVE);

            for (int i = 0; i < DeviceCollection.Count; i++) {
                AudioDevice device = new AudioDevice();

                AudioDevice find = Devices.SingleOrDefault(p => p.ID == DeviceCollection[i].ID);
                if (find != null) {
                    device = find;
                }


                device.device = DeviceCollection[i];
                device.Index = i;
                device.ID = DeviceCollection[i].ID;
                device.Name = DeviceCollection[i].FriendlyName;
                if (device.Nickname == null) {
                    device.Nickname = device.Name;
                }

                device.Volume = DeviceCollection[i].AudioEndpointVolume.MasterVolumeLevelScalar;

                // Checks device type
                if (DeviceCollection[i].DataFlow == EDataFlow.eRender) {
                    // Set this object's Type to "Playback"
                    device.Type = "Speaker";
                }
                else if (DeviceCollection[i].DataFlow == EDataFlow.eCapture) {
                    // Set this object's Type to "Recording"
                    device.Type = "Mic";
                }
                devices.Add(device);
            }
            List<AudioDevice> orderedDevices = devices.ToList().OrderBy(x => x.Nickname).ToList();
            devices = new BindingList<AudioDevice>(orderedDevices);
            Devices = devices;
        }

        private async void Save_Click(object sender, System.Windows.RoutedEventArgs e) {
            ConfigManager.saveDevices(Devices);
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            CheckBox box = (CheckBox)sender;
            AudioDevice device = (AudioDevice)box.DataContext;
            device.Hidden = (bool)box.IsChecked;
        }
    }
}
