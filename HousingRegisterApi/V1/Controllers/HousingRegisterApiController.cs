using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HousingRegisterApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/housing")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class HousingRegisterApiController : BaseController
    {
        private readonly IGetAllUseCase _getAllUseCase;
        private readonly IGetByIdUseCase _getByIdUseCase;
        public HousingRegisterApiController(IGetAllUseCase getAllUseCase, IGetByIdUseCase getByIdUseCase)
        {
            _getAllUseCase = getAllUseCase;
            _getByIdUseCase = getByIdUseCase;
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-base-api/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObjectList), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult ListContacts()
        {
            return Ok(_getAllUseCase.Execute());
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="404">No ? found for the specified ID</response>
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("{id}")]
        public IActionResult ViewRecord(int id)
        {
            return Ok(_getByIdUseCase.Execute(id));
        }
    }
}
