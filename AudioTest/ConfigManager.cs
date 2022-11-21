using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace AudioTest {
    class ConfigManager {
        

        //public static Dictionary<string,AudioDevice> readDevices() {
        //    Dictionary<string, AudioDevice> dict = new Dictionary<string, AudioDevice>();

        //    XmlSerializer serializer = new XmlSerializer(typeof(BindingList<AudioDevice>));
        //    TextReader writer = new StreamReader("Devices.xml");
        //    BindingList<AudioDevice> devices = (BindingList<AudioDevice>)serializer.Deserialize(writer);

        //    foreach(AudioDevice device in devices) {
        //        dict.Add(device.ID, device);
        //    }

        //    writer.Close();

        //    return dict;
        //}

        public static BindingList<AudioDevice> ReadDevices() {
            BindingList<AudioDevice> devices = new BindingList<AudioDevice>();

            if (File.Exists("Devices.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(BindingList<AudioDevice>));
                TextReader writer = new StreamReader("Devices.xml");
                devices = (BindingList<AudioDevice>)serializer.Deserialize(writer);
                writer.Close();
            }

            return devices;
        }

        public static void saveDevices(BindingList<AudioDevice> devices) {
            XmlSerializer serializer = new XmlSerializer(typeof(BindingList<AudioDevice>));
            TextWriter writer = new StreamWriter("Devices.xml");
            serializer.Serialize(writer, devices);
            writer.Close();
        }
    }
}
