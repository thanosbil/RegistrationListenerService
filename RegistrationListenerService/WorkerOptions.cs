using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService {

    /// <summary>
    /// Configuration settings for the worker service class
    /// </summary>
    public class WorkerOptions {

        /// <summary>
        /// The time in milliseconds that the execution cycle gets delayed for
        /// </summary>
        public int LoopCycleDelayMilliseconds { get; set; }
    }
}
