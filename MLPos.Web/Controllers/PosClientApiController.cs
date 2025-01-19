using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;

namespace MLPos.Web.Controllers
{
    [Route("api/PosClient")]
    [ApiController]
    public class PosClientApiController : ControllerBase
    {
        private readonly IPosClientService _posClientService;

        public PosClientApiController(IPosClientService posClientService)
        {
            _posClientService = posClientService;
        }

        [HttpGet("{loginCode}")]
        public async Task<IActionResult> GetPosClientByLoginCode(string loginCode)
        {
            return Ok(await _posClientService.GetPosClientByLoginCodeAsync(loginCode));
        }
    }
}
