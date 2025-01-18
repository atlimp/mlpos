using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MLPos.Web.Controllers
{
    [Route("api/Localization")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        private IStringLocalizer<SharedResources> _localizer;

        public LocalizationController(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        [HttpGet]
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
    }
}
