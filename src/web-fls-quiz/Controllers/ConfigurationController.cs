using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationService _configurationService;
        private readonly IMailService _mailService;
        private readonly ILogger _logger;
        public ConfigurationController(
            IConfigurationService configurationService,
            IMailService mailService,
            ILoggerFactory loggerFactory)
        {
            _configurationService = configurationService;
            _mailService = mailService;
            _logger = loggerFactory.CreateLogger("Configuration");
        }
        [HttpPost]
        public async Task<IActionResult> Change([FromBody]Configuration newConfiguration)
        {
            return (await _configurationService.ProcessConfigurationChangeRequest(
                newConfiguration.IP,
                newConfiguration.Port,
                newConfiguration.Token))
                .Bind(data => _mailService.SendConfirmCode(data))
                .WithLogging(_logger)
                .ToApiResult();
        }

        [HttpGet]
        public IActionResult Confirm(string confirmCode)
        {
            return _configurationService.ConfirmConfigurationChange(confirmCode)
                .WithLogging(_logger)
                .ToApiResult();
        }
    }
}
