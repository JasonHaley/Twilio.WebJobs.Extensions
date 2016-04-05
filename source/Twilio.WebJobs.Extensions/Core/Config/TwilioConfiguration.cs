
using System;
using System.Configuration;

namespace Twilio.WebJobs.Extensions.Config
{
    public class TwilioConfiguration
    {
        // ReSharper disable once InconsistentNaming
        internal const string AzureWebJobsTwilioSIDKeyName = "AzureWebJobsTwilioSID";
        internal const string AzureWebJobsTwilioTokenKeyName = "AzureWebJobsTwilioToken";
        internal const string AzureWebJobsTwilioFromPhoneKeyName = "AzureWebJobsTwilioFromPhone";

        public TwilioConfiguration()
        {
            SID = ConfigurationManager.AppSettings.Get(AzureWebJobsTwilioSIDKeyName);
            if (string.IsNullOrEmpty(SID))
            {
                SID = Environment.GetEnvironmentVariable(AzureWebJobsTwilioSIDKeyName);
            }

            Token = ConfigurationManager.AppSettings.Get(AzureWebJobsTwilioTokenKeyName);
            if (string.IsNullOrEmpty(Token))
            {
                Token = Environment.GetEnvironmentVariable(AzureWebJobsTwilioTokenKeyName);
            }

            FromPhone = ConfigurationManager.AppSettings.Get(AzureWebJobsTwilioFromPhoneKeyName);
            if (string.IsNullOrEmpty(FromPhone))
            {
                FromPhone = Environment.GetEnvironmentVariable(AzureWebJobsTwilioFromPhoneKeyName);
            }
        }

        public string SID { get; set; }
        public string Token { get; set; }
        public string FromPhone { get; set; }
    }
}
