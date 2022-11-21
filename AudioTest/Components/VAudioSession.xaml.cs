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
    /// Interaction logic for VAudioSession.xaml
    /// </summary>
    public partial class VAudioSession : UserControl
    {
        public VAudioSession()
        {
            InitializeComponent();
        }

        private void Slider_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            AudioSession session = (AudioSession)DataContext;
            session.invertMute();
            Volume.Foreground = session.TextColor;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            AudioSession session = (AudioSession)DataContext;
            if (session.control != null && session.control.SimpleAudioVolume.Mute) {
                session.TextColor = new SolidColorBrush(Colors.Yellow);
                Volume.Foreground = session.TextColor;
            }
        }
    }
}
