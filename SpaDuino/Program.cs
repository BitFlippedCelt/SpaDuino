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
        const String sdRoot = @"\SD\";
        InterruptPort ShutdownButton;
        Boolean Running = true;

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
            ShutdownButton = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            ShutdownButton.OnInterrupt += new NativeEventHandler(ShutdownButton_OnInterrupt);

            _webServer = new SimpleWeb.Server();

            // More test code
            //Controllers.RelayController relay = new Controllers.RelayController();
            //relay.Ports.Add(new OutputPort(Pins.GPIO_PIN_D0, false));
            //relay.Activate();

            Sensors.ThermistorSensor therm1 = new Sensors.ThermistorSensor(Pins.GPIO_PIN_A0, 10000);
            Sensors.ThermistorSensor therm2 = new Sensors.ThermistorSensor(Pins.GPIO_PIN_A1, 10000);
            Sensors.ThermistorSensor therm3 = new Sensors.ThermistorSensor(Pins.GPIO_PIN_A2, 10000);
            
            while (Running)
            {
                // Perform readings and send values to logger then analyzer
                double reading1 = therm1.GetReading();
                Debug.Print("S1: " + reading1);

                double reading2 = therm2.GetReading();
                Debug.Print("S2: " + reading2);

                double reading3 = therm3.GetReading();
                Debug.Print("S3: " + reading3);

                Thread.Sleep(1000);
            }

            //relay.Deactivate();
            //relay.Dispose();
            ShutdownButton.Dispose();
        }

        void ShutdownButton_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            Running = false;
        }

        SimpleWeb.Server _webServer;
    }
}
