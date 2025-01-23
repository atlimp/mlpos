using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;

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
            PosClient client = await _posClientService.GetPosClientByLoginCodeAsync(loginCode);

            if (!client.VisibleOnPos)
            {
                return NotFound();
            }

            return Ok(client);
        }
    }
}
