using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using AccelerometerMPU9150App.Models;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AccelerometerMPU9150App
{
    /// <summary>
    /// Controlling of inertial unit MPU9159 using I2C
    /// </summary>
    public sealed partial class BasicPage : Page
    {
        bool timer = false;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private Timer periodicTimer;
        DataMPU9150 mpu = new DataMPU9150();
        public BasicPage()
        {
            this.InitializeComponent();
            Unloaded += BasicPage_Unloaded;
        }

        private void BasicPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (mpu.NotConnectedMessage == false) { mpu.DisposeMPU(); }

            if (timer) { periodicTimer.Dispose(); }
        }

        private void Callback(object state)
        {
            if (mpu.NotConnectedMessage == false)
            {
                try
                {
                    mpu.ReadingData();
                }
                catch (Exception)
                {
                    return;
                }
                var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    PrintOnScreen();
                });
            }
            else
            {
                TextBlockAddress.Text = "MPU9150 not connected, restart";
                TextBlockAddress.Foreground = redBrush;
            }

        }

        private void GetDataMPU()
        {
            try
            {
                PrintOnScreen();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void PrintOnScreen()
        {
            TextBlockAddress.Text = String.Format("Address: 0x{0}", mpu.address);
            TextBlockTemp.Text = String.Format("Temperture: {0:F2} degC", mpu.Temperature);
            TextBlockAccX.Text = String.Format("{0:F3} g", mpu.Acceleration_X / 1000);
            TextBlockAccY.Text = String.Format("{0:F3} g", mpu.Acceleration_Y / 1000);
            TextBlockAccZ.Text = String.Format("{0:F3} g", mpu.Acceleration_Z / 1000);
            TextBlockGyroX.Text = String.Format("{0:F1} deg/s", mpu.AngularSpeed_X);
            TextBlockGyroY.Text = String.Format("{0:F1} deg/s", mpu.AngularSpeed_Y);
            TextBlockGyroZ.Text = String.Format("{0:F1} deg/s", mpu.AngularSpeed_Z);
            TextBlockStatus.Text = "Status: Running";
        }

        private void ButtonTimerON_Click(object sender, RoutedEventArgs e)
        {
            if (mpu.NotConnectedMessage == false)
            {
                periodicTimer = new Timer(Callback, null, 0, 50);
                timer = true;
                ButtonTimerON.IsEnabled = false;
                ButtonTimerOFF.IsEnabled = true;
            }
            else
            {
                TextBlockAddress.Text = "MPU9150 not connected, restart";
                TextBlockAddress.Foreground = redBrush;
            }

        }

        private void ButtonTimerOFF_Click(object sender, RoutedEventArgs e)
        {
            periodicTimer.Dispose();
            timer = false;
            ButtonTimerOFF.IsEnabled = false;
            ButtonTimerON.IsEnabled = true;
        }

        private void MPU_ON_Click(object sender, RoutedEventArgs e)
        {
            if (mpu.NotConnectedMessage == true)
            {
                MPU_ON.IsEnabled = true;
                MPU_OFF.IsEnabled = false;
                ButtonTimerOFF.IsEnabled = false;
                ButtonTimerON.IsEnabled = false;
                mpu.NotConnectedMessage = false;
                TextBlockAddress.Text = "MPU9150 not connected, restart";
                TextBlockAddress.Foreground = redBrush;
            }
            else
            {
                mpu.InitMPUAsync();
                GetDataMPU();
                MPU_ON.IsEnabled = false;
                MPU_OFF.IsEnabled = true;
                ButtonTimerOFF.IsEnabled = false;
                ButtonTimerON.IsEnabled = true;
            }

        }

        private void MPU_OFF_Click(object sender, RoutedEventArgs e)
        {
            if (mpu.NotConnectedMessage == false)
            {
                mpu.DisposeMPU();
                periodicTimer.Dispose();
                MPU_ON.IsEnabled = true;
                MPU_OFF.IsEnabled = false;
                ButtonTimerOFF.IsEnabled = false;
                ButtonTimerON.IsEnabled = false;
            }
            else
            {
                MPU_OFF.IsEnabled = false;
            }

        }
    }
}
