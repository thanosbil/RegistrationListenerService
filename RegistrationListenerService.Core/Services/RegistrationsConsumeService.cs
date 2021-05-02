using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RegistrationListenerService.Core.DataAccess;
using RegistrationListenerService.Core.Helpers;
using RegistrationListenerService.Core.Interfaces;
using RegistrationListenerService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Services {
    public class RegistrationsConsumeService : IRegistrationConsumeService {
        private readonly ILogger<RegistrationsConsumeService> _logger;
        private readonly IDbContextFactory<RegistrationsDBContext> _dbContextFactory;
        private IRegistrationService_Configuration _registrationService_Configuration;
        private IConnection _connection;
        private IModel _channel;

        public RegistrationsConsumeService(ILogger<RegistrationsConsumeService> logger, 
            IDbContextFactory<RegistrationsDBContext> dbContextFactory) {

            this._logger = logger;
            this._dbContextFactory = dbContextFactory;            
        }

        /// <summary>
        /// Initialize a reusable connection using the provided endpoint url when the service starts
        /// </summary>
        /// <param name="endpoint"></param>
        public void Start(string endpoint) {
            try {
                var factory = new ConnectionFactory { Uri = new Uri(endpoint) };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex) {
                _logger.LogError($"RegistrationConsumeService.Start(): threw an exception {ex}");
            }
        }

        /// <summary>
        /// Sets up the channel and subscribes to the Received event 
        /// </summary>
        /// <param name="rabbitMQ_Configuration"></param>
        /// <returns></returns>
        public Task ExecuteAsync(IRegistrationService_Configuration registrationService_Configuration) {

            try {
                this._registrationService_Configuration = registrationService_Configuration;

                _channel = _connection.CreateModel();                
                _channel.ExchangeDeclare(_registrationService_Configuration.ExchangeName, ExchangeType.Direct, durable: true);
                _channel.QueueDeclare(_registrationService_Configuration.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.QueueBind(_registrationService_Configuration.QueueName,
                    _registrationService_Configuration.ExchangeName,
                    _registrationService_Configuration.RoutingKey,
                    null);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += Consumer_MessageReceived;
                
                _channel.BasicConsume(queue: _registrationService_Configuration.QueueName, autoAck: true, consumer: consumer);
            }
            catch (Exception ex) {
                _logger.LogError($"RegistrationConsumeService.ExecuteAsync(): threw an exception. {ex}");                
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the event when a new message is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Consumer_MessageReceived(object sender, BasicDeliverEventArgs e) {
            try {
                var body = e.Body.ToArray();
                using var context = _dbContextFactory.CreateDbContext();
                var message = Encoding.UTF8.GetString(body);
                
                _logger.LogInformation($"New registration message received: {message}");

                RegistrationMessage messageRecord = new RegistrationMessage {
                    MessagePayload = message,
                    ReceivedDateTime = DateTime.Now
                };

                context.Add<RegistrationMessage>(messageRecord);
                await context.SaveChangesAsync();
                FileHelper.WriteToCSVFile(_registrationService_Configuration.FileOutputPath,
                                          _registrationService_Configuration.FileOutputName,
                                          messageRecord);
            }
            catch(Exception ex) {
                _logger.LogError($"RegistrationConsumeService.Consumer_MessageReceived(): threw an exception {ex}");
            }            
        }

        /// <summary>
        /// Closes the channel and the connection when the service stops
        /// </summary>
        public void StopAndDispose() {
            try {
                _channel.Close();
                _connection.Close();
                _connection.Dispose();
            }
            catch(Exception ex) {
                _logger.LogError($"RegistrationConsumeService.StopAndDispose(): threw an exception {ex}");
            }            
        }        
    }
}
