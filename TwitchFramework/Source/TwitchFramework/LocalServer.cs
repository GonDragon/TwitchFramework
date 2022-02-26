using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace TwitchFramework
{
    public static class LocalServer
    {
        static readonly HttpListener _httpListener = new HttpListener();

        public static void Start()
        {
            TwitchFramework.Log("Starting server...");
            _httpListener.Prefixes.Add("http://localhost:5000/");
            _httpListener.Start();
            TwitchFramework.Log("Server started.");


            Thread _responseThread = new Thread(ResponseThread);
            _responseThread.Start(); // start the response thread
        }

        public static void Stop()
        {
            _httpListener.Stop();
            TwitchFramework.Log("Server closed.");
        }

        private static void ResponseThread()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext context = _httpListener.GetContext();

                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string raw_queries = request.Url.Query;

                if ((request.HttpMethod == "POST") && (request.Url.AbsolutePath == "/shutdown"))
                {
                    runServer = false;
                }
                else if (raw_queries.Length != 0 && request.Url.AbsolutePath != "/favicon.ico")
                {
                    string[] queries = raw_queries.Substring(1).Split('&');

                    foreach (string querie in queries)
                    {
                        string[] parts = querie.Split('=');
                        if (parts[0] == "access_token")
                        {
                            TwitchFramework.settings.TwitchAccessToken = parts[1];
                            runServer = false;
                        }
                    }
                }
                else if (request.Url.AbsolutePath != "/favicon.ico")
                {
                    string raw_html = TwitchFramework.ReadTextResourceFromAssembly("index.html");

                    byte[] encoded_response = Encoding.UTF8.GetBytes(raw_html);

                    response.ContentType = "text/html";
                    response.ContentEncoding = Encoding.UTF8;
                    response.ContentLength64 = encoded_response.LongLength;

                    response.OutputStream.Write(encoded_response, 0, encoded_response.Length);
                }

                //response.KeepAlive = false;
                response.Close();
            }

            Stop();
        }
    }
}
