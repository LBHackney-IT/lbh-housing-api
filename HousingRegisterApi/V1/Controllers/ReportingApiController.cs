using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        private readonly IApproveNovaletExportUseCase _approveNovaletExportUseCase;
        private readonly IGetInternalReportUseCase _getInternalReportUseCase;

        public ReportingApiController(
            IListNovaletExportFilesUseCase listNovaletExportFilesUseCase,
            IGetNovaletExportUseCase getNovaletCsvUseCase,
            ICreateNovaletExportUseCase createNovaletCsvUseCase,
            IApproveNovaletExportUseCase approveNovaletExportUseCase,
            IGetInternalReportUseCase getInternalReportUseCase)
        {
            _listNovaletExportFilesUseCase = listNovaletExportFilesUseCase;
            _getNovaletExportUseCase = getNovaletCsvUseCase;
            _createNovaletExportUseCase = createNovaletCsvUseCase;
            _approveNovaletExportUseCase = approveNovaletExportUseCase;
            _getInternalReportUseCase = getInternalReportUseCase;
        }

        /// <summary>
        /// Returns the specified export file
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">No file found for the specified filename</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("export")]
        public async Task<IActionResult> DownloadReport([FromBody][Required] InternalReportRequest request)
        {
            var result = await _getInternalReportUseCase.Execute(request).ConfigureAwait(true);
            if (result == null) return NotFound();

            return File(result.Data, result.FileMimeType, result.FileName);
        }

        /// <summary>
        /// Returns the Novalet export file
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
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
        /// <response code="404">No file found for the specified filename</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        /// <response code="404">Unable to generate export file</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("generatenovaletexport")]
        public async Task<IActionResult> GenerateNovaletExport()
        {
            var result = await _createNovaletExportUseCase.Execute().ConfigureAwait(true);
            if (result == null) return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Approves the Novalet export file for transfer
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Unable to approve export file or file doesnt not exist</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("approvenovaletexport/{fileName}")]
        public async Task<IActionResult> ApproveNovaletExport([FromRoute][Required] string fileName)
        {
            var result = await _approveNovaletExportUseCase.Execute(fileName).ConfigureAwait(false);
            if (result == false) return BadRequest();

            return Ok();
        }
    }
}
