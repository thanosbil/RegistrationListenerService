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
        private readonly RegistrationService_Configuration _registrationService_Configuration;
        
        public Worker(ILogger<Worker> logger, IRegistrationConsumeService consumeService, 
            RegistrationService_Configuration registrationService_Configuration) {

            this._logger = logger;
            this._consumeService = consumeService;
            this._registrationService_Configuration = registrationService_Configuration;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// Calls the Start method of the injected service to initialize the connection.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken) {
            _consumeService.Start(_registrationService_Configuration.MessageBrokerEndpoint);
            _logger.LogInformation("Worker service has started.");            
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Calls the ExecuteAsync method of the injected service, to set up the channel and subscribe to 
        /// the Received Event.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await _consumeService.ExecuteAsync(_registrationService_Configuration);
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// Calls the StopAndDispose method of the injected service to close the channel and the connection.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken) {
            _consumeService.StopAndDispose();
            _logger.LogInformation("Worker service has stopped.");
            return base.StopAsync(cancellationToken);
        }
    }
}
