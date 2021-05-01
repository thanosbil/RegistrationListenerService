using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Interfaces {
    public interface IRegistrationConsumeService {
        void Start(string endpoint);
        Task ExecuteAsync(IRabbitMQ_Configuration rabbitMQ_Configuration);
        void StopAndDispose();
    }
}
