using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using CUE.NET;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Exceptions;
using CUE.NET.Devices.Generic.Enums;

namespace LedProjectWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AsusVgaAura.AsusVgaAuraWrapper graphicsCard = new AsusVgaAura.AsusVgaAuraWrapper();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                CueSDK.Initialize();
                Debug.WriteLine("Initialized with " + CueSDK.LoadedArchitecture + "-SDK");

                CorsairKeyboard keyboard = CueSDK.KeyboardSDK;
                if (keyboard == null)
                    throw new WrapperException("No Keyboard Found");
            }
            catch (CUEException ex)
            {
                Debug.WriteLine("CUE Exception! ErrorCode: " + Enum.GetName(typeof(CorsairError), ex.Error));
            }
            catch (WrapperException ex)
            {
                Debug.WriteLine("Wrapper Exception! Message: " + ex.Message);
            }
        }

        private void LightsOffButton_Click(object sender, RoutedEventArgs e)
        {
            graphicsCard.DisableLights();
        }

        private void FadeColorsButton_Click(object sender, RoutedEventArgs e)
        {
            graphicsCard.SetCrossFade();
        }

        private void SyncWithCorsairKeyboard_Click(object sender, RoutedEventArgs e)
        {
            graphicsCard.DisableLights();
            CorsairKeyboard keyboard = CueSDK.KeyboardSDK;
            Debug.WriteLine("Keyboard Found: " + CueSDK.IsSDKAvailable(CorsairDeviceType.Keyboard));
            int r = keyboard['A'].Color.R;
            graphicsCard.SetColor(r, g, b);
        }
    }
}
