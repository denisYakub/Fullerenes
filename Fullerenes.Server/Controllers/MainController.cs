using AutoMapper;
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
            using(MiniProfiler.Current.Step("Общий процесс запроса"))
            {
                if (!request.IsCorrectRequest())
                    return new BadRequestObjectResult("Something wrong with your request!");

                var factory = factoryService.GetFactory(areaType, fullereneType, request);

                Task<int> result = createService.GenerateAreaAsync(factory);

                return new OkObjectResult(result.Result);
            }
        }
        [AllowAnonymous]
        [HttpPost("create-density-of-fullerenes-in-layers/{areaId}/{seriesFs}/{numberOfLayers}/{numberOfDots}/{excess}")]
        public IActionResult CreateDensityOfFullerenesInLayers([FromRoute] int areaId, [FromRoute] int seriesFs,
            [FromRoute] int numberOfLayers, [FromRoute] int numberOfDots, [FromRoute] double excess)
        {
            var result = createService.GenerateDensityAsync(areaId, seriesFs, numberOfLayers, numberOfDots);

            return new OkObjectResult(result.Result);
        }
        [AllowAnonymous]
        [HttpPost("run-tests-on-fullerenes-collision-in-limited-area/{areaType}/{fullereneType}")]
        public IActionResult RunTestsOnFullerenesCollision([FromRoute] AreaTypes areaType, [FromRoute] FullereneTypes fullereneType,
            [FromBody] LimitedAreaWithFullerenesRequest request)
        {
            if (!request.IsCorrectRequest())
                return new BadRequestObjectResult("Something wrong with your request!");

            var mappedFs = mapper.Map<List<Fullerene>>(request.Fullerenes);

            var result = testService.CheckFullerenesIntersectionAsync(mappedFs, request.Center,
                request.Parameters[0]);

            return new OkObjectResult(result.Result);
        }
        [AllowAnonymous]
        [HttpGet("get-fullerenes-and-limited-area/{areaId}")]
        public IActionResult GetFullerenesAndLimitedArea([FromRoute] int areaId)
        {
            var result = dataBaseService.GetAreaWithFullerenesAsync(areaId);

            return new OkObjectResult(result.Result);
        }
    }
}
