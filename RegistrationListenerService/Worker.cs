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
        private readonly WorkerOptions _workerOptions;

        public Worker(ILogger<Worker> logger, IRegistrationPollingService pollingService, WorkerOptions workerOptions) {
            this._logger = logger;
            this._pollingService = pollingService;
            this._workerOptions = workerOptions;
        }

        public override Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Worker service has started");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                _logger.LogInformation("============================>Worker: Start of main execution loop<============================");
                await _pollingService.ExecuteAsync();
                await Task.Delay(_workerOptions.LoopCycleDelayMilliseconds, stoppingToken);                
            }
        }
        
        public override Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Worker service has stopped");
            return base.StopAsync(cancellationToken);
        }
    }
}
