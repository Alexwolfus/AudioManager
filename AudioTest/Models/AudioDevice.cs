using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using AudioTest;

namespace AudioTest { 
    public class AudioDevice : INotifyPropertyChanged
    {
        public string ID;
        [XmlIgnoreAttribute]
        public MMDevice device;


        [XmlIgnoreAttribute]
        public System.Windows.Threading.Dispatcher dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

        //Windows Properties
        [XmlIgnoreAttribute]
        public int Index { get; set; }
        [XmlIgnoreAttribute]
        public string Name { get; set; }
        [XmlIgnoreAttribute]
        public string Type { get; set; }
        [XmlIgnoreAttribute]
        public bool Mute { get; set; }
        [XmlIgnoreAttribute]
        public bool Active { get { return !Mute; } }
        private float _Volume;
        [XmlIgnoreAttribute]
        public float Volume { get { return _Volume; } set { _Volume = value; setVolume(); } }
        [XmlIgnoreAttribute]
        public float VolumeSlider { get { return (float)Math.Round(Volume * 100, 2); } set { Volume = (float)Math.Round(value / 100, 2); } }
        [XmlIgnoreAttribute]
        public BindingList<AudioSession> Sessions { get; set; }

        //Config Properties
        public string Nickname { get; set; }
        public bool Hidden { get; set; }


    public event PropertyChangedEventHandler PropertyChanged;

        private void setVolume() {
            if (device != null) {
                device.AudioEndpointVolume.MasterVolumeLevelScalar = _Volume; 
            }
        }

    } 
}
