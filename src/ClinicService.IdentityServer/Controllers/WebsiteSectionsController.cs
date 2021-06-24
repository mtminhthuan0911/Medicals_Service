using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.Data;
using ClinicService.IdentityServer.Data.Entities;
using ClinicService.IdentityServer.Filters;
using ClinicService.IdentityServer.Models;
using ClinicService.IdentityServer.Services;
using ClinicService.IdentityServer.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicService.IdentityServer.Controllers
{
    [Route("api/website-sections")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WebsiteSectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private readonly ILogger<WebsiteSectionsController> _logger;

        public WebsiteSectionsController(ApplicationDbContext context, IStorageService storageService, IMapper mapper, ILogger<WebsiteSectionsController> logger)
        {
            _context = context;
            _storageService = storageService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/website-sections
        [HttpGet]
        [IdentityPermission(FunctionsConstant.CONTENT_WEBSITE_SECTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _context.WebsiteSections.ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<IEnumerable<WebsiteSection>, IEnumerable<WebsiteSectionViewModel>>(models));
        }

        // GET: api/website-sections/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.CONTENT_WEBSITE_SECTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _context.WebsiteSections
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            if (!string.IsNullOrEmpty(q))
                models = models.Where(w => w.Id.Contains(q) || w.Name.Contains(q) || w.ParentId.Contains(q) || w.SeoAlias.Contains(q)).ToList();

            return Ok(new Pagination<WebsiteSectionViewModel>
            {
                Items = _mapper.Map<IEnumerable<WebsiteSection>, IEnumerable<WebsiteSectionViewModel>>(models),
                TotalRecords = await _context.ClinicBranches.CountAsync()
            });
        }

        // GET api/website-sections/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CONTENT_WEBSITE_SECTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _context.WebsiteSections.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            var viewModel = _mapper.Map<WebsiteSection, WebsiteSectionViewModel>(model);
            viewModel.Attachments = await _context.WebsiteSectionAttachments.Where(w => w.WebsiteSectionId == id).ToListAsync();

            return Ok(viewModel);
        }

        // GET api/website-sections/{parentId}/children
        [HttpGet("{parentId}/children")]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CONTENT_WEBSITE_SECTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetChildrenByParentId(string parentId)
        {
            var models = await _context.WebsiteSections.Where(w => w.ParentId == parentId).ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<IEnumerable<WebsiteSection>, IEnumerable<WebsiteSectionViewModel>>(models));
        }

        // POST api/website-sections
        [HttpPost]
        [IdentityPermission(FunctionsConstant.CONTENT_WEBSITE_SECTION, CommandsConstant.CREATE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post([FromForm] WebsiteSectionRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = _mapper.Map<WebsiteSectionRequestModel, WebsiteSection>(requestModel);
                model.CreatedDate = DateTime.Now;

                await _context.WebsiteSections.AddAsync(model);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    if (requestModel.Attachments != null)
                    {
                        var attachmentModels = new List<WebsiteSectionAttachment>();
                        foreach (var file in requestModel.Attachments)
                        {
                            await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
                            attachmentModels.Add(new WebsiteSectionAttachment
                            {
                                FileName = file.FileName,
                                FilePath = _storageService.GetFileUrl(file.FileName),
                                FileSize = file.Length,
                                FileType = Path.GetExtension(_storageService.GetFileUrl(file.FileName)),
                                WebsiteSectionId = model.Id
                            });
                        }

                        await _context.WebsiteSectionAttachments.AddRangeAsync(attachmentModels);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await transaction.RollbackAsync();
            }

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/website-sections/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_WEBSITE_SECTION, CommandsConstant.UPDATE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Put(string id, [FromForm] WebsiteSectionRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = await _context.WebsiteSections.FindAsync(id);

                if (model == null)
                    return NotFound(new ErrorMessageModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = MessagesConstant.DEFAULT_NOT_FOUND
                    });

                _mapper.Map(requestModel, model);

                model.ModifiedDate = DateTime.Now;

                _context.WebsiteSections.Update(model);

                if (requestModel.Attachments != null)
                {
                    var attachmentModels = new List<WebsiteSectionAttachment>();
                    foreach (var file in requestModel.Attachments)
                    {
                        await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
                        attachmentModels.Add(new WebsiteSectionAttachment
                        {
                            FileName = file.FileName,
                            FilePath = _storageService.GetFileUrl(file.FileName),
                            FileSize = file.Length,
                            FileType = Path.GetExtension(_storageService.GetFileUrl(file.FileName)),
                            WebsiteSectionId = id
                        });
                    }

                    await _context.WebsiteSectionAttachments.AddRangeAsync(attachmentModels);
                }

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await transaction.RollbackAsync();
            }

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // DELETE api/website-sections/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_WEBSITE_SECTION, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(string id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = await _context.WebsiteSections.FindAsync(id);

                if (model == null)
                    return NotFound(new ErrorMessageModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = MessagesConstant.DEFAULT_NOT_FOUND
                    });

                _context.WebsiteSections.Remove(model);

                var attachmentModels = await _context.WebsiteSectionAttachments.Where(w => w.WebsiteSectionId == id).ToListAsync();
                if (attachmentModels != null)
                {
                    foreach (var file in attachmentModels)
                        await _storageService.DeleteFileAsync(file.FileName);

                    _context.WebsiteSectionAttachments.RemoveRange(attachmentModels);
                }

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await transaction.RollbackAsync();
            }

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        #region Website Section via Attachments actions
        //DELETE api/website-sections/{websiteSectionId}/attachments/{id}
        [HttpDelete("{websiteSectionId}/attachments/{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_WEBSITE_SECTION, CommandsConstant.DELETE)]
        public async Task<IActionResult> DeleteAttachment(string websiteSectionId, int id)
        {
            var attachmentModel = await _context.WebsiteSectionAttachments.FindAsync(id);
            if (attachmentModel == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            await _storageService.DeleteFileAsync(attachmentModel.FileName);

            _context.WebsiteSectionAttachments.Remove(attachmentModel);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }
        #endregion Website Section via Attachments actions
    }
}
