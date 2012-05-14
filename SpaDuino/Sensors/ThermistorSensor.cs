using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Reflection;

using netduino.helpers.Math;
using System.Threading;

namespace SpaDuino.Sensors
{
    public class ThermistorSensor : IDisposable
    {
        const int SAMPLECOUNT = 5;
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputPin"></param>
        /// <param name="tempNominal"></param>
        /// <param name="thermistorNominal"></param>
        /// <param name="beta"></param>
        /// <param name="baseResistance"></param>
        public ThermistorSensor(Cpu.Pin inputPin, float tempNominal = 25, float thermistorNominal = 10000, float beta = 3950, float baseResistance = 10000)
        {
            _Port = new AnalogInput(inputPin);
            _TempNominal = tempNominal;
            _ThermistorNominal = thermistorNominal;
            _Beta = beta;
            _BaseResistance = baseResistance;
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
            }
        }
        ~ThermistorSensor()
        {
            Dispose(false);
        }
        #endregion

        /// <summary>
        /// Returns temp in Fahrenheit
        /// Using Steinhart-Hart Equation http://en.wikipedia.org/wiki/Steinhart%E2%80%93Hart_equation
        /// </summary>
        /// <returns></returns>
        public float GetReading(Sensors.Types.TemperatureTypes tempType, int sampleCount = SAMPLECOUNT)
        {
            if (_Port != null)
            {
                float average = 0.0f;

                // Capture sampleCount samples for average
                for (int i = 0; i < sampleCount; i++)
                {
                    average += _Port.Read();
                    
                    Thread.Sleep(20); // Sleep before next iteration
                }

                // Get averaged value
                average /= sampleCount;

                Debug.Print("Average reading from analog pin: " + average);

                if (average > 0)
                {
                    float Resistance = ((1024 * _BaseResistance / average) - _BaseResistance);
                    float temp = Trigo.Log(Resistance); // Saving the Log(resistance) so not to calculate  it 4 times later
                    temp = 1 / (0.001129148f + (0.000234125f * temp) + (0.0000000876741f * temp * temp * temp));

                    switch (tempType)
                    {
                        case Types.TemperatureTypes.Kelvin:
                            return temp;

                        case Types.TemperatureTypes.Celsius:
                            return temp - 273.15f;

                        default:
                            return ((temp - 273.15f) * 9.0f) / 5.0f + 32.0f;
                    }
                }
            }

            return 0.0f;
        }

        private readonly AnalogInput _Port;
        private readonly float _TempNominal;
        private readonly float _ThermistorNominal;
        private readonly float _BaseResistance;
        private readonly float _Beta;
    }
}
