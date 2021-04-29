using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Models {
    public class RegistrationMessage {
        
        /// <summary>
        /// File Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The message received (json format)
        /// </summary>
        public string MessagePayload { get; set; }

        /// <summary>
        /// DateTime that the message was received from queue
        /// </summary>
        public DateTime ReceivedDateTime { get; set; }
    }
}
