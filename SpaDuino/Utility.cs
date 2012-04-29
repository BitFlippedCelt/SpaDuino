using System;
using System.Net.Sockets;
using System.Net;
using Microsoft.SPOT;
using Microsoft.SPOT.Time;
using System.IO;

namespace SpaDuino
{
    public static class Utility
    {
        public static bool CheckForDir(String path, bool createIfNotFound = false)
        {
            bool result = false; // Assume it exists

            try
            {
                DirectoryInfo rootDirectory = new DirectoryInfo(path);
                if (!rootDirectory.Exists)
                {
                    if (createIfNotFound)
                    {
                        try
                        {
                            rootDirectory.Create();
                            result = true;
                        }
                        catch (IOException ex)
                        {
                            Debug.Print(ex.Message);
                        }
                    }
                }
                else
                    result = true;
            }
            catch (IOException)
            { }

            return result;
        }

        /// <summary>
        /// Get DateTime from NTP Server 
        /// Sourced from: http://forums.netduino.com/index.php?/topic/475-still-learning-internet-way-to-grab-date-and-time-on-startup/
        /// </summary>
        /// <param name="TimeServer">Timeserver</param>
        /// <returns>Local NTP Time</returns>
        public static DateTime NTPTime(String TimeServer, int UTC_offset)
        {
            // Find endpoint for timeserver
            IPEndPoint ep = new IPEndPoint(Dns.GetHostEntry(TimeServer).AddressList[0], 123);

            // Connect to timeserver
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.Connect(ep);

            // Make send/receive buffer
            byte[] ntpData = new byte[48];
            Array.Clear(ntpData, 0, 48);

            // Set protocol version
            ntpData[0] = 0x1B;

            // Send Request
            s.Send(ntpData);

            // Receive Time
            s.Receive(ntpData);

            byte offsetTransmitTime = 40;

            ulong intpart = 0;
            ulong fractpart = 0;

            for (int i = 0; i <= 3; i++)
                intpart = (intpart << 8) | ntpData[offsetTransmitTime + i];

            for (int i = 4; i <= 7; i++)
                fractpart = (fractpart << 8) | ntpData[offsetTransmitTime + i];

            ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);

            s.Close();

            TimeSpan timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);
            DateTime dateTime = new DateTime(1900, 1, 1);
            dateTime += timeSpan;

            TimeSpan offsetAmount = new TimeSpan(0, UTC_offset, 0, 0, 0);
            DateTime networkDateTime = (dateTime + offsetAmount);

            return networkDateTime;
        }

    }
}
