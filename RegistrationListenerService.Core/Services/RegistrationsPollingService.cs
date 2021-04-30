using Microsoft.Extensions.Logging;
using RegistrationListenerService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Services {
    public class RegistrationsPollingService : IRegistrationPollingService {
        private readonly ILogger<RegistrationsPollingService> _logger;

        public RegistrationsPollingService(ILogger<RegistrationsPollingService> logger) {
            this._logger = logger;
        }

        public Task<string> ExecuteAsync() {
            _logger.LogInformation("this is a log message from the RegistrationService!");
            return Task.FromResult("aaaa");
        }
    }
}
