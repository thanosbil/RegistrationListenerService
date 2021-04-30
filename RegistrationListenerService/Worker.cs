using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RegistrationListenerService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RegistrationListenerService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly IRegistrationPollingService _pollingService;

        public Worker(ILogger<Worker> logger, IRegistrationPollingService pollingService) {
            _logger = logger;
            _pollingService = pollingService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _pollingService.ExecuteAsync();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
