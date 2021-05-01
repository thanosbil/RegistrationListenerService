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
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IModel _channel;

        public RegistrationsConsumeService(ILogger<RegistrationsConsumeService> logger, IServiceScopeFactory scopeFactory) {
            this._logger = logger;
            this._scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Gets the message from the queue and writes the message to the database and file
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
                
                _channel.BasicConsume(queue: rabbitMQ_Configuration.QueueName, autoAck: false, consumer: consumer);
            }
            catch (Exception ex) {
                _logger.LogError($"RegistrationPollingService.ExecuteAsync(): threw an exception. {ex}");
                // throw maybe?
            }            
        }

        private void Consumer_MessageReceived(object sender, BasicDeliverEventArgs e) {
            
            var body = e.Body.ToArray();
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RegistrationsDBContext>();

            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation($"New registration message received: {message}");
            RegistrationMessage messageRecord = new RegistrationMessage {
                MessagePayload = message,
                ReceivedDateTime = DateTime.Now
            };

            db.SaveChanges();
        }

        public void StopAndDispose() {
            _channel.Close();
            _connection.Close();            
        }

        public void Start(string endpoint) {
            //var factory = new ConnectionFactory { Uri = new Uri(endpoint) };
            var factory = new ConnectionFactory { 
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();            
         }
    }
}
