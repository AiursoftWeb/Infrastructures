using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Aiursoft.Account.Services
{
    public class AccountSMSSender
    {
        public string SMSAccountIdentification;
        public string SMSAccountPassword;
        public string SMSAccountFrom;
        private readonly ILogger _logger;
        public AccountSMSSender(
            IConfiguration configuration,
            ILogger<AccountSMSSender> logger)
        {
            SMSAccountFrom = configuration["SMSAccountFrom"];
            SMSAccountIdentification = configuration["SMSAccountIdentification"];
            SMSAccountPassword = configuration["SMSAccountPassword"];
            _logger = logger;
        }

        public Task SendAsync(string number, string message)
        {
            if (string.IsNullOrWhiteSpace(SMSAccountFrom))
            {
                throw new ArgumentNullException();
            }
            if (string.IsNullOrWhiteSpace(SMSAccountIdentification))
            {
                throw new ArgumentNullException();
            }
            if (string.IsNullOrWhiteSpace(SMSAccountPassword))
            {
                throw new ArgumentNullException();
            }
            try
            {
                TwilioClient.Init(SMSAccountIdentification, SMSAccountPassword);
                return MessageResource.CreateAsync(
                  to: new PhoneNumber(number),
                  from: new PhoneNumber(SMSAccountFrom),
                  body: message);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, e.Message);
                return Task.CompletedTask;
            }
        }
    }
}
