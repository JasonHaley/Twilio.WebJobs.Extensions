using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Twilio.WebJobs.Extensions.Bindings;

namespace Twilio.WebJobs.Extensions.Config
{
    public static class TwilioJobHostConfigurationExtensions
    {
        public static void UseTwilio(this JobHostConfiguration config, TwilioConfiguration twilioConfiguration = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (twilioConfiguration == null)
            {
                twilioConfiguration = new TwilioConfiguration();
            }

            config.RegisterExtensionConfigProvider(new TwilioExtensionConfig(twilioConfiguration));
        }

        private class TwilioExtensionConfig : IExtensionConfigProvider
        {
            private TwilioConfiguration _twilioConfiguration;

            public TwilioExtensionConfig(TwilioConfiguration twilioConfiguration)
            {
                _twilioConfiguration = twilioConfiguration;
            }

            public void Initialize(ExtensionConfigContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                context.Config.RegisterBindingExtension(new TwilioAttributeBindingProvider(_twilioConfiguration, context.Config.NameResolver));
            }
        }

    }
}
