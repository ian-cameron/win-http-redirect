using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTTP_Redirect_Server
{
    public class HttpProcessor
    {
        private Uri url;

        public HttpProcessor(Uri _url)
        {
            url = _url;
        }

        public void HandleClient(TcpClient tcpClient)
        {
            Stream inputStream = tcpClient.GetStream();
            Stream outputStream = tcpClient.GetStream();
            HttpWebRequest request = GetRequest(inputStream);
            string redirectedUrl = url.Scheme + "://" +
                                   url.Host.ToString() +
                                   (url.AbsolutePath == "/" ? "" : ("/" + url.AbsolutePath.Trim('/'))) +
                                   request.RequestUri.AbsolutePath +
                                   url.Query;
            if (request.RequestUri.Query.Length > 0 && url.Query.Length > 0)
            {
                redirectedUrl = redirectedUrl + request.RequestUri.Query.Replace('?', '&');
            }
            else if (request.RequestUri.Query.Length > 0)
            {
                redirectedUrl = redirectedUrl + request.RequestUri.Query;
            }

            string rawResponse = "HTTP/1.1 301 Moved Permanently\r\n";
            rawResponse = rawResponse + "Server: HTTP-Redirect v0.1\r\n";
            rawResponse = rawResponse + "Location: " + redirectedUrl;

            //Console.WriteLine("Received request {0} \r\nRedirecting to {1}", request.RequestUri.ToString(), redirectedUrl);

            WriteResponse(outputStream, rawResponse);

            outputStream.Flush();
            outputStream.Close();
            outputStream = null;

            inputStream.Close();
            inputStream = null;

        }
        private HttpWebRequest GetRequest(Stream inputStream)
        {
            string request = Readline(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            string method = tokens[0].ToUpper();
            string url = tokens[1];
            return (HttpWebRequest)WebRequest.Create("http://localhost" + url);
        }

        private static void WriteResponse(Stream stream, string response)
        {
            Write(stream, response.ToString() + "\r\n\r\n");
        }

        private static void Write(Stream stream, string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static string Readline(Stream stream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = stream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }
    }
}
