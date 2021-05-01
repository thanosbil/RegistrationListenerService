using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Interfaces {

    /// <summary>
    /// Interface to expose configuration properties for RabbitMQ
    /// </summary>
    public interface IRabbitMQ_Configuration {

        /// <summary>
        /// The endpoint for the messaging service
        /// </summary>
        string Endpoint { get; set; }

        string ExchangeName { get; set; }

        /// <summary>
        /// The name of the queue to check for messages
        /// </summary>
        string QueueName { get; set; }

        /// <summary>
        /// The routing key of the queue
        /// </summary>
        string RoutingKey { get; set; }
    }
}
