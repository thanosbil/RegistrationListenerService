using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RegistrationListenerService.Core.DataAccess;
using RegistrationListenerService.Core.Helpers;
using RegistrationListenerService.Core.Interfaces;
using RegistrationListenerService.Core.Mappings;
using RegistrationListenerService.Core.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Services {
    public class RegistrationsConsumeService : IRegistrationConsumeService {
        private readonly ILogger<RegistrationsConsumeService> _logger;
        private readonly IDbContextFactory<RegistrationsDBContext> _dbContextFactory;
        private readonly IRepostingService _repostingService;
        private readonly IMapper _mapper;

        private IRegistrationService_Configuration _registrationService_Configuration;
        private IConnection _connection;
        private IModel _channel;

        public RegistrationsConsumeService(ILogger<RegistrationsConsumeService> logger, 
            IDbContextFactory<RegistrationsDBContext> dbContextFactory,
            IRepostingService repostingService) {

            this._logger = logger;
            this._dbContextFactory = dbContextFactory;
            this._repostingService = repostingService;
            MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this._mapper = configuration.CreateMapper();
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
                _logger.LogError($"RegistrationConsumeService.Start(): threw an exception. {ex}");
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
                
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"New registration message received: {message}");

                RegistrationMessage messageRecord = new RegistrationMessage {
                    MessagePayload = message,
                    ReceivedDateTime = DateTime.Now
                };

                RegistrationMessageRepost messageRepost = _mapper.Map<RegistrationMessageRepost>(messageRecord);

                if(!await SaveToDataBase(messageRecord)) {
                    _logger.LogError($"RegistrationConsumeService.Consumer_MessageReceived(): failed to save to DataBase");
                }                
                
                FileHelper.WriteToCSVFile(_registrationService_Configuration.FileOutputPath,
                                          _registrationService_Configuration.FileOutputName,
                                          messageRecord);
                
                await RepostRegistrationMessage(messageRepost);
            }
            catch(Exception ex) {
                _logger.LogError($"RegistrationConsumeService.Consumer_MessageReceived(): threw an exception. {ex}");
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
                _logger.LogError($"RegistrationConsumeService.StopAndDispose(): threw an exception. {ex}");
            }            
        }        

        /// <summary>
        /// Creates a short lived DbContext and saves a RegistrationMessage to the Database
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<bool> SaveToDataBase(RegistrationMessage message) {            
            try {
                using var context = _dbContextFactory.CreateDbContext();            
                context.Add<RegistrationMessage>(message);
                return await context.SaveChangesAsync() > 0;             
                
            }
            catch(Exception ex) {
                _logger.LogError($"RegistrationConsumeService.SaveToDataBase(): threw an exception. {ex}");
            }

            return false;
        }

        /// <summary>
        /// Utilizes the RepostingService to repost a RegistrationMessageRepost instance to the provided endpoints
        /// </summary>
        /// <param name="messageRepost"></param>
        /// <returns></returns>
        private async Task RepostRegistrationMessage(RegistrationMessageRepost messageRepost) {
            
            if (!String.IsNullOrEmpty(_registrationService_Configuration.PostEndpoint1)) {
                await _repostingService.RepostToEndpoint(messageRepost, _registrationService_Configuration.PostEndpoint1);
            }

            if (!String.IsNullOrEmpty(_registrationService_Configuration.PostEndpoint2)) {
                await _repostingService.RepostToEndpoint(messageRepost, _registrationService_Configuration.PostEndpoint2);
            }            
        }
    }
}
