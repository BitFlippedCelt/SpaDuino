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
    public class Program : IDisposable
    {
        const String SDROOT = @"\SD\";
        const String TEMPERATURELOGFILE = @"temp.log";
        const int TIMEZONEOFFSET = -8;
        const int ITERATIONBEFORELOG = 30;
        const int AVGSAMPLECOUNT = 60;

        const double AMBIENTTRIGGER = 70.0; // Minimum temp in the ambient air before turning pump on
        const double RETURNRISEMIN = 2.0; // Minimum difference to accept before shutting pump down

        public static void Main()
        {
            new Program().run();
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
                if (_WebServer != null)
                {
                    _WebServer.Dispose();
                    _WebServer = null;
                }

                if (_SpaMonitorThread != null)
                {
                    _SpaMonitorThread.Dispose();
                    _SpaMonitorThread = null;
                }
            }   
        }
        ~Program()
        {
            Dispose(false);
        }
        #endregion

        private void run()
        {
            // Setup data logger
            try
            {
                String logPath = Path.Combine(SDROOT, TEMPERATURELOGFILE);
                if (File.Exists(logPath))
                    File.Delete(logPath);

                _DataLogger = new DataLogger(logPath);
                _DataLogger.WriteBreak();
            }
            catch (IOException ex)
            {
                Debug.Print(ex.ToString());
            }

            // Set local time
            try
            {
                //DateTime ntpTime = Utility.NTPTime("pool.ntp.org", TIMEZONEOFFSET);
                //Microsoft.SPOT.Hardware.Utility.SetLocalTime(ntpTime);

                //Debug.Print("Set netduino time to: " + ntpTime);
            }
            catch (Exception ex)
            {
                Debug.Print("Failed to grab time from NTP server!");
            }

            // Startup web server
            _WebServer = new SimpleWeb.Server();

            // More test code
            Controllers.RelayController relay = new Controllers.RelayController();
            relay.Ports.Add(new OutputPort(Pins.GPIO_PIN_D0, false));

            // 
            _SpaMonitorThread = new SpaMonitor(Pins.GPIO_PIN_A0, Pins.GPIO_PIN_A1, Pins.GPIO_PIN_A2);
            _SpaMonitorThread.StartMonitorThread();

            while (_SpaMonitorThread.Running)
            {
                Debug.Print("Ambient: " + _SpaMonitorThread.AmbientTemp);
                Debug.Print("Cold: " + _SpaMonitorThread.ColdTemp);
                Debug.Print("Hot: " + _SpaMonitorThread.HotTemp);

                Thread.Sleep(1000);
            }

            relay.Deactivate();
            relay.Dispose();
        }

        private SimpleWeb.Server _WebServer;
        private DataLogger _DataLogger;
        private SpaMonitor _SpaMonitorThread;
    }
}
