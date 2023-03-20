using AutoMapper;
using KiBoards.Services;

namespace KiBoards.Profiles
{
    public class HealthProfile : Profile
    {
        public HealthProfile()
        {
            CreateMap<HealthInfo, Models.HealthInfoDto>();
        }
    }
}
