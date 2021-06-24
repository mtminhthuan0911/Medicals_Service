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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicService.IdentityServer.Controllers
{
    [Route("api/specialties")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private readonly ILogger<SpecialtiesController> _logger;

        public SpecialtiesController(ApplicationDbContext context, IStorageService storageService, IMapper mapper, ILogger<SpecialtiesController> logger)
        {
            _context = context;
            _storageService = storageService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/specialties
        [HttpGet]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CONTENT_SPECIALTY, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _context.Specialties.ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<IEnumerable<Specialty>, IEnumerable<SpecialtyViewModel>>(models));
        }

        // GET: api/specialties/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CONTENT_SPECIALTY, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _context.Specialties
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
                models = models.Where(w => w.Id.Contains(q) || w.Name.Contains(q) || w.ParentId.Contains(q) || w.Description.Contains(q) || w.SeoAlias.Contains(q) || w.Description.Contains(q)).ToList();

            return Ok(new Pagination<SpecialtyViewModel>
            {
                Items = _mapper.Map<IEnumerable<Specialty>, IEnumerable<SpecialtyViewModel>>(models),
                TotalRecords = await _context.Specialties.CountAsync()
            });
        }

        // GET api/specialties/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CONTENT_SPECIALTY, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _context.Specialties.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            var viewModel = _mapper.Map<Specialty, SpecialtyViewModel>(model);
            viewModel.Attachments = await _context.SpecialtyAttachments.Where(w => w.SpecialtyId == id).ToListAsync();

            return Ok(viewModel);
        }

        // POST api/specialties
        [HttpPost]
        [IdentityPermission(FunctionsConstant.CONTENT_SPECIALTY, CommandsConstant.CREATE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post([FromForm] SpecialtyRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = _mapper.Map<SpecialtyRequestModel, Specialty>(requestModel);

                await _context.Specialties.AddAsync(model);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    if (requestModel.Attachments != null)
                    {
                        var attachmentModels = new List<SpecialtyAttachment>();
                        foreach (var file in requestModel.Attachments)
                        {
                            await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
                            attachmentModels.Add(new SpecialtyAttachment
                            {
                                FileName = file.FileName,
                                FilePath = _storageService.GetFileUrl(file.FileName),
                                FileSize = file.Length,
                                FileType = Path.GetExtension(_storageService.GetFileUrl(file.FileName)),
                                SpecialtyId = model.Id
                            });
                        }

                        await _context.SpecialtyAttachments.AddRangeAsync(attachmentModels);
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

        // PUT api/specialties/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_SPECIALTY, CommandsConstant.UPDATE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Put(string id, [FromForm] SpecialtyRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = await _context.Specialties.FindAsync(id);

                if (model == null)
                    return NotFound(new ErrorMessageModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = MessagesConstant.DEFAULT_NOT_FOUND
                    });

                _mapper.Map(requestModel, model);

                _context.Specialties.Update(model);

                if (requestModel.Attachments != null)
                {
                    var attachmentModels = new List<SpecialtyAttachment>();
                    foreach (var file in requestModel.Attachments)
                    {
                        await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
                        attachmentModels.Add(new SpecialtyAttachment
                        {
                            FileName = file.FileName,
                            FilePath = _storageService.GetFileUrl(file.FileName),
                            FileSize = file.Length,
                            FileType = Path.GetExtension(_storageService.GetFileUrl(file.FileName)),
                            SpecialtyId = id
                        });
                    }

                    await _context.SpecialtyAttachments.AddRangeAsync(attachmentModels);
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

        // DELETE api/specialties/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_SPECIALTY, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(string id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = await _context.Specialties.FindAsync(id);

                if (model == null)
                    return NotFound(new ErrorMessageModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = MessagesConstant.DEFAULT_NOT_FOUND
                    });

                _context.Specialties.Remove(model);

                var attachmentModels = await _context.SpecialtyAttachments.Where(w => w.SpecialtyId == id).ToListAsync();
                if (attachmentModels != null)
                {
                    foreach (var file in attachmentModels)
                        await _storageService.DeleteFileAsync(file.FileName);

                    _context.SpecialtyAttachments.RemoveRange(attachmentModels);
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

        #region Specialty via Attachments actions
        //DELETE api/specialties/{specialtyId}/attachments/{id}
        [HttpDelete("{specialtyId}/attachments/{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_SPECIALTY, CommandsConstant.DELETE)]
        public async Task<IActionResult> DeleteAttachment(string specialtyId, int id)
        {
            var attachmentModel = await _context.SpecialtyAttachments.FindAsync(id);
            if (attachmentModel == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            await _storageService.DeleteFileAsync(attachmentModel.FileName);

            _context.SpecialtyAttachments.Remove(attachmentModel);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }
        #endregion Specialty via Attachments actions
    }
}
