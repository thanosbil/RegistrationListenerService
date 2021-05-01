using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RegistrationListenerService.Core.DataAccess;
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
        private IConnection _connection;
        private IModel _channel;

        public RegistrationsConsumeService(ILogger<RegistrationsConsumeService> logger, IDbContextFactory<RegistrationsDBContext> dbContextFactory) {
            this._logger = logger;
            this._dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Sets up the channel and subscribes to the Received event 
        /// </summary>
        /// <param name="rabbitMQ_Configuration"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(IRabbitMQ_Configuration rabbitMQ_Configuration) {

            try {
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(rabbitMQ_Configuration.ExchangeName, ExchangeType.Direct, durable: true);
                _channel.QueueDeclare(rabbitMQ_Configuration.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.QueueBind(rabbitMQ_Configuration.QueueName,
                    rabbitMQ_Configuration.ExchangeName,
                    rabbitMQ_Configuration.RoutingKey,
                    null);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += Consumer_MessageReceived;
                
                _channel.BasicConsume(queue: rabbitMQ_Configuration.QueueName, autoAck: true, consumer: consumer);
            }
            catch (Exception ex) {
                _logger.LogError($"RegistrationConsumeService.ExecuteAsync(): threw an exception. {ex}");
                // throw maybe?
            }            
        }

        /// <summary>
        /// Handles the event when a new message is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Consumer_MessageReceived(object sender, BasicDeliverEventArgs e) {
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
                context.SaveChanges();
            }
            catch(Exception ex) {
                _logger.LogError($"RegistrationConsumeService.Consumer_MessageReceived(): threw an exception {ex}");
            }            
        }

        /// <summary>
        /// Closes the channel and the connection when the service stops
        /// </summary>
        public void StopAndDispose() {
            _channel.Close();
            _connection.Close();            
        }

        /// <summary>
        /// Initialize a reusable connection using the provided endpoint url when the service starts
        /// </summary>
        /// <param name="endpoint"></param>
        public void Start(string endpoint) {
            var factory = new ConnectionFactory { Uri = new Uri(endpoint) };            
            _connection = factory.CreateConnection();            
         }
    }
}
