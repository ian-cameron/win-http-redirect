using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HTTP_Redirect_Server
{
    public class HttpServer
    {
        private int Port;
        private TcpListener Listener;
        private HttpProcessor Processor;
        private IPAddress IP;
        private Uri URL;
        private bool IsActive = true;
        private EventLog logger;


        public HttpServer(IPAddress ip, int port, Uri url, EventLog log)
        {
            logger = log;
            IP = ip;
            URL = url;
            Port = port;
            Processor = new HttpProcessor(url);
        }

        public void Stop()
        {
            this.IsActive = false;
            logger.WriteEntry("Stopping Listener");
            Listener.Stop();
        }

        public void Listen()
        {
            this.Listener = new TcpListener(IP, this.Port);
            this.Listener.Start();
            logger.WriteEntry(String.Format("Now listening on {0}:{1}, Redirecting to {2}", IP, Port, URL.AbsoluteUri));
            while (this.IsActive)
            {
                TcpClient s = Listener.AcceptTcpClient();
                Thread thread = new Thread(() =>
                {
                    Processor.HandleClient(s);
                });
                thread.Start();
                Thread.Sleep(1);
            }
        }
    }
}
