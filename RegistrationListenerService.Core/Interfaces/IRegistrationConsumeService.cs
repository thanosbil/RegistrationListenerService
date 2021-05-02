using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Interfaces {
    public interface IRegistrationConsumeService {
        void Start(string endpoint);
        Task ExecuteAsync(IRegistrationService_Configuration registrationService_Configuration);
        void StopAndDispose();
    }
}
