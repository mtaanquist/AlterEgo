using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AlterEgo
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if RELEASE
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
#endif
#if DEBUG
            var host = new WebHostBuilder()
                .UseKestrel(options => { options.UseHttps("kestrel-dev.pfx", "boobies"); })
                .UseUrls("http://localhost:5000", "https://localhost:5001")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
#endif
            host.Run();
        }
    }
}
