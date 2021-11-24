using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/reporting")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ReportingApiController : BaseController
    {
        private readonly IListNovaletExportFilesUseCase _listNovaletExportFilesUseCase;
        private readonly IGetNovaletExportUseCase _getNovaletExportUseCase;
        private readonly ICreateNovaletExportUseCase _createNovaletExportUseCase;

        public ReportingApiController(
            IListNovaletExportFilesUseCase listNovaletExportFilesUseCase,
            IGetNovaletExportUseCase getNovaletCsvUseCase,
            ICreateNovaletExportUseCase createNovaletCsvUseCase)
        {
            _listNovaletExportFilesUseCase = listNovaletExportFilesUseCase;
            _getNovaletExportUseCase = getNovaletCsvUseCase;
            _createNovaletExportUseCase = createNovaletCsvUseCase;
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
        [Route("listnovaletfiles")]
        public async Task<IActionResult> ViewNovaletExportFiles()
        {
            var result = await _listNovaletExportFilesUseCase.Execute().ConfigureAwait(false);
            return Ok(result);
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
        [Route("novaletexport/{fileName}")]
        public async Task<IActionResult> DownloadNovaletExport([FromRoute][Required] string fileName)
        {
            var result = await _getNovaletExportUseCase.Execute(fileName).ConfigureAwait(false);
            if (result == null) return NotFound();

            return File(result.Data, result.FileMimeType, result.FileName);
        }

        /// <summary>
        /// Generates/Regenerates the Novalet export file
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">No record found for the specified ID</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("generatenovaletexport")]
        public async Task<IActionResult> GenerateNovaletExport()
        {
            var result = await _createNovaletExportUseCase.Execute().ConfigureAwait(false);
            if (result == null) return BadRequest();

            return Ok();
        }
    }
}
