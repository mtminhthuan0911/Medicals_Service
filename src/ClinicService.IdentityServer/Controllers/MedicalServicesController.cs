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
    [Route("api/medical-services")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MedicalServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicalServicesController> _logger;

        public MedicalServicesController(ApplicationDbContext context, IStorageService storageService, IMapper mapper, ILogger<MedicalServicesController> logger)
        {
            _context = context;
            _storageService = storageService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/medical-services
        [HttpGet]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CONTENT_MEDICAL_SERVICE, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _context.MedicalServices.ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<IEnumerable<MedicalService>, IEnumerable<MedicalServiceViewModel>>(models));
        }

        // GET: api/medical-services/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CONTENT_MEDICAL_SERVICE, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _context.MedicalServices
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
                models = models.Where(w => w.Title.Contains(q) || w.Description.Contains(q)).ToList();

            var viewModels = _mapper.Map<IEnumerable<MedicalService>, IEnumerable<MedicalServiceViewModel>>(models);
            foreach(var item in viewModels)
            {
                item.Attachments = await _context.MedicalServiceAttachments.Where(w => w.MedicalServiceId == item.Id).ToListAsync();
            }

            return Ok(new Pagination<MedicalServiceViewModel>
            {
                Items = viewModels,
                TotalRecords = await _context.ClinicBranches.CountAsync()
            });
        }

        // GET api/medical-services/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CONTENT_MEDICAL_SERVICE, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(int id)
        {
            var model = await _context.MedicalServices.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            var viewModel = _mapper.Map<MedicalService, MedicalServiceViewModel>(model);
            viewModel.Attachments = await _context.MedicalServiceAttachments.Where(w => w.MedicalServiceId == id).ToListAsync();

            return Ok(viewModel);
        }

        // POST api/medical-services
        [HttpPost]
        [IdentityPermission(FunctionsConstant.CONTENT_MEDICAL_SERVICE, CommandsConstant.CREATE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post([FromForm] MedicalServiceRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = _mapper.Map<MedicalServiceRequestModel, MedicalService>(requestModel);
                model.CreatedDate = DateTime.Now;

                await _context.MedicalServices.AddAsync(model);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    if (requestModel.Attachments != null)
                    {
                        var attachmentModels = new List<MedicalServiceAttachment>();
                        foreach (var file in requestModel.Attachments)
                        {
                            await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
                            attachmentModels.Add(new MedicalServiceAttachment
                            {
                                FileName = file.FileName,
                                FilePath = _storageService.GetFileUrl(file.FileName),
                                FileSize = file.Length,
                                FileType = Path.GetExtension(_storageService.GetFileUrl(file.FileName)),
                                MedicalServiceId = model.Id
                            });
                        }

                        await _context.MedicalServiceAttachments.AddRangeAsync(attachmentModels);
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

        // PUT api/medical-services/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_MEDICAL_SERVICE, CommandsConstant.UPDATE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Put(int id, [FromForm] MedicalServiceRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = await _context.MedicalServices.FindAsync(id);
                model.ModifiedDate = DateTime.Now;

                if (model == null)
                    return NotFound(new ErrorMessageModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = MessagesConstant.DEFAULT_NOT_FOUND
                    });

                _mapper.Map(requestModel, model);

                _context.MedicalServices.Update(model);

                if (requestModel.Attachments != null)
                {
                    var attachmentModels = new List<MedicalServiceAttachment>();
                    foreach (var file in requestModel.Attachments)
                    {
                        await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
                        attachmentModels.Add(new MedicalServiceAttachment
                        {
                            FileName = file.FileName,
                            FilePath = _storageService.GetFileUrl(file.FileName),
                            FileSize = file.Length,
                            FileType = Path.GetExtension(_storageService.GetFileUrl(file.FileName)),
                            MedicalServiceId = id
                        });
                    }

                    await _context.MedicalServiceAttachments.AddRangeAsync(attachmentModels);
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

        // DELETE api/medical-services/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_MEDICAL_SERVICE, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = await _context.MedicalServices.FindAsync(id);

                if (model == null)
                    return NotFound(new ErrorMessageModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = MessagesConstant.DEFAULT_NOT_FOUND
                    });

                _context.MedicalServices.Remove(model);

                var attachmentModels = await _context.MedicalServiceAttachments.Where(w => w.MedicalServiceId == id).ToListAsync();
                if (attachmentModels != null)
                {
                    foreach (var file in attachmentModels)
                        await _storageService.DeleteFileAsync(file.FileName);

                    _context.MedicalServiceAttachments.RemoveRange(attachmentModels);
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

        #region Medical Service via Types actions
        // GET: api/medical-services/types/{medicalServiceTypeId}
        [HttpGet("types/{medicalServiceTypeId}")]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CONTENT_MEDICAL_SERVICE, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllByTypeId(string medicalServiceTypeId)
        {
            var models = await _context.MedicalServices.Where(w => w.MedicalServiceTypeId == medicalServiceTypeId).ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            var viewModels = _mapper.Map<IEnumerable<MedicalService>, IEnumerable<MedicalServiceViewModel>>(models);
            foreach (var item in viewModels)
            {
                item.Attachments = await _context.MedicalServiceAttachments.Where(w => w.MedicalServiceId == item.Id).ToListAsync();
            }

            return Ok(viewModels);
        }
        #endregion Medical Service via Types actions

        #region Medical Service via Attachments actions
        //DELETE api/medical-services/{medicalServiceId}/attachments/{id}
        [HttpDelete("{medicalServiceId}/attachments/{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_MEDICAL_SERVICE, CommandsConstant.DELETE)]
        public async Task<IActionResult> DeleteAttachment(string medicalServiceId, int id)
        {
            var attachmentModel = await _context.MedicalServiceAttachments.FindAsync(id);
            if (attachmentModel == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            await _storageService.DeleteFileAsync(attachmentModel.FileName);

            _context.MedicalServiceAttachments.Remove(attachmentModel);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }
        #endregion Medical Service via Attachments actions
    }
}
