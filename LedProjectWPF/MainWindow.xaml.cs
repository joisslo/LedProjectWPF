using CUE.NET;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Exceptions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LedProjectWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AsusVgaAura.AsusVgaAuraWrapper graphicsCard = new AsusVgaAura.AsusVgaAuraWrapper();
        private CorsairKeyboard keyboard;
        public int MILLI_DELAY = 1;
        public bool killFade = true;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                CueSDK.Initialize();
                CueSDK.UpdateMode = UpdateMode.Continuous; //Set the keyboard to update itself every33.3 milliseconds;
                Debug.WriteLine("Initialized with " + CueSDK.LoadedArchitecture + "-SDK");
                keyboard = CueSDK.KeyboardSDK;
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

        private async Task KillTheLights()
        {
            killFade = true;
            await PutTaskDelay(60);
            Debug.WriteLine("Turning Lights Off.");
            graphicsCard.DisableLights();
        }

        private async void LightsOffButton_Click(object sender, RoutedEventArgs e)
        {
            await KillTheLights();
        }

        private void FadeColorsButton_Click(object sender, RoutedEventArgs e)
        {
            killFade = true;
            graphicsCard.SetCrossFade();
        }

        //Async Method so the program doesn't freeze up while continually
        //updating the graphics Card
        private async Task PutTaskDelay()
        {
            await Task.Delay(MILLI_DELAY);
        }

        private async Task PutTaskDelay(int delayMillis)
        {
            await Task.Delay(delayMillis);
        }

        //Async Method continued from above
        //Breathing effect, only in red for right now
        private async void RedFade_Click(object sender, RoutedEventArgs e)
        {
            await KillTheLights();
            killFade = false;
            byte red = 0;
            while (!killFade)
            {
                while (red < 255 && !killFade)
                {
                    graphicsCard.SetColor(red += 1, 0, 0);
                    Log("Going Up. red = " + red);
                    await PutTaskDelay();
                }
                while (red > 0 && !killFade)
                {
                    graphicsCard.SetColor(red -= 1, 0, 0);
                    Log("Going Down. red = " + red);
                    await PutTaskDelay();
                }
            }
        }

        //Async method continued from above
        //Custom rainbow spectrum fading
        private async void CustomRainbowButton_Click(object sender, RoutedEventArgs e)
        {
            await KillTheLights();
            killFade = false;
            byte red = 255, green = 0, blue = 0;
            while (!killFade)
            {
                if (red > 0 && blue == 0)
                {
                    red--;
                    green++;
                }
                if (green > 0 && red == 0)
                {
                    green--;
                    blue++;
                }
                if (blue > 0 && green == 0)
                {
                    blue--;
                    red++;
                }
                graphicsCard.SetColor(red, green, blue);
                Log("(R, G, B) = (" + red + ", " + green + ", " + blue + ")");
                await PutTaskDelay();
            }
        }

        //Async method continued from above
        //Custom rainbow spectrum sync with keyboard
        private async void CustomRainbowSync_Click(object sender, RoutedEventArgs e)
        {
            //CustomRainbowButton_Click(sender, e);
            await KillTheLights();
            killFade = false;
            byte red = 255, green = 0, blue = 0;
            while (!killFade)
            {
                if (red > 0 && blue == 0)
                {
                    red--;
                    green++;
                }
                if (green > 0 && red == 0)
                {
                    green--;
                    blue++;
                }
                if (blue > 0 && green == 0)
                {
                    blue--;
                    red++;
                }

                Log("(R, G, B) = (" + red + ", " + green + ", " + blue + ")");
                //TODO: Keyboard Stuff
                CUE.NET.Devices.Generic.CorsairColor c = new CUE.NET.Devices.Generic.CorsairColor(red, green, blue);
                //keyboard['A'].Color = c;
                //CUE.NET.Groups.RectangleLedGroup leds = new CUE.NET.Groups.RectangleLedGroup(keyboard, CorsairLedId.Escape, CorsairLedId.KeypadEnter);
                //keyboard.AttachLedGroup(leds);
                CUE.NET.Brushes.SolidColorBrush color = new CUE.NET.Brushes.SolidColorBrush(c);
                keyboard.Brush = color;
                graphicsCard.SetColor(red, green, blue);
                //keyboard.Update();
                await PutTaskDelay();
            }
        }

        //Async Method continued from above
        private async void SyncWithCorsairKeyboard_Click(object sender, RoutedEventArgs e)
        {
            graphicsCard.DisableLights();
            Debug.WriteLine("Keyboard Found: " + CueSDK.IsSDKAvailable(CorsairDeviceType.Keyboard));
            killFade = false;
            while (!killFade)
            {
                byte r = keyboard['A'].Color.R;
                byte g = keyboard['A'].Color.G;
                byte b = keyboard['A'].Color.B;
                graphicsCard.SetColor(r, g, b);
                await PutTaskDelay();
                Debug.WriteLine("Color Values: (" + r + ", " + g + ", " + b + ")");
            }
        } //End async methods

        //Static color selection with a combobox
        private void Color_Choice_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            killFade = true;
            string selected = this.Color_Choice_ComboBox.SelectedValue.ToString();
            if (selected == "Off")
            {
                graphicsCard.DisableLights();
            }
            else if (selected == "Red")
            {
                graphicsCard.SetColor(255, 0, 0);
            }
            else if (selected == "Green")
            {
                graphicsCard.SetColor(0, 255, 0);
            }
            else if (selected == "Blue")
            {
                graphicsCard.SetColor(0, 0, 255);
            }
        }

        private static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + ": " + message);
        }

        private static void OnTimerElapsed(object state)
        {
            Log("Timer Elapsed.");
        }

        private void RainowDelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MILLI_DELAY = (int)e.NewValue;
            Log("Delay changed to " + MILLI_DELAY);
        }

        //private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    MILLI_DELAY = (int)RedFadeTimer.Value;
        //}

        //private async void Audio_Click(object sender, RoutedEventArgs e)
        //{
        //    AudioNodeListener listener = new AudioNodeListener();
        //    killFade = false;
        //    while (!killFade)
        //    {
        //        byte r = (byte)listener.DopplerVelocity.X;
        //        byte g = (byte)listener.DopplerVelocity.Y;
        //        byte b = (byte)listener.DopplerVelocity.Z;
        //        Log(listener.Position.X + "");
        //        Log(listener.Orientation.X + "");
        //        Log("R: " + r + ", G: " + g + ", B: " + b);
        //        await PutTaskDelay();
        //    }
        //}
    }
}