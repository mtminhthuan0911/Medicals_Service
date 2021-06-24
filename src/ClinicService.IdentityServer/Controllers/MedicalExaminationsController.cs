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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicService.IdentityServer.Controllers
{
    [Route("api/medical-examinations")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MedicalExaminationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicalExaminationsController> _logger;

        public MedicalExaminationsController(ApplicationDbContext context, IStorageService storageService, UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<MedicalExaminationsController> logger)
        {
            _context = context;
            _storageService = storageService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/medical-examinations
        [HttpGet]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var viewModels = await (from me in _context.MedicalExaminations
                                    join u in _context.Users on me.PatientId equals u.Id
                                    join sc in _context.StatusCategories on me.StatusCategoryId equals sc.Id
                                    orderby me.CreatedDate descending
                                    select new MedicalExaminationViewModel
                                    {
                                        Id = me.Id,
                                        CreatedDate = me.CreatedDate,
                                        ModifiedDate = me.ModifiedDate,
                                        PatientId = me.PatientId,
                                        PatientFullName = $"{u.LastName} {u.FirstName}",
                                        StatusCategoryId = me.StatusCategoryId,
                                        StatusCategoryName = sc.Name,
                                        AppointmentId = me.AppointmentId,
                                        ReappointmentId = me.ReappointmentId
                                    }).ToListAsync();

            if (viewModels == null || viewModels.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModels);
        }

        // GET: api/medical-examinations/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var viewModels = await (from me in _context.MedicalExaminations
                                    join u in _context.Users on me.PatientId equals u.Id
                                    join sc in _context.StatusCategories on me.StatusCategoryId equals sc.Id
                                    orderby me.CreatedDate descending
                                    select new MedicalExaminationViewModel
                                    {
                                        Id = me.Id,
                                        CreatedDate = me.CreatedDate,
                                        ModifiedDate = me.ModifiedDate,
                                        PatientId = me.PatientId,
                                        PatientFullName = $"{u.LastName} {u.FirstName}",
                                        StatusCategoryId = me.StatusCategoryId,
                                        StatusCategoryName = sc.Name,
                                        AppointmentId = me.AppointmentId,
                                        ReappointmentId = me.ReappointmentId
                                    })
                                    .Skip((page - 1) * limit)
                                    .Take(limit)
                                    .ToListAsync();

            if (viewModels == null || viewModels.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            if (!string.IsNullOrEmpty(q))
                viewModels = viewModels.Where(w => w.PatientFullName.Contains(q)).ToList();

            return Ok(new Pagination<MedicalExaminationViewModel>
            {
                Items = viewModels,
                TotalRecords = await _context.MedicalExaminations.CountAsync()
            });
        }

        // GET api/medical-examinations/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(int id)
        {
            var viewModel = await (from me in _context.MedicalExaminations
                                   join u in _context.Users on me.PatientId equals u.Id
                                   join sc in _context.StatusCategories on me.StatusCategoryId equals sc.Id
                                   where me.Id == id
                                   select new MedicalExaminationViewModel
                                   {
                                       Id = me.Id,
                                       CreatedDate = me.CreatedDate,
                                       ModifiedDate = me.ModifiedDate,
                                       PatientId = me.PatientId,
                                       PatientFullName = $"{u.LastName} {u.FirstName}",
                                       StatusCategoryId = me.StatusCategoryId,
                                       StatusCategoryName = sc.Name,
                                       AppointmentId = me.AppointmentId,
                                       ReappointmentId = me.ReappointmentId
                                   }).FirstOrDefaultAsync();

            if (viewModel == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            var modelDetails = await _context.MedicalExaminationDetails.Where(w => w.MedicalExaminationId == id).ToListAsync();
            if (modelDetails != null && modelDetails.Count > 0)
                viewModel.Details = _mapper.Map<IEnumerable<MedicalExaminationDetail>, IEnumerable<MedicalExaminationDetailViewModel>>(modelDetails);

            var prescriptions = await _context.Prescriptions.Where(w => w.MedicalExaminationId == id).ToListAsync();
            if (prescriptions != null && prescriptions.Count > 0)
                viewModel.Prescriptions = _mapper.Map<IEnumerable<Prescription>, IEnumerable<PrescriptionViewModel>>(prescriptions);

            viewModel.Attachments = await _context.MedicalExaminationAttachments.Where(w => w.MedicalExaminationId == id).ToListAsync();

            return Ok(viewModel);
        }

        // GET api/medical-examinations/from-appointments/{appointmentId}
        [HttpGet("from-appointments/{appointmentId}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.READ)]
        public async Task<IActionResult> GetByFromAppointmentId(int appointmentId)
        {
            var viewModel = await (from me in _context.MedicalExaminations
                                   join u in _context.Users on me.PatientId equals u.Id
                                   join sc in _context.StatusCategories on me.StatusCategoryId equals sc.Id
                                   where me.AppointmentId == appointmentId
                                   select new MedicalExaminationViewModel
                                   {
                                       Id = me.Id,
                                       CreatedDate = me.CreatedDate,
                                       ModifiedDate = me.ModifiedDate,
                                       PatientId = me.PatientId,
                                       PatientFullName = $"{u.LastName} {u.FirstName}",
                                       StatusCategoryId = me.StatusCategoryId,
                                       StatusCategoryName = sc.Name,
                                       AppointmentId = me.AppointmentId,
                                       ReappointmentId = me.ReappointmentId
                                   }).FirstOrDefaultAsync();

            if (viewModel == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModel);
        }

        // GET api/medical-examinations/from-reappointments/{appointmentId}
        [HttpGet("from-reappointments/{reappointmentId}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.READ)]
        public async Task<IActionResult> GetByFromReappointmentId(int reappointmentId)
        {
            var viewModel = await (from me in _context.MedicalExaminations
                                   join u in _context.Users on me.PatientId equals u.Id
                                   join sc in _context.StatusCategories on me.StatusCategoryId equals sc.Id
                                   where me.ReappointmentId == reappointmentId
                                   select new MedicalExaminationViewModel
                                   {
                                       Id = me.Id,
                                       CreatedDate = me.CreatedDate,
                                       ModifiedDate = me.ModifiedDate,
                                       PatientId = me.PatientId,
                                       PatientFullName = $"{u.LastName} {u.FirstName}",
                                       StatusCategoryId = me.StatusCategoryId,
                                       StatusCategoryName = sc.Name,
                                       AppointmentId = me.AppointmentId,
                                       ReappointmentId = me.ReappointmentId
                                   }).FirstOrDefaultAsync();

            if (viewModel == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModel);
        }

        // POST api/medical-examinations
        [HttpPost]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] MedicalExaminationRequestModel requestModel)
        {
            var model = _mapper.Map<MedicalExaminationRequestModel, MedicalExamination>(requestModel);
            model.CreatedDate = DateTime.Now;

            await _context.MedicalExaminations.AddAsync(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // POST api/medical-examinations/full
        [HttpPost("full")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.CREATE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostFull([FromForm] MedicalExaminationFullRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Add medical examination
                var medicalExaminationModel = _mapper.Map<MedicalExamination>(requestModel);
                medicalExaminationModel.CreatedDate = DateTime.Now;
                await _context.MedicalExaminations.AddAsync(medicalExaminationModel);
                var medicalExaminationResult = await _context.SaveChangesAsync();

                // Add medical examination details
                if (medicalExaminationResult > 0)
                {
                    // Add details
                    var convertedDetails = JsonConvert.DeserializeObject<IEnumerable<MedicalExaminationDetailRequestModel>>(requestModel.DetailRequestModels);
                    var medicalExaminationDetailModels = _mapper.Map<IEnumerable<MedicalExaminationDetailRequestModel>, IEnumerable<MedicalExaminationDetail>>(convertedDetails);
                    foreach (var detail in medicalExaminationDetailModels)
                    {
                        detail.CreatedDate = DateTime.Now;
                        detail.MedicalExaminationId = medicalExaminationModel.Id;
                    }
                    await _context.MedicalExaminationDetails.AddRangeAsync(medicalExaminationDetailModels);

                    // Add prescriptions
                    var convertedPrescriptions = JsonConvert.DeserializeObject<IEnumerable<PrescriptionRequestModel>>(requestModel.PrescriptionRequestModels);
                    var prescriptionModels = _mapper.Map<IEnumerable<PrescriptionRequestModel>, IEnumerable<Prescription>>(convertedPrescriptions);
                    foreach (var pres in prescriptionModels)
                    {
                        pres.CreatedDate = DateTime.Now;
                        pres.MedicalExaminationId = medicalExaminationModel.Id;
                    }
                    await _context.Prescriptions.AddRangeAsync(prescriptionModels);

                    // Add attachments
                    if (requestModel.Attachments != null)
                    {
                        var attachmentModels = new List<MedicalExaminationAttachment>();
                        foreach (var file in requestModel.Attachments)
                        {
                            await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
                            attachmentModels.Add(new MedicalExaminationAttachment
                            {
                                FileName = file.FileName,
                                FilePath = _storageService.GetFileUrl(file.FileName),
                                FileSize = file.Length,
                                FileType = Path.GetExtension(_storageService.GetFileUrl(file.FileName)),
                                MedicalExaminationId = medicalExaminationModel.Id
                            });
                        }

                        await _context.MedicalExaminationAttachments.AddRangeAsync(attachmentModels);
                    }

                    // Update status to appointment
                    if (requestModel.AppointmentId > 0)
                    {
                        var appointmentModel = await _context.Appointments.SingleOrDefaultAsync(s => s.Id == requestModel.AppointmentId);
                        appointmentModel.StatusCategoryId = SystemsConstant.StatusCategories.ID_STATUS_COMPLETED;

                        _context.Appointments.Update(appointmentModel);
                    }
                    else if (requestModel.ReappointmentId > 0)
                    {
                        var reappointmentModel = await _context.Reappointments.SingleOrDefaultAsync(s => s.Id == requestModel.ReappointmentId);
                        reappointmentModel.StatusCategoryId = SystemsConstant.StatusCategories.ID_STATUS_COMPLETED;

                        _context.Reappointments.Update(reappointmentModel);
                    }

                    var results = await _context.SaveChangesAsync();
                    if (results > 0)
                    {
                        await transaction.CommitAsync();
                        return CreatedAtAction(nameof(GetById), new { id = medicalExaminationModel.Id }, medicalExaminationModel);
                    }
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

        // PUT api/medical-examinations/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.UPDATE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Put(int id, [FromForm] MedicalExaminationRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = await _context.MedicalExaminations.FindAsync(id);

                if (model == null)
                    return NotFound(new ErrorMessageModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = MessagesConstant.DEFAULT_NOT_FOUND
                    });

                _mapper.Map(requestModel, model);

                model.ModifiedDate = DateTime.Now;

                _context.MedicalExaminations.Update(model);

                if (requestModel.Attachments != null)
                {
                    var attachmentModels = new List<MedicalExaminationAttachment>();
                    foreach (var file in requestModel.Attachments)
                    {
                        await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
                        attachmentModels.Add(new MedicalExaminationAttachment
                        {
                            FileName = file.FileName,
                            FilePath = _storageService.GetFileUrl(file.FileName),
                            FileSize = file.Length,
                            FileType = Path.GetExtension(_storageService.GetFileUrl(file.FileName)),
                            MedicalExaminationId = id
                        });
                    }

                    await _context.MedicalExaminationAttachments.AddRangeAsync(attachmentModels);
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

        // PUT api/medical-examinations/{id}/change-status/{statusCategoryId}
        [HttpPut("{id}/change-status/{statusCategoryId}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.UPDATE)]
        public async Task<IActionResult> PutStatusCategory(int id, string statusCategoryId)
        {
            var model = await _context.MedicalExaminations.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            model.StatusCategoryId = statusCategoryId;
            model.ModifiedDate = DateTime.Now;

            _context.MedicalExaminations.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // DELETE api/medical-examinations/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var model = await _context.MedicalExaminations.FindAsync(id);

                if (model == null)
                    return NotFound(new ErrorMessageModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = MessagesConstant.DEFAULT_NOT_FOUND
                    });

                _context.MedicalExaminations.Remove(model);

                var details = await _context.MedicalExaminationDetails.Where(w => w.MedicalExaminationId == id).ToListAsync();
                if (details != null && details.Count > 0)
                    _context.MedicalExaminationDetails.RemoveRange(details);

                var prescriptions = await _context.Prescriptions.Where(w => w.MedicalExaminationId == id).ToListAsync();
                if (prescriptions != null && prescriptions.Count > 0)
                    _context.Prescriptions.RemoveRange(prescriptions);

                var attachmentModels = await _context.MedicalExaminationAttachments.Where(w => w.MedicalExaminationId == id).ToListAsync();
                if (attachmentModels != null)
                {
                    foreach (var file in attachmentModels)
                        await _storageService.DeleteFileAsync(file.FileName);

                    _context.MedicalExaminationAttachments.RemoveRange(attachmentModels);
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

        #region Medical Examination via Details actions
        // GET api/medical-examinations/{medicalExaminationId}/details
        [HttpGet("{medicalExaminationId}/details")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.READ)]
        public async Task<IActionResult> GetDetails(int medicalExaminationId)
        {
            var viewModels = await _context.MedicalExaminationDetails.Where(w => w.MedicalExaminationId == medicalExaminationId).ToListAsync();

            if (viewModels == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModels);
        }

        // POST api/medical-examinations/{medicalExaminationId}/details
        [HttpPost("{medicalExaminationId}/details")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.CREATE)]
        public async Task<IActionResult> PostDetail (int medicalExaminationId, [FromBody] MedicalExaminationDetailRequestModel requestModel)
        {
            var model = _mapper.Map<MedicalExaminationDetailRequestModel, MedicalExaminationDetail>(requestModel);
            model.MedicalExaminationId = medicalExaminationId; 
            model.CreatedDate = DateTime.Now;

            await _context.MedicalExaminationDetails.AddAsync(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), requestModel);

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/medical-examinations/{medicalExaminationId}/details/{id}
        [HttpPut("{medicalExaminationId}/details/{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.UPDATE)]
        public async Task<IActionResult> PutDetail (int medicalExaminationId, int id, [FromBody] MedicalExaminationDetailRequestModel requestModel)
        {
            var model = await _context.MedicalExaminationDetails.Where(w => w.MedicalExaminationId == medicalExaminationId && w.Id == id).FirstOrDefaultAsync();

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            _mapper.Map(requestModel, model);

            model.ModifiedDate = DateTime.Now;

            _context.MedicalExaminationDetails.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // DELETE api/medical-examinations/{medicalExaminationId}/details/{id}
        [HttpDelete("{medicalExaminationId}/details/{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.DELETE)]
        public async Task<IActionResult> DeleteDetail(int medicalExaminationId, int id)
        {
            var model = await _context.MedicalExaminationDetails.Where(w => w.MedicalExaminationId == medicalExaminationId && w.Id == id).FirstOrDefaultAsync();

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            _context.Remove(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }
        #endregion Medical Examination via Details actions

        #region Medical Examination via Prescriptions actions
        // GET api/medical-examinations/{medicalExaminationId}/prescriptions
        [HttpGet("{medicalExaminationId}/prescriptions")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.READ)]
        public async Task<IActionResult> GetPrescriptions(int medicalExaminationId)
        {
            var viewModels = await _context.Prescriptions.Where(w => w.MedicalExaminationId == medicalExaminationId).ToListAsync();

            if (viewModels == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModels);
        }

        // POST api/medical-examinations/{medicalExaminationId}/prescriptions
        [HttpPost("{medicalExaminationId}/prescriptions")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.CREATE)]
        public async Task<IActionResult> PostPrescription(int medicalExaminationId, [FromBody] PrescriptionRequestModel requestModel)
        {
            var model = _mapper.Map<PrescriptionRequestModel, Prescription>(requestModel);
            model.MedicalExaminationId = medicalExaminationId;
            model.CreatedDate = DateTime.Now;

            await _context.Prescriptions.AddAsync(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), requestModel);

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/medical-examinations/{medicalExaminationId}/prescriptions/{id}
        [HttpPut("{medicalExaminationId}/prescriptions/{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.UPDATE)]
        public async Task<IActionResult> PutPrescription(int medicalExaminationId, int id, [FromBody] PrescriptionRequestModel requestModel)
        {
            var model = await _context.Prescriptions.Where(w => w.MedicalExaminationId == medicalExaminationId && w.Id == id).FirstOrDefaultAsync();

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            _mapper.Map(requestModel, model);

            model.ModifiedDate = DateTime.Now;

            _context.Prescriptions.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // DELETE api/medical-examinations/{medicalExaminationId}/prescriptions/{id}
        [HttpDelete("{medicalExaminationId}/prescriptions/{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.DELETE)]
        public async Task<IActionResult> DeletePrescription(int medicalExaminationId, int id)
        {
            var model = await _context.Prescriptions.Where(w => w.MedicalExaminationId == medicalExaminationId && w.Id == id).FirstOrDefaultAsync();

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            _context.Remove(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }
        # endregion Medical Examination via Prescriptions actions

        #region Medical Examination via Attachments actions
        //DELETE api/medical-examinations/{medicalExaminationId}/attachments/{id}
        [HttpDelete("{medicalExaminationId}/attachments/{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_WEBSITE_SECTION, CommandsConstant.DELETE)]
        public async Task<IActionResult> DeleteAttachment(int medicalExaminationId, int id)
        {
            var attachmentModel = await _context.MedicalExaminationAttachments.FindAsync(id);
            if (attachmentModel == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            await _storageService.DeleteFileAsync(attachmentModel.FileName);

            _context.MedicalExaminationAttachments.Remove(attachmentModel);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }
        #endregion Medical Examination via Attachments actions

        #region Medical Examination via Users actions
        // GET api/medical-examinations/patients/{patientId}
        [HttpGet("patients/{patientId}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_MEDICAL_EXAMINATION, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllByPatientId(string patientId)
        {
            var viewModels = await (from me in _context.MedicalExaminations
                                    join u in _context.Users on me.PatientId equals u.Id
                                    join sc in _context.StatusCategories on me.StatusCategoryId equals sc.Id
                                    where me.PatientId == patientId
                                    select new MedicalExaminationViewModel
                                    {
                                        Id = me.Id,
                                        CreatedDate = me.CreatedDate,
                                        ModifiedDate = me.ModifiedDate,
                                        PatientId = me.PatientId,
                                        PatientFullName = $"{u.LastName} {u.FirstName}",
                                        StatusCategoryId = me.StatusCategoryId,
                                        StatusCategoryName = sc.Name,
                                        AppointmentId = me.AppointmentId,
                                        ReappointmentId = me.ReappointmentId
                                    }).ToListAsync();

            if (viewModels == null || viewModels.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModels);
        }
        #endregion Medical Examination via Users actions
    }
}
