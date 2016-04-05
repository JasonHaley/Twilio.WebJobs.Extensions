using Microsoft.Azure.WebJobs;
using Twilio.WebJobs.Extensions.Config;

namespace SmsSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new JobHostConfiguration();

            config.UseDevelopmentSettings();

            config.UseTwilio();

            JobHost host = new JobHost(config);

            host.RunAndBlock();
        }
    }
}
