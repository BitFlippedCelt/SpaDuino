using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Text;

namespace SimpleWeb
{
    public class Server : IDisposable
    {
        /// <summary>
        /// Initializes Server object using port
        /// </summary>
        /// <param name="port">Network port to bind the listening socket to</param>
        public Server(UInt16 port = 8080)
        {
            // Setup the socket and bind to interface
            _webSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _webSocket.Bind(new IPEndPoint(IPAddress.Any, 80));
            _webSocket.Listen(10);

            // Debug output
            _ServerIPAddress = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress;
            Debug.Print("Listening for requests on " + ServerIPAddress + ":" + port);

            // Spin up socket listening thread
            _webListenThread = new Thread(ListenForRequests);
            _webListenThread.Start();
        }

        protected void ListenForRequests()
        {
            Debug.Print("Listening...");

            while (_webListenThread.ThreadState == ThreadState.Running)
            {
                using (Socket clientSocket = _webSocket.Accept())
                {
                    Debug.Print("Accepted connection originating from " + clientSocket.RemoteEndPoint);

                    int bytesReceived = clientSocket.Available;
                    if (bytesReceived > 0)
                    {
                        //Get request
                        byte[] buffer = new byte[bytesReceived];
                        int byteCount = clientSocket.Receive(buffer, bytesReceived, SocketFlags.None);
                        string request = new string(Encoding.UTF8.GetChars(buffer));

                        string response = ProcessRequest(request);
                        string header = "HTTP/1.0 200 OK\r\nContent-Type: text; charset=utf-8\r\nContent-Length: " + response.Length + "\r\nConnection: close\r\n\r\n";
                        clientSocket.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                        clientSocket.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);

                        //Debug.Print(request);
                        //Compose a response
                        //string response = "Hello World";
                        //string header = String.Format("HTTP/1.0 200 OK\r\nContent-Type: text; charset=utf-8\r\nContent-Length: {0}\r\nConnection: close\r\n\r\n", response.Length);
                        //clientSocket.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                        //clientSocket.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);
                    }
                }

                Thread.Sleep(150);
            }
        }

        protected String ProcessRequest(string request)
        {
            String result = "";

            HTTPRequest httpRequest = HTTPRequest.ParseRequestString(request);

            return result;
        }
            
        #region IDisposable implementation
        ~Server()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_webSocket != null)
                _webSocket.Close();

            if (_webListenThread != null && _webListenThread.IsAlive)
                _webListenThread.Abort();
        }
        #endregion

        public String ServerIPAddress
        {
            get
            {
                return _ServerIPAddress;
            }
        }

        private readonly Socket _webSocket;
        private Thread _webListenThread; 
        private readonly String _ServerIPAddress = "";
    }
}
