using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using KiBoards.Models;
using KiBoards.Services;

namespace KiBoards.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IHealthService _healthService;
        private readonly IMapper _mapper;

        public HealthController(ILogger<HealthController> logger, IHealthService healthService, IMapper mapper)
        {
            _logger = logger;
            _healthService = healthService;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetHealthInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HealthInfoDto>> GetHealthInfoAsync()
        {
            try
            {
                _logger.LogDebug("Getting health info");
                var healthInfo = await _healthService.GetHealthInfoAsync(Request);
                return Ok(_mapper.Map<HealthInfoDto>(healthInfo));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}