using Microsoft.EntityFrameworkCore;
using RegistrationListenerService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.DataAccess {
    public class RegistrationsDBContext : DbContext {
        public RegistrationsDBContext(DbContextOptions<RegistrationsDBContext> options) : base (options) { }
        public DbSet<RegistrationMessage> RegistrationMessages { get; set; }
    }
}
