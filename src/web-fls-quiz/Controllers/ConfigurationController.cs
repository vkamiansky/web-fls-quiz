using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Controllers
{
    public class ConfigurationController : Controller
    {
        private static DateTime _lastRequestTime;

        private static TimeSpan _timeBetweenRequests = new TimeSpan(0, 2, 0);

        private readonly IConfigurationService _configurationService;

        private readonly IDataStorage _dataStorage;

        private readonly IMailService _mailService;

        public ConfigurationController(
            IConfigurationService configurationService,
            IDataStorage dataStorage,
            IMailService mailService)
        {
            _configurationService = configurationService;
            _dataStorage = dataStorage;
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<string> Change([FromBody]Configuration newConfiguration)
        {
            if (string.IsNullOrEmpty(newConfiguration.Token))
                return "Error: token was not sent";
            if (string.IsNullOrEmpty(newConfiguration.IP))
                return "Error: IP address was not sent";
            if (string.IsNullOrEmpty(newConfiguration.Port))
                return "Error: port was not sent";

            var now = DateTime.Now;
            if (now - _lastRequestTime > _timeBetweenRequests)
            {
                _lastRequestTime = now;
                await TryReconfig(newConfiguration);
                return "Configuration change request accepted. Please, check admin email. If the configuration data is correct, a configuration change confirmation link will be sent.";
            }
            else
            {
                return $"Next configuration change will be possible at {_lastRequestTime + _timeBetweenRequests}";
            }
        }

        [HttpGet]
        public string Confirm(string confirmCode)
        {
            if (string.IsNullOrEmpty(confirmCode))
                return "Error: confirmation code was not sent";

            var result = _configurationService.ConfirmConfiguration(confirmCode);
            return result.ToString();
        }

        [NonAction]
        private async Task<bool> TryReconfig(Configuration newConfiguration)
        {
            if (_configurationService.CheckConfiguration(newConfiguration))
            {
                var confirmCode = _configurationService.SetConfiguration(newConfiguration);
                if (!string.IsNullOrEmpty(confirmCode))
                {
                    var result = await _mailService.SendConfirmCode(confirmCode);
                    if (!result)
                        result = await _mailService.SendConfirmCode(confirmCode, true);
                    return result;
                }
            }
            return false;
        }
    }
}
