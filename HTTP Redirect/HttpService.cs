using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Routing;

namespace HTTP_Redirect_Server
{
    public partial class HttpService : ServiceBase
    {
        private static int port = 80;
        private static IPAddress ip = IPAddress.Any;
        private static Uri url;
        public static NameValueCollection AppSettings { get; }
        private EventLog logger;
        private Thread _thread;
        private HttpServer _server;

        public HttpService()
        {
            InitializeComponent();
            logger = new EventLog();
            if (!EventLog.SourceExists("HttpRedirect"))
            {
                EventLog.CreateEventSource(
                    "HttpRedirect", "HTTP Redirect Log");
            }
            logger.Source = "HttpRedirect";
            logger.Log = "HTTP Redirect Log";

        }

        protected override void OnStart(string[] args)
        {
            try
            {
                logger.WriteEntry("Entered OnStart");
                ReadAllSettings();
                _server = new HttpServer(ip, port, url, logger);
                _thread = new Thread(_server.Listen);
                _thread.Name = "Http Redirect Thread";
                _thread.IsBackground = true;
                _thread.Start();
            }
            catch (FormatException e)
            {
                logger.WriteEntry(e.Message);
                ShowHelp();
                return;
            }
            catch (Exception e)
            {
                logger.WriteEntry(e.Message);
            }
        }

        protected override void OnStop()
        {
            logger.WriteEntry("Entered OnStop");
            _server.Stop();
        }


        private void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    logger.WriteEntry("AppSettings is empty.");
                }
                else
                {
                    ip = IPAddress.Parse(appSettings["IP"]);
                    port = int.Parse(appSettings["Port"]);
                    url = new Uri(appSettings["Redirect"]);
                }
            }
            catch (ConfigurationErrorsException)
            {
                logger.WriteEntry("Error reading app settings");
            }
        }

        private void ShowHelp()
        {
            var name = Process.GetCurrentProcess().MainModule.FileName;
            logger.WriteEntry(string.Format("Usage: \n\n{0} <Redirect to URL - required> <Local Socket (optional) - default: *:80>", name));
        }

    }
}
