using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.Data;
using ClinicService.IdentityServer.Data.Entities;
using ClinicService.IdentityServer.Filters;
using ClinicService.IdentityServer.Models;
using ClinicService.IdentityServer.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicService.IdentityServer.Controllers
{
    [Route("api/appointments")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(ApplicationDbContext context, IMapper mapper, ILogger<AppointmentsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/appointments
        [HttpGet]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_APPOINTMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var viewModels = await (from a in _context.Appointments
                                    join u in _context.Users on a.PatientId equals u.Id into tblPatients
                                    from u in tblPatients.DefaultIfEmpty()

                                    join sc in _context.StatusCategories on a.StatusCategoryId equals sc.Id

                                    join ms in _context.MedicalServices on a.MedicalServiceId equals ms.Id into tblMedicalServices
                                    from ms in tblMedicalServices.DefaultIfEmpty()

                                    join cb in _context.ClinicBranches on a.ClinicBranchId equals cb.Id into tblClinicBranches
                                    from cb in tblClinicBranches.DefaultIfEmpty()

                                    join p in _context.Payments on a.Id equals p.AppointmentId into tblPayments
                                    from p in tblPayments.DefaultIfEmpty()

                                    join psc in _context.StatusCategories on p.StatusCategoryId equals psc.Id into tblPaymentStatusCategories
                                    from psc in tblPaymentStatusCategories.DefaultIfEmpty()

                                    join pm in _context.PaymentMethods on p.PaymentMethodId equals pm.Id into tblPaymentMethods
                                    from pm in tblPaymentMethods.DefaultIfEmpty()
                                    orderby a.CreatedDate descending
                                    select new AppointmentViewModel
                                    {
                                        Id = a.Id,
                                        AppointmentDate = a.AppointmentDate,
                                        CreatedDate = a.CreatedDate,
                                        MedicalServiceId = a.MedicalServiceId,
                                        MedicalServiceTitle = ms.Title,
                                        ModifiedDate = a.ModifiedDate,
                                        Note = a.Note,
                                        PatientId = a.PatientId,
                                        PatientFullName = $"{u.LastName} {u.FirstName}",
                                        StatusCategoryId = a.StatusCategoryId,
                                        StatusCategoryName = sc.Name,
                                        ClinicBranchId = a.ClinicBranchId,
                                        ClinicBranchName = cb.Name,
                                        PaymentMethodName = pm.Name,
                                        PaymentStatusCategoryName = psc.Name,
                                        GuessFullName = a.GuessFullName,
                                        GuessPhoneNumber = a.GuessPhoneNumber
                                    }).ToListAsync();

            if (viewModels == null || viewModels.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModels);
        }

        // GET: api/appointments/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_APPOINTMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var viewModels = await (from a in _context.Appointments
                                    join u in _context.Users on a.PatientId equals u.Id into tblPatients
                                    from u in tblPatients.DefaultIfEmpty()

                                    join sc in _context.StatusCategories on a.StatusCategoryId equals sc.Id

                                    join ms in _context.MedicalServices on a.MedicalServiceId equals ms.Id into tblMedicalServices
                                    from ms in tblMedicalServices.DefaultIfEmpty()

                                    join cb in _context.ClinicBranches on a.ClinicBranchId equals cb.Id into tblClinicBranches
                                    from cb in tblClinicBranches.DefaultIfEmpty()

                                    join p in _context.Payments on a.Id equals p.AppointmentId into tblPayments
                                    from p in tblPayments.DefaultIfEmpty()

                                    join psc in _context.StatusCategories on p.StatusCategoryId equals psc.Id into tblPaymentStatusCategories
                                    from psc in tblPaymentStatusCategories.DefaultIfEmpty()

                                    join pm in _context.PaymentMethods on p.PaymentMethodId equals pm.Id into tblPaymentMethods
                                    from pm in tblPaymentMethods.DefaultIfEmpty()
                                    orderby a.CreatedDate descending
                                    select new AppointmentViewModel
                                    {
                                        Id = a.Id,
                                        AppointmentDate = a.AppointmentDate,
                                        CreatedDate = a.CreatedDate,
                                        MedicalServiceId = a.MedicalServiceId,
                                        MedicalServiceTitle = ms.Title,
                                        ModifiedDate = a.ModifiedDate,
                                        Note = a.Note,
                                        PatientId = a.PatientId,
                                        PatientFullName = $"{u.LastName} {u.FirstName}",
                                        StatusCategoryId = a.StatusCategoryId,
                                        StatusCategoryName = sc.Name,
                                        ClinicBranchId = a.ClinicBranchId,
                                        ClinicBranchName = cb.Name,
                                        PaymentMethodName = pm.Name,
                                        PaymentStatusCategoryName = psc.Name,
                                        GuessFullName = a.GuessFullName,
                                        GuessPhoneNumber = a.GuessPhoneNumber
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
                viewModels = viewModels.Where(w => (!string.IsNullOrEmpty(w.Note)?w.Note.Contains(q):false) || (!string.IsNullOrEmpty(w.MedicalServiceTitle) ? w.MedicalServiceTitle.Contains(q) : false) || (!string.IsNullOrEmpty(w.PatientFullName)?w.PatientFullName.Contains(q):false)).ToList();

           


            return Ok(new Pagination<AppointmentViewModel>
            {
                Items = viewModels,
                TotalRecords = await _context.Appointments.CountAsync()
            });
        }

        // GET api/appointments/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_APPOINTMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(int id)
        {
            var viewModel = await (from a in _context.Appointments
                                   join u in _context.Users on a.PatientId equals u.Id into tblPatients
                                   from u in tblPatients.DefaultIfEmpty()

                                   join sc in _context.StatusCategories on a.StatusCategoryId equals sc.Id

                                   join ms in _context.MedicalServices on a.MedicalServiceId equals ms.Id into tblMedicalServices
                                   from ms in tblMedicalServices.DefaultIfEmpty()

                                   join cb in _context.ClinicBranches on a.ClinicBranchId equals cb.Id into tblClinicBranches
                                   from cb in tblClinicBranches.DefaultIfEmpty()

                                   join p in _context.Payments on a.Id equals p.AppointmentId into tblPayments
                                   from p in tblPayments.DefaultIfEmpty()

                                   join psc in _context.StatusCategories on p.StatusCategoryId equals psc.Id into tblPaymentStatusCategories
                                   from psc in tblPaymentStatusCategories.DefaultIfEmpty()

                                   join pm in _context.PaymentMethods on p.PaymentMethodId equals pm.Id into tblPaymentMethods
                                   from pm in tblPaymentMethods.DefaultIfEmpty()
                                   where a.Id == id
                                   select new AppointmentViewModel
                                   {
                                       Id = a.Id,
                                       AppointmentDate = a.AppointmentDate,
                                       CreatedDate = a.CreatedDate,
                                       MedicalServiceId = a.MedicalServiceId,
                                       MedicalServiceTitle = ms.Title,
                                       ModifiedDate = a.ModifiedDate,
                                       Note = a.Note,
                                       PatientId = a.PatientId,
                                       PatientFullName = $"{u.LastName} {u.FirstName}",
                                       StatusCategoryId = a.StatusCategoryId,
                                       StatusCategoryName = sc.Name,
                                       ClinicBranchId = a.ClinicBranchId,
                                       ClinicBranchName = cb.Name,
                                       PaymentMethodName = pm.Name,
                                       PaymentStatusCategoryName = psc.Name,
                                       GuessFullName = a.GuessFullName,
                                       GuessPhoneNumber = a.GuessPhoneNumber
                                   }).FirstOrDefaultAsync();

            if (viewModel == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModel);
        }

        // POST api/appointments
        [HttpPost]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_APPOINTMENT, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] AppointmentRequestModel requestModel)
        {
            var model = _mapper.Map<AppointmentRequestModel, Appointment>(requestModel);
            model.CreatedDate = DateTime.Now;

            await _context.Appointments.AddAsync(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/appointments/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_APPOINTMENT, CommandsConstant.UPDATE)]
        public async Task<IActionResult> Put(int id, [FromBody] AppointmentRequestModel requestModel)
        {
            var model = await _context.Appointments.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            _mapper.Map(requestModel, model);

            model.ModifiedDate = DateTime.Now;

            _context.Appointments.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/appointments/{id}/change-status/{statusCategoryId}
        [HttpPut("{id}/change-status/{statusCategoryId}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_APPOINTMENT, CommandsConstant.UPDATE)]
        public async Task<IActionResult> PutStatusCategory(int id, string statusCategoryId)
        {
            var model = await _context.Appointments.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            model.StatusCategoryId = statusCategoryId;
            model.ModifiedDate = DateTime.Now;

            _context.Appointments.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/appointments/{id}/update-patient-id/{patientId}
        [HttpPut("{id}/update-patient-id/{patientId}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_APPOINTMENT, CommandsConstant.UPDATE)]
        public async Task<IActionResult> PutPatientId(int id, string patientId)
        {
            var model = await _context.Appointments.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            model.PatientId = patientId;
            model.ModifiedDate = DateTime.Now;

            _context.Appointments.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // DELETE api/appointments/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_APPOINTMENT, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Appointments.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            _context.Appointments.Remove(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        #region Appointment via Payments actions
        // POST api/appointments/payments
        [HttpPost("{payments}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_APPOINTMENT, CommandsConstant.CREATE)]
        public async Task<IActionResult> PostWithPayment([FromBody] AppointmentPaymentRequestModel requestModel)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var appointmentModel = _mapper.Map<AppointmentPaymentRequestModel, Appointment>(requestModel);
                appointmentModel.CreatedDate = DateTime.Now;

                await _context.Appointments.AddAsync(appointmentModel);

                var result = await _context.SaveChangesAsync();
                if(result > 0)
                {
                    var paymentModel = new Payment
                    {
                        AppointmentId = appointmentModel.Id,
                        CreatedDate = appointmentModel.CreatedDate,
                        PaymentMethodId = requestModel.PaymentMethodId,
                        StatusCategoryId = requestModel.PaymentStatusCategoryId
                    };

                    await _context.Payments.AddAsync(paymentModel);

                    var paymentResult = await _context.SaveChangesAsync();
                    if(paymentResult > 0)
                    {
                        await transaction.CommitAsync();
                        return CreatedAtAction(nameof(GetById), new { id = appointmentModel.Id }, requestModel);
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
        #endregion Appointment via Payments actions

        #region Appointment via Users actions
        // GET: api/appointments/patients/{patientId}
        [HttpGet("patients/{patientId}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_APPOINTMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllByPatientId(string patientId)
        {
            var viewModels = await (from a in _context.Appointments

                                    join u in _context.Users on a.PatientId equals u.Id into tblPatients
                                    from u in tblPatients.DefaultIfEmpty()

                                    join sc in _context.StatusCategories on a.StatusCategoryId equals sc.Id

                                    join ms in _context.MedicalServices on a.MedicalServiceId equals ms.Id into tblMedicalServices
                                    from ms in tblMedicalServices.DefaultIfEmpty()

                                    join cb in _context.ClinicBranches on a.ClinicBranchId equals cb.Id into tblClinicBranches
                                    from cb in tblClinicBranches.DefaultIfEmpty()

                                    join p in _context.Payments on a.Id equals p.AppointmentId into tblPayments
                                    from p in tblPayments.DefaultIfEmpty()

                                    join psc in _context.StatusCategories on p.StatusCategoryId equals psc.Id into tblPaymentStatusCategories
                                    from psc in tblPaymentStatusCategories.DefaultIfEmpty()

                                    join pm in _context.PaymentMethods on p.PaymentMethodId equals pm.Id into tblPaymentMethods
                                    from pm in tblPaymentMethods.DefaultIfEmpty()
                                    orderby a.CreatedDate descending
                                    where a.PatientId == patientId
                                    select new AppointmentViewModel
                                    {
                                        Id = a.Id,
                                        AppointmentDate = a.AppointmentDate,
                                        CreatedDate = a.CreatedDate,
                                        MedicalServiceId = a.MedicalServiceId,
                                        MedicalServiceTitle = ms.Title,
                                        ModifiedDate = a.ModifiedDate,
                                        Note = a.Note,
                                        PatientId = a.PatientId,
                                        PatientFullName = $"{u.LastName} {u.FirstName}",
                                        StatusCategoryId = a.StatusCategoryId,
                                        StatusCategoryName = sc.Name,
                                        ClinicBranchId = a.ClinicBranchId,
                                        ClinicBranchName = cb.Name,
                                        PaymentMethodName = pm.Name,
                                        PaymentStatusCategoryName = psc.Name,
                                        GuessFullName = a.GuessFullName,
                                        GuessPhoneNumber = a.GuessPhoneNumber
                                    }).ToListAsync();

            if (viewModels == null || viewModels.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModels);
        }
        #endregion Appointment via Users actions
    }
}
