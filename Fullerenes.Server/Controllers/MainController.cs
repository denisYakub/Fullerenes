using AutoMapper;
using Fullerenes.Server.CustomLogger;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
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
        ICreateService createService, IDataBaseService dataBaseService, IFileService fileService,
        SystemAbstractFactoryCreator factoryCreator) 
        : ControllerBase
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

        [AllowAnonymous]
        [HttpGet("get-series-of-generation/{superId}")]
        public IActionResult GetGenerationSeries([FromRoute] int superId)
        {
            var path = dataBaseService.GetDataPath(superId);

            var result = fileService.ReadMainInfo(path);

            return new OkObjectResult(result);
        }

        [AllowAnonymous]
        [HttpGet("get-phis-of-generation-series/{phis}/{superId}")]
        public IActionResult GetPhis([FromRoute] int phis, [FromRoute] int superId)
        {
            var path = dataBaseService.GetDataPath(superId);

            var result = createService.GeneratePhis(path, numberOfLayers: phis);

            return new OkObjectResult(result.Result);
        }

        [AllowAnonymous]
        [HttpGet("get-avg-phi-generations")]
        public IActionResult GetGenerationsAvgPhis()
        {
            var result = dataBaseService.GetAvgPhiGroups();

            return new OkObjectResult(result);
        }
    }
}
