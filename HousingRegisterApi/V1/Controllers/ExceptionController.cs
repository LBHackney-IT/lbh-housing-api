using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/exception")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ExceptionController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public ExceptionController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        [Route("handle")]
        public object HandlerException()
        {
            var context = _contextAccessor.HttpContext;

            var exHandler = context.Features.Get<IExceptionHandlerFeature>();

            Exception ex = exHandler?.Error;
            if (ex != null)
            {

                if (ex is HousingRegisterException)
                {
                    context.Response.StatusCode = 400;
                    return new { message = ex.Message, type = ex.GetType().Name };
                }
                else
                {
                    context.Response.StatusCode = 500;
                    return new { message = $"An unexpected problem occurred. CorrelationId:{this.GetCorrelationId()}", type = ex.GetType().Name };

                }
            }
            else
            {
                return Ok();
            }
        }
    }
}
