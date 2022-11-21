using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ControlzEx.Theming;
using AudioTest;

namespace AudioTest
{
    public class AudioSession : INotifyPropertyChanged
    {
        public string ProgrammName { get; set; }
        public int ChannelCount { get; set; }
        private float _Volume;
        public float Volume { get { return _Volume; } set { _Volume = value; setVolume(); } }
        public Brush TextColor { get; set; }

        public float VolumeSlider { get { return (float)Math.Round(Volume * 100,2); } set { Volume = (float)Math.Round(value / 100,2); } }
        public int PID { get; set; }
        public string SID { get; set; }
        public AudioSessionState State { get; set; }

        public AudioSessionControl control;

        public event PropertyChangedEventHandler PropertyChanged;

        public AudioSession() {

        }

        internal void invertMute() {
            control.SimpleAudioVolume.Mute = !control.SimpleAudioVolume.Mute;
            if (control.SimpleAudioVolume.Mute) {
                TextColor = new SolidColorBrush(Colors.Yellow);
            }
            else {
                TextColor = new SolidColorBrush(Colors.White);
            }
        }

        private void setVolume() {
            control.SimpleAudioVolume.MasterVolume = _Volume;
        }
    }
}
