using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace HousingRegisterApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class AuthApiController : BaseController
    {
        private readonly ICreateAuthUseCase _createAuthUseCase;
        private readonly IVerifyAuthUseCase _verifyAuthUseCase;

        public AuthApiController(
            ICreateAuthUseCase createAuthUseCase,
            IVerifyAuthUseCase verifyAuthUseCase)
        {
            _createAuthUseCase = createAuthUseCase;
            _verifyAuthUseCase = verifyAuthUseCase;
        }

        /// <summary>
        /// Generate a verify code for an existing application
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Application not found</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("{id}/generate")]
        public IActionResult GenerateCode([FromRoute][Required] Guid id, [FromBody] CreateAuthRequest applicationRequest)
        {
            var result = _createAuthUseCase.Execute(id, applicationRequest);
            if (result == null) return NotFound(id);

            return Ok(result);
        }

        /// <summary>
        /// Confirm a verify code for an existing application
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Application not found</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("{id}/verify")]
        public IActionResult VerifyCode([FromRoute][Required] Guid id, [FromBody] VerifyAuthRequest applicationRequest)
        {
            var result = _verifyAuthUseCase.Execute(id, applicationRequest);
            if (result == null) return NotFound(id);

            return Ok(result);
        }
    }
}
