using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/reporting")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ReportingApiController : BaseController
    {
        private readonly IGetNovaletExportUseCase _getNovaletExportUseCase;

        public ReportingApiController(IGetNovaletExportUseCase getNovaletCsvUseCase)
        {
            _getNovaletExportUseCase = getNovaletCsvUseCase;
        }

        /// <summary>
        /// Returns the Novalet export file
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">No record found for the specified ID</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("novaletexport")]
        public async Task<IActionResult> DownloadNovaletExport()
        {
            var result = await _getNovaletExportUseCase.Execute().ConfigureAwait(false);
            if (result == null) return NotFound();

            return File(result.Data, result.FileMimeType, result.FileName);
        }
    }
}
