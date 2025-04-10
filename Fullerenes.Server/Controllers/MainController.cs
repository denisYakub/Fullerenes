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
    public class MainController(
        ICreateService createService, ITestService testService,
        IDataBaseService dataBaseService, IFactoryService factoryService,
        IMapper mapper
        ) : ControllerBase
    {
        [HttpGet("get-message-from-server")]
        public IActionResult Get()
        {
            return new OkObjectResult("user.Name");
        }
        [AllowAnonymous]
        [HttpPost("create-fullerenes-and-limited-area/{areaType}/{fullereneType}")]
        public IActionResult CreateFullerenesAndLimitedArea([FromRoute] AreaTypes areaType, [FromRoute] FullereneTypes fullereneType,
            [FromBody] CreateFullerenesAndLimitedAreaRequest request)
        {
            if (!request.IsCorrectRequest())
                return new BadRequestObjectResult("Something wrong with your request!");

            FullereneAndLimitedAreaFactory factory = factoryService.GetFactory(areaType, fullereneType, request);

            var before = DateTime.Now;

            var result = createService.GenerateArea(factory);

            var after = DateTime.Now;

            Print.PrintToConsole((after - before).Minutes + " min");

            return new OkObjectResult(result);
        }
    }
}
