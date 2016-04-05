using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings.Path;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Twilio.WebJobs.Extensions.Config;

namespace Twilio.WebJobs.Extensions.Bindings
{
    internal class TwilioBinding : IBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly TwilioAttribute _attribute;
        private readonly TwilioConfiguration _config;
        private readonly INameResolver _nameResolver;
        private readonly TwilioRestClient _twilioClient;
        private readonly BindingTemplate _toFieldBindingTemplate;
        private readonly BindingTemplate _bodyFieldBindingTemplate;
        
        public TwilioBinding(ParameterInfo parameter, TwilioAttribute attribute, TwilioConfiguration config, INameResolver nameResolver, BindingProviderContext context)
        {
            _parameter = parameter;
            _attribute = attribute;
            _config = config;
            _nameResolver = nameResolver;

            _twilioClient = new TwilioRestClient(_config.SID, _config.Token);

            if (!string.IsNullOrEmpty(_attribute.To))
            {
                _toFieldBindingTemplate = CreateBindingTemplate(_attribute.To, context.BindingDataContract);
            }
            
            if (!string.IsNullOrEmpty(_attribute.Body))
            {
                _bodyFieldBindingTemplate = CreateBindingTemplate(_attribute.Body, context.BindingDataContract);
            }
        }

        public bool FromAttribute
        {
            get { return true; }
        }

        public async Task<IValueProvider> BindAsync(BindingContext context)
        {
            Message message = CreateDefaultMessage(context.BindingData);

            return await BindAsync(message, context.ValueContext);
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            Message message = (Message)value;

            return Task.FromResult<IValueProvider>(new TwilioValueBinder(_twilioClient, message));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor
            {
                Name = _parameter.Name
            };
        }

        internal Message CreateDefaultMessage(IReadOnlyDictionary<string, object> bindingData)
        {
            Message message = new Message();

            if (_config.FromPhone != null)
            {
                message.From = _config.FromPhone;
            }

            if (_toFieldBindingTemplate != null)
            {
                message.To = _toFieldBindingTemplate.Bind(bindingData);
            }
            //else
            //{
            //    if (!string.IsNullOrEmpty(_config.To))
            //    {
            //        message.To = _config.To);
            //    }
            //}
            
            if (_bodyFieldBindingTemplate != null)
            {
                message.Body = _bodyFieldBindingTemplate.Bind(bindingData);
            }

            return message;
        }

        private BindingTemplate CreateBindingTemplate(string pattern, IReadOnlyDictionary<string, Type> bindingDataContract)
        {
            if (_nameResolver != null)
            {
                pattern = _nameResolver.ResolveWholeString(pattern);
            }
            BindingTemplate bindingTemplate = BindingTemplate.FromString(pattern);
            bindingTemplate.ValidateContractCompatibility(bindingDataContract);

            return bindingTemplate;
        }
        
        internal class TwilioValueBinder : IValueBinder
        {
            private readonly Message _message;
            private readonly TwilioRestClient _twilioClient;

            public TwilioValueBinder(TwilioRestClient twilioClient, Message message)
            {
                _message = message;
                _twilioClient = twilioClient;
            }

            public Type Type
            {
                get
                {
                    return typeof(Message);
                }
            }

            public object GetValue()
            {
                return _message;
            }

            public Task SetValueAsync(object value, CancellationToken cancellationToken)
            {
                if (value == null)
                {
                    // if this is a 'ref' binding and the user set the parameter to null, that
                    // signals that they don't want us to send the message
                    return Task.FromResult<object>(null);
                }

                if (string.IsNullOrEmpty(_message.To))
                {
                    throw new InvalidOperationException("A 'To' sms number must be specified for the message.");
                }
                if (string.IsNullOrEmpty(_message.From))
                {
                    throw new InvalidOperationException("A 'From' phone number must be specified for the message.");
                }

                var result = _twilioClient.SendMessage(_message.From, "+1" + _message.To, _message.Body);

                System.Diagnostics.Debug.WriteLine(result.ErrorMessage);
                System.Diagnostics.Debug.WriteLine(result.Sid);

                return Task.FromResult<object>(null);
            }

            public string ToInvokeString()
            {
                return null;
            }
        }
    }
}
