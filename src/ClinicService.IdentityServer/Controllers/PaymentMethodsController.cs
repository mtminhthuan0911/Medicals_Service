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
    [Route("api/payment-methods")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentMethodsController> _logger;

        public PaymentMethodsController(ApplicationDbContext context, IMapper mapper, ILogger<PaymentMethodsController> logger, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/payment-methods
        [HttpGet]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CATEGORY_PAYMENT_METHOD, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _context.PaymentMethods.ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            var viewModels = _mapper.Map<IEnumerable<PaymentMethod>, IEnumerable<PaymentMethodViewModel>>(models);
            foreach(var viewModel in viewModels)
            {
                viewModel.Attachments = await _context.PaymentMethodAttachments.Where(w => w.PaymentMethodId == viewModel.Id).ToListAsync();
            }

            return Ok(viewModels);
        }

        // GET: api/payment-methods/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.CATEGORY_PAYMENT_METHOD, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _context.PaymentMethods
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
                models = models.Where(w => w.Id.Contains(q) || w.Name.Contains(q)).ToList();

            return Ok(new Pagination<PaymentMethodViewModel>
            {
                Items = _mapper.Map<IEnumerable<PaymentMethod>, IEnumerable<PaymentMethodViewModel>>(models),
                TotalRecords = await _context.PaymentMethods.CountAsync()
            });
        }

        // GET api/payment-methods/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.CATEGORY_PAYMENT_METHOD, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _context.PaymentMethods.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            var viewModel = _mapper.Map<PaymentMethod, PaymentMethodViewModel>(model);
            viewModel.Attachments = await _context.PaymentMethodAttachments.Where(w => w.PaymentMethodId == id).ToListAsync();

            return Ok(viewModel);
        }

        // POST api/payment-methods
        [HttpPost]
        [IdentityPermission(FunctionsConstant.CATEGORY_PAYMENT_METHOD, CommandsConstant.CREATE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post([FromForm] PaymentMethodRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = _mapper.Map<PaymentMethodRequestModel, PaymentMethod>(requestModel);

                await _context.PaymentMethods.AddAsync(model);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    if (requestModel.Attachments != null)
                    {
                        var attachmentModels = new List<PaymentMethodAttachment>();
                        foreach (var file in requestModel.Attachments)
                        {
                            await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
                            attachmentModels.Add(new PaymentMethodAttachment
                            {
                                FileName = file.FileName,
                                FilePath = _storageService.GetFileUrl(file.FileName),
                                FileSize = file.Length,
                                FileType = Path.GetExtension(_storageService.GetFileUrl(file.FileName)),
                                PaymentMethodId = model.Id
                            });
                        }

                        await _context.PaymentMethodAttachments.AddRangeAsync(attachmentModels);
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

        // PUT api/payment-methods/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.CATEGORY_PAYMENT_METHOD, CommandsConstant.UPDATE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Put(string id, [FromForm] PaymentMethodRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = await _context.PaymentMethods.FindAsync(id);

                if (model == null)
                    return NotFound(new ErrorMessageModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = MessagesConstant.DEFAULT_NOT_FOUND
                    });

                _mapper.Map(requestModel, model);

                _context.PaymentMethods.Update(model);

                if (requestModel.Attachments != null)
                {
                    var attachmentModels = new List<PaymentMethodAttachment>();
                    foreach (var file in requestModel.Attachments)
                    {
                        await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
                        attachmentModels.Add(new PaymentMethodAttachment
                        {
                            FileName = file.FileName,
                            FilePath = _storageService.GetFileUrl(file.FileName),
                            FileSize = file.Length,
                            FileType = Path.GetExtension(_storageService.GetFileUrl(file.FileName)),
                            PaymentMethodId = id
                        });
                    }

                    await _context.PaymentMethodAttachments.AddRangeAsync(attachmentModels);
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

        // DELETE api/payment-methods/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.CATEGORY_PAYMENT_METHOD, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(string id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = await _context.PaymentMethods.FindAsync(id);

                if (model == null)
                    return NotFound(new ErrorMessageModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = MessagesConstant.DEFAULT_NOT_FOUND
                    });

                _context.PaymentMethods.Remove(model);

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

        #region Payment Method via Attachments actions
        //DELETE api/website-sections/{websiteSectionId}/attachments/{id}
        [HttpDelete("{paymentMethodId}/attachments/{id}")]
        [IdentityPermission(FunctionsConstant.CATEGORY_PAYMENT_METHOD, CommandsConstant.DELETE)]
        public async Task<IActionResult> DeleteAttachment(string paymentMethodId, int id)
        {
            var attachmentModel = await _context.PaymentMethodAttachments.FindAsync(id);
            if (attachmentModel == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            await _storageService.DeleteFileAsync(attachmentModel.FileName);

            _context.PaymentMethodAttachments.Remove(attachmentModel);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }
        #endregion Payment Method via Attachments actions
    }
}
