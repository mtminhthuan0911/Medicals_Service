using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.Models;
using ClinicService.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicService.IdentityServer.Controllers
{
    [Route("api/file-managers")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FileManagersController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;

        public FileManagersController(IStorageService storageService, IConfiguration configuration)
        {
            _storageService = storageService;
            _configuration = configuration;
        }

        [HttpPost("upload-from-editor")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadFromEditor()
        {
            var file = Request.Form.Files.FirstOrDefault();
            if (file == null)
                return BadRequest(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = string.Format(MessagesConstant.RECORD_NOT_FOUND, "Tập tin")
                });

            await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);

            var imgUrl = _configuration.GetValue<string>("IdentityServerUrl") + _storageService.GetFileUrl(file.FileName);

            return Ok(new LocationResponseModel { Location = imgUrl });
        }
    }
}
