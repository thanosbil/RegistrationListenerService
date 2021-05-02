using RegistrationListenerService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService {
    public class RegistrationService_Configuration : IRegistrationService_Configuration {

        /// <summary>
        /// The endpoint for the messaging service
        /// </summary>
        public string MessageBrokerEndpoint { get; set; }

        /// <summary>
        /// The name of the exchange
        /// </summary>
        public string ExchangeName{ get; set; }

        /// <summary>
        /// The name of the queue to check for messages
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// The routing key of the queue
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// The path where the file will be stored
        /// </summary>
        public string FileOutputPath { get; set; }

        /// <summary>
        /// The file name
        /// </summary>
        public string FileOutputName { get; set; }

        /// <summary>
        /// The endpoint for the first service
        /// </summary>
        public string PostEndpoint1 { get; set; }

        /// <summary>
        /// The endpoint for the second service
        /// </summary>
        public string PostEndpoint2 { get; set; }
    }
}
