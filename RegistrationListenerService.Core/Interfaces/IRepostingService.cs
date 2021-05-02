using RegistrationListenerService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Interfaces {
    public interface IRepostingService {
        Task<bool> RepostToEndpoint(RegistrationMessageRepost message, string endpointUrl);        
    }
}
