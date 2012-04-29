using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Reflection;

using netduino.helpers.Math;

namespace SpaDuino.Sensors
{
    public class ThermistorSensor : IDisposable
    {
        const float VREF = 3.3f;

        public ThermistorSensor(Cpu.Pin inputPin, float baseResistance)
        {
            Port = new AnalogInput(inputPin);
            BaseResistance = baseResistance;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
        ~ThermistorSensor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Returns temp in Fahrenheit
        /// Sourced this section from http://arduino.cc/playground/ComponentLib/Thermistor2
        /// </summary>
        /// <returns></returns>
        public double GetReading()
        {
            double temp = 0.0;

            if (Port != null)
            {
                int adc = Port.Read();

                if (adc > 0)
                {
                    float Resistance = ((1024 * BaseResistance / adc) - BaseResistance);
                    temp = Trigo.Log(Resistance); // Saving the Log(resistance) so not to calculate  it 4 times later
                    temp = 1 / (0.001129148 + (0.000234125 * temp) + (0.0000000876741 * temp * temp * temp));
                    temp -= 273.15;  // to Celsius
                    temp = (temp * 9.0) / 5.0 + 32.0; // to Fahrenheit
                }
            }

            return temp;
        }

        public AnalogInput Port { get; protected set; }
        public float BaseResistance { get; protected set; }
    }
}
