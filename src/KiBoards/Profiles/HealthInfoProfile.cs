using AutoMapper;

namespace KiBoards.Profiles
{
    public class HealthProfile : Profile
    {
        public HealthProfile()
        {
            CreateMap<Services.HealthInfo, Models.HealthInfoDto>();
        }
    }
}
