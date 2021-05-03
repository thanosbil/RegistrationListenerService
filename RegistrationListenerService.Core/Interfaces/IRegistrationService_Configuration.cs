using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Interfaces {

    /// <summary>
    /// Interface to expose configuration properties for the RegistrationConsumeService
    /// </summary>
    public interface IRegistrationService_Configuration {

        /// <summary>
        /// The endpoint for the messaging service
        /// </summary>
        string MessageBrokerEndpoint { get; set; }

        /// <summary>
        /// The name of the exchange
        /// </summary>
        string ExchangeName { get; set; }

        /// <summary>
        /// The name of the queue to check for messages
        /// </summary>
        string QueueName { get; set; }

        /// <summary>
        /// The routing key of the queue
        /// </summary>
        string RoutingKey { get; set; }

        /// <summary>
        /// The path where the file will be stored
        /// </summary>
        string FileOutputPath { get; set; }

        /// <summary>
        /// The file name
        /// </summary>
        string FileOutputName { get; set; }

        /// <summary>
        /// The endpoint for the first service
        /// </summary>
        string PostEndpoint1 { get; set; }

        /// <summary>
        /// The endpoint for the second service
        /// </summary>
        string PostEndpoint2 { get; set; }

        /// <summary>
        /// The mode of operation for data storage
        /// </summary>
        PersistenceMode DataPersistenceMode { get; set; }
    }
}
