using Churchee.Module.Site.Features.HtmlEditor.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Churchee.Module.Site.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public UploadController(IMediator mediator, ILogger<UploadController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("/upload/image")]
        public async Task<IActionResult> Image(IFormFile file)
        {
            try
            {
                var command = new UploadImageCommand(file);

                string url = await _mediator.Send(command);

                return Ok(new { Url = url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");

                return StatusCode(500, "Error uploading image");
            }
        }

    }
}
