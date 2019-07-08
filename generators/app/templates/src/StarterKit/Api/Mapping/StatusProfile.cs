using AutoMapper;

// mapping profiles within the current assembly will be picked up
// due to the statement "services.AddAutoMapper(--current assembly--)" in Startup
namespace StarterKit.Api.Mapping
{  
  public class StatusProfile : Profile
  {
    public StatusProfile()
    {
      CreateMap<Business.Monitoring.Monitoring, Api.Models.Monitoring>();
      CreateMap<Business.Monitoring.Component, Api.Models.Component>();
    }
  }
}
