using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Net.Http.Headers;
using System.Net.Http;
using server;

namespace server
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            Console.WriteLine("Choose port number: ");
            string number = Console.ReadLine();
            var config = new HttpSelfHostConfiguration("http://localhost:" + number);

            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }
    }
}
