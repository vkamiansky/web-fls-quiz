using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebFlsQuiz.Controllers
{
    public class ConfigurationController : Controller
    {
        private static DateTime _lastRequestTime;
        private static TimeSpan _timeBetweenRequests = new TimeSpan(0, 1, 0);

        [HttpPost]
        public string SetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return "Error: token was not sent";

            var now = DateTime.Now;
            if (_lastRequestTime == null)
            {
                _lastRequestTime = now;
                // Set token
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
    }
}
