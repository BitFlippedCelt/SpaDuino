using System;
using Microsoft.SPOT;
using System.Collections;

namespace SpaDuino.Controllers
{
    public interface IController
    {
        void Activate();
        void Deactivate();
        void Toggle();

        ArrayList Ports { get; set; }
    }
}
