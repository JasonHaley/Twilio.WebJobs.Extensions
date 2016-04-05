
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Twilio.WebJobs.Extensions.Config;

namespace Twilio.WebJobs.Extensions.Bindings
{
    public class TwilioAttributeBindingProvider : IBindingProvider
    {
        private readonly TwilioConfiguration _config;
        private readonly INameResolver _nameResolver;

        public TwilioAttributeBindingProvider(TwilioConfiguration config, INameResolver nameResolver)
        {
            _config = config;
            _nameResolver = nameResolver;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ParameterInfo parameter = context.Parameter;
            TwilioAttribute attribute = parameter.GetCustomAttribute<TwilioAttribute>(inherit: false);
            if (attribute == null)
            {
                return Task.FromResult<IBinding>(null);
            }

            if (context.Parameter.ParameterType != typeof(Message) &&
                context.Parameter.ParameterType != typeof(Message).MakeByRefType())
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind TwilioAttribute to type '{0}'.", parameter.ParameterType));
            }

            if (string.IsNullOrEmpty(_config.SID))
            {
                throw new InvalidOperationException(
                    string.Format("The Twilio SID must be set either via a '{0}' app setting, via a '{0}' environment variable, or directly in code via TwilioConfiguration.SID.",
                    TwilioConfiguration.AzureWebJobsTwilioSIDKeyName));
            }

            if (string.IsNullOrEmpty(_config.Token))
            {
                throw new InvalidOperationException(
                    string.Format("The Twilio Token must be set either via a '{0}' app setting, via a '{0}' environment variable, or directly in code via TwilioConfiguration.Token.",
                    TwilioConfiguration.AzureWebJobsTwilioTokenKeyName));
            }

            if (string.IsNullOrEmpty(_config.FromPhone))
            {
                throw new InvalidOperationException(
                    string.Format("The Twilio FromPhone must be set either via a '{0}' app setting, via a '{0}' environment variable, or directly in code via TwilioConfiguration.FromPhone.",
                    TwilioConfiguration.AzureWebJobsTwilioFromPhoneKeyName));
            }

            return Task.FromResult<IBinding>(new TwilioBinding(parameter, attribute, _config, _nameResolver, context));
        }
    }
}
