using System;

namespace Twilio.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class TwilioAttribute : Attribute
    {
        public string To { get; set; }
        public string Body { get; set; }
    }
}
