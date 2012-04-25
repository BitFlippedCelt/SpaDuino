using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace SpaDuino
{
    public class Program : IDisposable
    {
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
            _webServer = new SimpleWeb.Server();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        SimpleWeb.Server _webServer;
    }
}
