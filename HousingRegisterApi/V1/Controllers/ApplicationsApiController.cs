using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/applications")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ApplicationsApiController : BaseController
    {
        private readonly IGetAllApplicationsUseCase _getApplicationsUseCase;
        private readonly IGetApplicationByIdUseCase _getByIdUseCase;
        private readonly ICreateNewApplicationUseCase _createNewApplicationUseCase;
        private readonly IUpdateApplicationUseCase _updateApplicationUseCase;
        private readonly ICompleteApplicationUseCase _completeApplicationUseCase;
        private readonly ICreateEvidenceRequestUseCase _createEvidenceRequestUseCase;
        private readonly ICalculateBedroomsUseCase _calculateBedroomsUseCase;

        public ApplicationsApiController(
            IGetAllApplicationsUseCase getApplicationsUseCase,
            IGetApplicationByIdUseCase getByIdUseCase,
            ICreateNewApplicationUseCase createNewApplicationUseCase,
            IUpdateApplicationUseCase updateApplicationUseCase,
            ICompleteApplicationUseCase completeApplicationUseCase,
            ICreateEvidenceRequestUseCase createEvidenceRequestUseCase,
            ICalculateBedroomsUseCase calculateBedroomsUseCase)
        {
            _getApplicationsUseCase = getApplicationsUseCase;
            _getByIdUseCase = getByIdUseCase;
            _createNewApplicationUseCase = createNewApplicationUseCase;
            _updateApplicationUseCase = updateApplicationUseCase;
            _completeApplicationUseCase = completeApplicationUseCase;
            _createEvidenceRequestUseCase = createEvidenceRequestUseCase;
            _calculateBedroomsUseCase = calculateBedroomsUseCase;
        }

        /// <summary>
        /// List the applications
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid Query Parameter.</response>
        /// <response code="404">No records found for the specified query</response>
        /// <response code="500">Something went wrong</response>
        [ProducesResponseType(typeof(PaginatedApplicationListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public IActionResult ListApplications([FromQuery] SearchQueryParameter searchParameters)
        {
            var response = _getApplicationsUseCase.Execute(searchParameters);
            if (response == null || !response.Results.Any()) return NotFound(searchParameters);

            return Ok(response);
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
            if (entity == null) return NotFound(id);

            return Ok(entity);
        }

        /// <summary>
        /// Creates a new application
        /// </summary>
        /// <response code="201">Returns the application created with its ID</response>
        /// <response code="400">Invalid fields in the post parameter.</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public IActionResult CreateNewApplication([FromBody] CreateApplicationRequest applicationRequest)
        {
            var newApplication = _createNewApplicationUseCase.Execute(applicationRequest);
            return Created(new Uri($"api/v1/applications/{newApplication.Id}", UriKind.Relative), newApplication);
        }

        /// <summary>
        /// Updates an existing application
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid fields in the post parameter.</response>
        /// <response code="404">No record found for the specified ID</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        [Route("{id}")]
        public IActionResult UpdateApplication([FromRoute][Required] Guid id, [FromBody] UpdateApplicationRequest applicationRequest)
        {
            var result = _updateApplicationUseCase.Execute(id, applicationRequest);
            if (result == null) return NotFound(id);

            return Ok(result);
        }

        /// <summary>
        /// Completes an existing application
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">No record found for the specified ID</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        [Route("{id}/complete")]
        public IActionResult CompleteApplication([FromRoute][Required] Guid id)
        {
            var result = _completeApplicationUseCase.Execute(id);
            if (result == null) return NotFound(id);

            return Ok(result);
        }

        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("bedrooms")]
        public IActionResult CalculateBedrooms([FromBody] CalculateBedroomsRequest request)
        {
            var result = _calculateBedroomsUseCase.Execute(request);
            return Ok(result);
        }

        /// <summary>
        /// Create evidence requests for an application
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">No record found for the specified ID</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("{id}/evidence")]
        public async Task<IActionResult> CreateEvidenceRequestAsync([FromRoute][Required] Guid id, [FromBody] CreateEvidenceRequestBase request)
        {
            var result = await _createEvidenceRequestUseCase.ExecuteAsync(id, request).ConfigureAwait(false);
            if (result == null) return NotFound(id);

            return Ok(result);
        }
    }
}
