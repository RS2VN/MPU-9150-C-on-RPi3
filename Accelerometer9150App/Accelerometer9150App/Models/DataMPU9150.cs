using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace AccelerometerMPU9150App.Models
{
    class DataMPU9150
    {
        private I2cDevice MPU;
        public string address;
        public double Temperature { get; set; }
        public double Acceleration_X { get; set; }
        public double Acceleration_Y { get; set; }
        public double Acceleration_Z { get; set; }
        public double AngularSpeed_X { get; set; }
        public double AngularSpeed_Y { get; set; }
        public double AngularSpeed_Z { get; set; }
        public bool NotConnectedMessage = false;

        public async void InitMPUAsync()
        {
            var settings = new I2cConnectionSettings(DataMPU9150.MPU_I2C_ADDRESS);
            settings.BusSpeed = I2cBusSpeed.FastMode;
            var controller = await I2cController.GetDefaultAsync();
            MPU = controller.GetDevice(settings);

            if (MPU == null)
            {
                NotConnectedMessage = true;
            }
            else
            {
                try
                {
                    Configuration();
                    ReadingData();
                    NotConnectedMessage = false;
                }
                catch (Exception)
                {
                    NotConnectedMessage = true;
                    return;
                }
            }
        }
        public void DisposeMPU()
        {
            if (MPU != null)
            {
                MPU.Dispose();
            }
        }

        public void ReadingData()
        {
            Temperature = ReadingValue(Buf_TEMP) / 340 + 35;
            Acceleration_X = ReadingValue(Buf_ACCX) / 8.192 + 26;
            Acceleration_Y = ReadingValue(Buf_ACCY) / 8.192 - 25;
            Acceleration_Z = ReadingValue(Buf_ACCZ) / 8.192 - 26;
            AngularSpeed_X = ReadingValue(Buf_GYROX) / 65.5 + 0.7;
            AngularSpeed_Y = ReadingValue(Buf_GYROY) / 65.5 - 2.8;
            AngularSpeed_Z = ReadingValue(Buf_GYROZ) / 65.5 + 0.3;
        }

        private double ReadingValue(byte[] buf_REG)
        {
            byte[] Read_REG = new byte[2];
            MPU.WriteRead(buf_REG, Read_REG);
            byte tempH = Read_REG[0];
            byte tempL = Read_REG[1];
            Int16 TempRaw = (Int16)(tempH << 8 | tempL);

            double value = (double)TempRaw;
            return value;
        }

        private void Configuration()
        {
            MPU.Write(Buf_SMPLRT);
            MPU.Write(Buf_CONFIG);
            MPU.Write(Buf_GYRO_CONFIG);
            MPU.Write(Buf_ACCEL_CONFIG);
            MPU.Write(Buf_BYPASS);
            MPU.Write(Buf_PWR_MNGMT);

            MPU.WriteRead(Buf_WHO_AM_I, whoAmI);
            address = BitConverter.ToString(whoAmI);
        }


        //private byte[] Buf_SMPLRT = new byte[] { SMPLRT_DIV, SMPLRT_100Hz };
        private byte[] Buf_SMPLRT = new byte[] { SMPLRT_DIV, SMPLRT_20Hz };
        private byte[] Buf_CONFIG = new byte[] { CONFIG, CONFIG_94Hz };
        private byte[] Buf_GYRO_CONFIG = new byte[] { GYRO_CONFIG, GYRO_CONFIG_500 };
        private byte[] Buf_ACCEL_CONFIG = new byte[] { ACCEL_CONFIG, ACCEL_CONFIG_4g };
        private byte[] Buf_BYPASS = new byte[] { BYPASS, BYPASS_ENABLED };
        private byte[] Buf_PWR_MNGMT = new byte[] { PWR_MNGMT_1, PWR_MNGMT_ZCLOCK };

        private byte[] Buf_WHO_AM_I = new byte[] { WHO_AM_I };
        private byte[] whoAmI = new byte[1];
        private byte[] Buf_TEMP = new byte[] { TEMP_H };
        private byte[] Buf_ACCX = new byte[] { ACC_X_H };
        private byte[] Buf_ACCY = new byte[] { ACC_Y_H };
        private byte[] Buf_ACCZ = new byte[] { ACC_Z_H };
        private byte[] Buf_GYROX = new byte[] { GYRO_X_H };
        private byte[] Buf_GYROY = new byte[] { GYRO_Y_H };
        private byte[] Buf_GYROZ = new byte[] { GYRO_Z_H };

        /**address*/
        private const byte MPU_I2C_ADDRESS = 0x68; //ADDRESS MPU
        private const byte WHO_AM_I = 0x75;//WHO AM I
        /*configuration registers*/
        private const byte SMPLRT_DIV = 0x19;//SAMPLE RATE DIVIDER 
        private const byte CONFIG = 0x1A;//CONFIGURATION OF FILTER
        private const byte GYRO_CONFIG = 0x1B;//CONFIGURATION OF GYRO
        private const byte ACCEL_CONFIG = 0x1C;//CONFIGURATION OF ACCELEROMETER
        private const byte BYPASS = 0x37;
        private const byte PWR_MNGMT_1 = 0x6B;//POWER MANAGEMENT
        /*configuration data*/
        private const byte SMPLRT_100Hz = 0x09;//SAMPLE RATE 10ms
        private const byte SMPLRT_20Hz = 0x13;//SAMPLE Rate 50ms
        private const byte CONFIG_94Hz = 0x02;// FILTER accelerometer 94Hz
        private const byte GYRO_CONFIG_500 = 0x08;// GYRO +/- 500deg/s
        private const byte ACCEL_CONFIG_4g = 0x0A;//ACCELEROMETER +/- 4g -> 2.5Hz filter
        private const byte BYPASS_ENABLED = 0x02;//COMPAS direct reading enabled
        private const byte PWR_MNGMT_ZCLOCK = 0x03;//PLL with Z axis gyroscope reference
        /*data registers*/
        private const byte ACC_X_H = 0x3B;//accelerometer X 15:8
        private const byte ACC_X_L = 0x3C;//accelerometer X 7:0
        private const byte ACC_Y_H = 0x3D;//accelerometer Y 15:8
        private const byte ACC_Y_L = 0x3E;//accelerometer Y 7:0
        private const byte ACC_Z_H = 0x3F;//accelerometer Z 15:8
        private const byte ACC_Z_L = 0x40;//accelerometer Z 7:0
        private const byte TEMP_H = 0x41;//temperature 15:8
        private const byte TEMP_L = 0x42;//temperature 7:0
        private const byte GYRO_X_H = 0x43;//gyro X Z 15:8
        private const byte GYRO_X_L = 0x44;//gyro X 7:0
        private const byte GYRO_Y_H = 0x45;//gyro y Z 15:8
        private const byte GYRO_Y_L = 0x46;//gyro y 7:0
        private const byte GYRO_Z_H = 0x47;//gyro z Z 15:8
        private const byte GYRO_Z_L = 0x48;//gyro z 7:0

    }
}
