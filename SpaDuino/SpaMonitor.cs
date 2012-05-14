using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

using netduino.helpers.Math;

namespace SpaDuino
{
    public class SpaMonitor : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ambientPin"></param>
        /// <param name="coldPin"></param>
        /// <param name="hotPin"></param>
        public SpaMonitor(Cpu.Pin ambientPin, Cpu.Pin coldPin, Cpu.Pin hotPin)
        {
            _AmbientSensor = new Sensors.ThermistorSensor(ambientPin);
            _ColdSensor = new Sensors.ThermistorSensor(coldPin);
            _HotSensor = new Sensors.ThermistorSensor(hotPin);
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_AmbientSensor != null)
                    _AmbientSensor.Dispose();
                if (_ColdSensor != null)
                    _ColdSensor.Dispose();
                if (_HotSensor != null)
                    _HotSensor.Dispose();
            }
        }

        ~SpaMonitor()
        {
            Dispose(false);
        }
        #endregion

        public bool StartMonitorThread()
        {
            if (_MonitorThread == null || !_MonitorThread.IsAlive)
            {
                if (_MonitorThread != null)
                    _MonitorThread.Resume();
                else
                {
                    _MonitorThread = new Thread(MonitorThread);
                    _MonitorThread.Start();
                }

                return true;
            }

            return false;
        }

        public bool StopMonitorThread()
        {
            if (_MonitorThread != null && _MonitorThread.IsAlive)
            {
                _MonitorThread.Abort();
                _MonitorThread = null;

                return true;
            }

            return false;
        }

        private void MonitorThread()
        {
            while (true)
            {
                // Perform readings and send values to logger then analyzer
                AmbientTemp = _AmbientSensor.GetReading(Sensors.Types.TemperatureTypes.Fahrenheit); // Ambient
                ColdTemp = _ColdSensor.GetReading(Sensors.Types.TemperatureTypes.Fahrenheit); // Coil intake
                HotTemp = _HotSensor.GetReading(Sensors.Types.TemperatureTypes.Fahrenheit); // Coil return

                Debug.Print("AmbientTemp: " + AmbientTemp);
                Debug.Print("Cold: " + ColdTemp);
                Debug.Print("Hot: " + HotTemp);

                Thread.Sleep(1000);
            }
        }

        private Thread _MonitorThread = null;

        private readonly Sensors.ThermistorSensor _AmbientSensor;
        private readonly Sensors.ThermistorSensor _ColdSensor;
        private readonly Sensors.ThermistorSensor _HotSensor;

        public float AmbientTemp { get; protected set; }
        public float ColdTemp { get; protected set; }
        public float HotTemp { get; protected set; }

        public bool Running
        {
            get
            {
                if (_MonitorThread != null)
                    return _MonitorThread.IsAlive;

                return false;
            }
        }
    }
}
