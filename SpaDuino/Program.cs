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

        const double AMBIENTTRIGGER = 85.0; // Minimum temp in the ambient air before turning pump on
        const double RETURNRISEMIN = 3.0; // Minimum difference to accept before shutting pump down

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
                if (_webServer != null)
                {
                    _webServer.Dispose();
                    _webServer = null;
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

                _dataLogger = new DataLogger(logPath);
                _dataLogger.WriteBreak();
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
            //_webServer = new SimpleWeb.Server();

            // More test code
            Controllers.RelayController relay = new Controllers.RelayController();
            relay.Ports.Add(new OutputPort(Pins.GPIO_PIN_D0, false));

            Sensors.ThermistorSensor therm1 = new Sensors.ThermistorSensor(Pins.GPIO_PIN_A0, 10000);
            Sensors.ThermistorSensor therm2 = new Sensors.ThermistorSensor(Pins.GPIO_PIN_A1, 10000);
            Sensors.ThermistorSensor therm3 = new Sensors.ThermistorSensor(Pins.GPIO_PIN_A2, 10000);
            
            int iterationCounter = 0;
            while (Running)
            {
                // Perform readings and send values to logger then analyzer
                double r1 = therm1.GetReading(); // Ambient
                double r2 = therm2.GetReading(); // Coil intake
                double r3 = therm3.GetReading(); // Coil return
                //Debug.Print("S1: " + r1);
                //Debug.Print("S2: " + r2);
                //Debug.Print("S3: " + r3);

                // Very basic logic to control pump based on simple constraints defined at build
                // This will migrate to the config, and have to have config file requeried for changes
                // from time to time
                if (!relay.State)
                {
                    if (r1 >= AMBIENTTRIGGER)
                        relay.Activate();
                }
                else
                {
                    if (r3 <= (r2 + RETURNRISEMIN) || r1 < AMBIENTTRIGGER)
                        relay.Deactivate();
                }


                if (iterationCounter++ >= ITERATIONBEFORELOG)
                {
                    _dataLogger.WriteLine(r1 + ", " + r2 + ", " + r3);
                    iterationCounter = 0;
                }

                Thread.Sleep(1000);
            }

            relay.Deactivate();
            relay.Dispose();
        }

        SimpleWeb.Server _webServer;
        DataLogger _dataLogger;
        Boolean Running = true;
    }
}
