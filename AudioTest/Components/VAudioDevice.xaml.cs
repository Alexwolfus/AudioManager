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

namespace AudioTest
{
    /// <summary>
    /// Interaction logic for VAudioDevice.xaml
    /// </summary>
    public partial class VAudioDevice : UserControl
    {
        public VAudioDevice()
        {
            InitializeComponent();
        }

        private void Slider_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            AudioDevice device = (AudioDevice)DataContext;
            device.Mute = !device.Mute;
        }
    }
}
