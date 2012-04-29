using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace SpaDuino.Controllers
{
    class RelayController : IController, IDisposable
    {
        public RelayController()
        {
            Ports = new ArrayList();
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
        ~RelayController()
        {
            Dispose(false);
        }
        
        public void Activate()
        {
            foreach (OutputPort port in Ports)
            {
                port.Write(true);
                State = true;
            }
        }

        public void Deactivate()
        {
            foreach (OutputPort port in Ports)
            {
                port.Write(false);
                State = false;
            }
        }

        public void Toggle()
        {
            foreach (OutputPort port in Ports)
            {
                _toggle = (_toggle) ? false : true;
                port.Write(_toggle);
                State = _toggle;
            }
        }

        public ArrayList Ports
        {
            get;
            set;
        }

        public Boolean State
        {
            get;
            private set;
        }

        private Boolean _toggle = false;
    }
}
