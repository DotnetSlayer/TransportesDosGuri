using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TransportesDosGuri.API.Controllers
{
    [Route("api/v{version:apiVersion}/[Controller]")]
    [ApiController]
    public class CustomControllerBase : Controller
    {
        protected bool TryGetCurrentUserId(out long userId)
        {
            userId = 0;

            var claim =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            return long.TryParse(claim, out userId);
        }
    }
}
