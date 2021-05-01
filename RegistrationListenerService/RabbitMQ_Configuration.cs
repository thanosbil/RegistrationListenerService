using RegistrationListenerService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService {
    public class RabbitMQ_Configuration : IRabbitMQ_Configuration {
        public string Endpoint { get; set; }
        public string ExchangeName{ get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
    }
}
