using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AudioTest {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged {
        public BindingList<AudioDevice> Devices { get; set; } = new BindingList<AudioDevice>();
        public BindingList<AudioDevice> ConfiguredDevices { get; set; } = new BindingList<AudioDevice>();
        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindow() {
            InitializeComponent();
            DataContext = this;

            initTimer();
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public BindingList<AudioDevice> GetAudioDevices() {
            BindingList<AudioDevice> devices = new BindingList<AudioDevice>();

            // Create a new MMDeviceEnumerator
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            // Create a MMDeviceCollection of every devices that are enabled
            MMDeviceCollection DeviceCollection = DevEnum.EnumerateAudioEndPoints(EDataFlow.eAll, EDeviceState.DEVICE_STATE_ACTIVE);

            for (int i = 0; i < DeviceCollection.Count; i++) {
                AudioDevice device = new AudioDevice();

                AudioDevice find = ConfiguredDevices.SingleOrDefault(p => p.ID == DeviceCollection[i].ID);
                if (find != null) {
                    device = find;

                    if (find.Hidden) {
                        continue;
                    }
                    else {
                        device.Nickname = find.Nickname;
                    }
                }
                //find = ConfiguredDevices.SingleOrDefault(p => p.ID == DeviceCollection[i].ID);
                //if (find.Hidden) {
                //    continue;
                //}
                //else {
                //    device.Nickname = find.Nickname;
                //}

                device.device = DeviceCollection[i];
                device.Sessions = new BindingList<AudioSession>();
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

                //device.Volume = DeviceCollection[i].AudioEndpointVolume.MasterVolumeLevel = 0.1F;
                device.Mute = DeviceCollection[i].AudioEndpointVolume.Mute;

                var Sessions = DeviceCollection[i].AudioSessionManager.Sessions;

                for (int x = 0; x < Sessions.Count; x++) {
                    AudioSession session = new AudioSession();
                    session.control = Sessions[x];
                    session.PID = (int)Sessions[x].ProcessID;
                    if (session.PID == 0) {
                        session.ProgrammName = "Systemsounds";
                    }
                    else {
                        session.ProgrammName = Process.GetProcessById((int)Sessions[x].ProcessID).ProcessName;

                        if (Process.GetProcessById((int)Sessions[x].ProcessID).MainWindowTitle != "") {
                            session.ProgrammName = Process.GetProcessById((int)Sessions[x].ProcessID).MainWindowTitle;
                        }
                    }

                    session.SID = Sessions[x].SessionInstanceIdentifier;
                    session.Volume = Sessions[x].SimpleAudioVolume.MasterVolume;
                    session.State = Sessions[x].State;
                    session.ChannelCount = Sessions[x].AudioMeterInformation.PeakValues.Count;

                    new Thread(() => {
                        if (device.dispatcher.CheckAccess()) {
                            device.Sessions.Add(session);
                        }
                        else {
                            device.dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new Action(delegate {
                                        device.Sessions.Add(session);
                                    }
                           ));
                        }
                    }).Start();

                    List<AudioSession> orderedSessions = device.Sessions.ToList().OrderBy(x => x.ProgrammName).ToList();
                    device.Sessions = new BindingList<AudioSession>(orderedSessions);
                }
                devices.Add(device);
            }
            List<AudioDevice> orderedDevices = devices.ToList().OrderBy(x => x.Nickname).ToList();
            devices = new BindingList<AudioDevice>(orderedDevices);
            return devices;
        }

        private BindingList<AudioDevice> refreshAudioDevices(List<AudioDevice> devices) {
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
                else {
                    find = ConfiguredDevices.SingleOrDefault(p => p.ID == DeviceCollection[i].ID);
                    if (find != null) {
                        device = find;

                        if (find.Hidden) {
                            continue;
                        }
                        else {
                            device.Nickname = find.Nickname;
                        }
                    }

                    device.device = DeviceCollection[i];
                    device.Sessions = new BindingList<AudioSession>();
                    device.Index = i;
                    device.ID = DeviceCollection[i].ID;
                    device.Name = DeviceCollection[i].FriendlyName;
                    if (device.Nickname == null) {
                        device.Nickname = device.Name;
                    }
                }

                device.Volume = DeviceCollection[i].AudioEndpointVolume.MasterVolumeLevelScalar;
                device.Mute = DeviceCollection[i].AudioEndpointVolume.Mute;

                var Sessions = DeviceCollection[i].AudioSessionManager.Sessions;

                for (int x = 0; x < Sessions.Count; x++) {
                    AudioSession findSession = device.Sessions.SingleOrDefault(p => p.SID == Sessions[x].SessionInstanceIdentifier);
                    if (findSession != null) {
                        findSession.Volume = Sessions[x].SimpleAudioVolume.MasterVolume;
                        findSession.State = Sessions[x].State;
                        findSession.ChannelCount = Sessions[x].AudioMeterInformation.PeakValues.Count;
                        continue;
                    }

                    AudioSession session = new AudioSession();
                    session.control = Sessions[x];
                    session.PID = (int)Sessions[x].ProcessID;
                    if (session.PID == 0) {
                        session.ProgrammName = "Systemsounds";
                    }
                    else {
                        session.ProgrammName = Process.GetProcessById((int)Sessions[x].ProcessID).ProcessName;

                        if (Process.GetProcessById((int)Sessions[x].ProcessID).MainWindowTitle != "") {
                            session.ProgrammName = Process.GetProcessById((int)Sessions[x].ProcessID).MainWindowTitle;
                        }
                    }

                    session.SID = Sessions[x].SessionInstanceIdentifier;

                    session.Volume = Sessions[x].SimpleAudioVolume.MasterVolume;
                    session.State = Sessions[x].State;
                    session.ChannelCount = Sessions[x].AudioMeterInformation.PeakValues.Count;

                    new Thread(() => {
                        if (device.dispatcher.CheckAccess()) {
                            device.Sessions.Add(session);
                        }
                        else {
                            device.dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new Action(delegate {
                                        device.Sessions.Add(session);
                                    }
                           ));
                        }
                    }).Start();

                    List<AudioSession> orderedSessions = device.Sessions.ToList().OrderBy(x => x.ProgrammName).ToList();
                    device.Sessions = new BindingList<AudioSession>(orderedSessions);
                    orderedSessions.Clear();
                }

                BindingList<AudioSession> audioSessions = new BindingList<AudioSession>();
                foreach (AudioSession curSession in device.Sessions) {
                    if (curSession.State != AudioSessionState.AudioSessionStateExpired) {
                        audioSessions.Add(curSession);
                    }
                }
                device.Sessions = audioSessions;

                if (find == null) {
                    device.dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                new Action(delegate {
                                    devices.Add(device);
                                }
                            ));
                }
            }
            List<AudioDevice> orderedDevices = devices.ToList().OrderBy(x => x.Type).OrderBy(x => x.Nickname).ToList();
            return new BindingList<AudioDevice>(orderedDevices);
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e) {
            ConfiguredDevices = ConfigManager.ReadDevices();
            Devices = GetAudioDevices();
        }

        private void Settings_Click(object sender, RoutedEventArgs e) {
            Config window = new Config();
            window.ShowDialog();
        }

        private void initTimer() {
            timer.Tick += Timer_Tick;
            timer.Interval = new System.TimeSpan(0, 0, 0, 1, 0);
            timer.Start();
        }

        private async void Timer_Tick(object sender, System.EventArgs e) {
            //Devices.Clear();
            Devices = await Task.Run(() => refreshAudioDevices(Devices.ToList()));
        }

        private void MetroWindow_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            timer.Stop();
        }

        private void MetroWindow_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            timer.Start();
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e) {
            if (this.WindowState == WindowState.Minimized) {
                timer.Stop();
            }
            else {
                timer.Start();
            }
        }
    }
}