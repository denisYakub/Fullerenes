using AutoMapper;
using Fullerenes.Server.CustomLogger;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;

namespace Fullerenes.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MainController(ICreateService createService, SystemAbstractFactoryCreator factoryCreator) : ControllerBase
    {
        [HttpGet("get-message-from-server")]
        public IActionResult Get()
        {
            return new OkObjectResult("user.Name");
        }
        [AllowAnonymous]
        [HttpPost("create-fullerenes-and-limited-area/{series}/{fullereneNumber}")]
        public IActionResult CreateFullerenesAndLimitedArea(
            [FromBody] CreateFullerenesAndLimitedAreaRequest request, [FromRoute] int series, [FromRoute] int fullereneNumber)
        {
            if (!request.IsCorrectRequest())
                return new BadRequestObjectResult("Something wrong with your request!");

            SystemAbstractFactory factory = factoryCreator.CreateSystemFactory(request, series, fullereneNumber);

            var result = createService.GenerateArea(factory);

            return new OkObjectResult(result);
        }

    }
}
