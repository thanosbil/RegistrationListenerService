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
            CreateMap<RegistrationMessage, RegistrationMessageRepost>()
                .ForMember(dest => dest.PersistenceTime, options =>
                    options.MapFrom(src => DateTime.Now.Subtract(src.ReceivedDateTime)));
        }
    }
}
