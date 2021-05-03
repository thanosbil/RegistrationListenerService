using AutoMapper;
using RegistrationListenerService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Mappings {
    public class MappingProfile : Profile {
        public MappingProfile() {
            CreateMap<RegistrationMessage, RegistrationMessageRepost>();
        }
    }
}
