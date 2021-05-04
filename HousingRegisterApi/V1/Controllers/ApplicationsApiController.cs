using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HousingRegisterApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/applications")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ApplicationsApiController : BaseController
    {
        private readonly IGetAllApplicationsUseCase _getAllUseCase;
        private readonly IGetApplicationByIdUseCase _getByIdUseCase;
        public ApplicationsApiController(IGetAllApplicationsUseCase getAllUseCase, IGetApplicationByIdUseCase getByIdUseCase)
        {
            _getAllUseCase = getAllUseCase;
            _getByIdUseCase = getByIdUseCase;
        }

        /// <summary>
        /// List the applications
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid Query Parameter.</response>
        /// <response code="500">Something went wrong</response>
        [ProducesResponseType(typeof(ApplicationList), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public IActionResult ListApplications()
        {
            return Ok(_getAllUseCase.Execute());
        }

        /// <summary>
        /// Retrives the application corresponding to the supplied id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid id value supplied</response>
        /// <response code="404">No record found for the specified ID</response>
        /// <response code="500">Something went wrong</response>
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{id}")]
        public IActionResult ViewApplication(Guid id)
        {
            var entity = _getByIdUseCase.Execute(id);
            if (null == entity) return NotFound(id);

            return Ok(entity);
        }
    }
}
