using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MLPos.Web.Controllers
{
    [Route("api/Localization")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        private IStringLocalizer<PosResources> _localizer;

        public LocalizationController(IStringLocalizer<PosResources> localizer)
        {
            _localizer = localizer;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetLocalizedStrings([FromQuery] string culture)
        {
            
            if (culture == null)
            {
                return BadRequest();
            }

            var strings = _localizer.GetAllStrings();
            IDictionary<string, string> stringDictionary = new Dictionary<string, string>();

            foreach (LocalizedString localizedString in strings)
            {
                if (!stringDictionary.ContainsKey(localizedString.Name))
                {
                    stringDictionary[localizedString.Name] = localizedString.Value;
                }
            }

            return Ok(stringDictionary);
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetLocalizedStrings(string key, [FromQuery] string culture)
        {

            if (culture == null)
            {
                return BadRequest();
            }

            string localized = _localizer[key];
            if (localized == null)
            {
                return NotFound();
            }

            return Ok(new Dictionary<string, string> { [key] = localized });
        }
    }
}
