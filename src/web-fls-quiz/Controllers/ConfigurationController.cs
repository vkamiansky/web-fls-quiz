using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using WebFlsQuiz.Interfaces;

namespace WebFlsQuiz.Controllers
{
    public class ConfigurationController : Controller
    {
        private static DateTime _lastRequestTime;

        private static TimeSpan _timeBetweenRequests = new TimeSpan(0, 1, 0);

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
        public string SetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return "Error: token was not sent";

            var now = DateTime.Now;
            if (_lastRequestTime == null)
            {
                _lastRequestTime = now;
                if (_configurationService.CheckToken(token))
                {
                    var confirmCode = _configurationService.SetToken(token);
                    _mailService.SendConfirmCode(confirmCode);
                }
                return "";
            }
            else
            {
                if (now - _lastRequestTime > _timeBetweenRequests)
                {
                    // Set token
                    return "";
                }
                else
                {
                    // User has to wait
                    return "";
                }
            }
        }

        [HttpPost]
        public string ConfirmConfig(string confirmCode)
        {
            if (string.IsNullOrEmpty(confirmCode))
                return "Error: confirmation code was not sent";


        }
    }
}
