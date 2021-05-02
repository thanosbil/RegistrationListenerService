using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Models {
    public abstract class RegistrationMessageBase : BaseEntity {
        
        /// <summary>
        /// The message received
        /// </summary>
        public string MessagePayload { get; set; }

        /// <summary>
        /// DateTime that the message was received from queue
        /// </summary>
        public DateTime ReceivedDateTime { get; set; }
    }
}
