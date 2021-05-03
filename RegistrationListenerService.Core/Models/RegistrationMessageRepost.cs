using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Models {
    public class RegistrationMessageRepost : RegistrationMessageBase {

        /// <summary>
        /// e.g. Database name, File storage path
        /// </summary>
        public List<string> PersistenceSystems { get; set; } = new List<string>();

        /// <summary>
        /// Timespan between message reception and repost to endpoint
        /// </summary>
        public TimeSpan PersistenceTime { get; set; }
    }
}
