using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Fullerenes.Server.Objects.Services;
using Newtonsoft.Json.Linq;

namespace Fullerenes.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class MainController(
        ICreateService createService, IDataBaseService dataBaseService, IFileService fileService,
        SystemAbstractFactoryCreator factoryCreator) 
        : ControllerBase
    {
        [HttpPost("create-fullerenes-and-limited-area/{series}/{fullereneNumber}")]
        public IActionResult CreateFullerenesAndLimitedArea(
            [FromBody] CreateFullerenesAndLimitedAreaRequest request, 
            [FromRoute] int series, [FromRoute] int fullereneNumber)
        {
            var factory = 
                factoryCreator
                .CreateSystemFactory(request, fullereneNumber);

            var result = 
                createService
                .GenerateArea(factory, series);

            var resultJSON = 
                new JsonResult(
                    new { 
                        GenId = result.id, 
                        SuperIds = result.superIds 
                    }
                );

            return new OkObjectResult(resultJSON);
        }

        [HttpGet("get-series-of-generation/{superId}")]
        public IActionResult GetGenerationSeries([FromRoute] int superId)
        {
            var path = 
                dataBaseService
                .GetDataPath(superId);

            var result = 
                fileService
                .ReadMainInfo(path);

            var resultJSON =
                new JsonResult(
                    new
                    {
                        MainInfo = result
                    }
                );

            return new OkObjectResult(resultJSON);
        }

        [HttpGet("get-phis-of-generation-series/{phis}/{superId}")]
        public IActionResult GetPhis([FromRoute] int phis, [FromRoute] int superId)
        {
            var path = 
                dataBaseService
                .GetDataPath(superId);

            var result = 
                createService
                .GeneratePhis(path, numberOfLayers: phis, numberOfPoints: 1_000_000);

            var resultJSON =
                new JsonResult(
                    new
                    {
                        Phis = result.Result
                    }
                );

            return new OkObjectResult(resultJSON);
        }

        [HttpGet("get-avg-phi-generations")]
        public IActionResult GetGenerationsAvgPhis()
        {
            var result = 
                dataBaseService
                .GetAvgPhiGroups();

            var resultJSON =
                new JsonResult(
                    new
                    {
                        AvgPhi = result
                    }
                );

            return new OkObjectResult(resultJSON);
        }

        [HttpGet("get-intens-opt-series/{qMin}/{qMax}/{qNum}/{superId}")]
        public IActionResult GetIntensOpt([FromRoute] float qMin, [FromRoute] float qMax, [FromRoute] int qNum, [FromRoute] int superId)
        {
            var path =
                dataBaseService
                .GetDataPath(superId);

            var result = 
                createService
                .GenerateIntensOpt(path, qMin, qMax, qNum);

            var resultJSON =
                new JsonResult(
                    new
                    {
                        q = result.q,
                        I = result.I
                    }
                );

            return new OkObjectResult(resultJSON);
        }
    }
}
