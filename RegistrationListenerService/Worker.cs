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
        private readonly IRegistrationConsumeService _consumeService;
        private readonly RabbitMQ_Configuration _rabbitMQ_Configuration;

        public Worker(ILogger<Worker> logger, IRegistrationConsumeService consumeService, 
            RabbitMQ_Configuration rabbitMQ_Configuration) {

            this._logger = logger;
            this._consumeService = consumeService;
            this._rabbitMQ_Configuration = rabbitMQ_Configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken) {
            _consumeService.Start(_rabbitMQ_Configuration.Endpoint);
            _logger.LogInformation("Worker service has started");            
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await _consumeService.ExecuteAsync(_rabbitMQ_Configuration);
        }
        
        public override Task StopAsync(CancellationToken cancellationToken) {
            _consumeService.StopAndDispose();
            _logger.LogInformation("Worker service has stopped");
            return base.StopAsync(cancellationToken);
        }
    }
}
