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
    [Route("api/re-appointments")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReappointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ReappointmentsController> _logger;

        public ReappointmentsController(ApplicationDbContext context, IMapper mapper, ILogger<ReappointmentsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/re-appointments
        [HttpGet]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_REAPPOINTMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var viewModels = await (from a in _context.Reappointments
                                    join u in _context.Users on a.PatientId equals u.Id
                                    join sc in _context.StatusCategories on a.StatusCategoryId equals sc.Id
                                    orderby a.CreatedDate descending
                                    select new ReappointmentViewModel
                                    {
                                        Id = a.Id,
                                        ReappointmentDate = a.ReappointmentDate,
                                        CreatedDate = a.CreatedDate,
                                        ModifiedDate = a.ModifiedDate,
                                        Note = a.Note,
                                        PatientId = a.PatientId,
                                        PatientFullName = $"{u.LastName} {u.FirstName}",
                                        StatusCategoryId = a.StatusCategoryId,
                                        StatusCategoryName = sc.Name,
                                        FromMedicalExaminationId = a.FromMedicalExaminationId
                                    }).ToListAsync();

            if (viewModels == null || viewModels.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModels);
        }

        // GET: api/re-appointments/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_REAPPOINTMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var viewModels = await (from a in _context.Reappointments
                                    join u  in _context.Users on a.PatientId equals u.Id
                                    join sc in _context.StatusCategories on a.StatusCategoryId equals sc.Id
                                    orderby a.CreatedDate descending
                                    select new ReappointmentViewModel
                                    {
                                        Id = a.Id,
                                        ReappointmentDate = a.ReappointmentDate,
                                        CreatedDate = a.CreatedDate,
                                        ModifiedDate = a.ModifiedDate,
                                        Note = a.Note,
                                        PatientId = a.PatientId,
                                        PatientFullName = $"{u.LastName} {u.FirstName}",
                                        StatusCategoryId = a.StatusCategoryId,
                                        StatusCategoryName = sc.Name,
                                        FromMedicalExaminationId = a.FromMedicalExaminationId
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
                viewModels = viewModels.Where(w => w.Note.Contains(q) || w.PatientFullName.Contains(q)).ToList();

            return Ok(new Pagination<ReappointmentViewModel>
            {
                Items = viewModels,
                TotalRecords = await _context.Reappointments.CountAsync()
            });
        }

        // GET api/re-appointments/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_REAPPOINTMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(int id)
        {
            var viewModel = await (from a in _context.Reappointments
                                   join u in _context.Users on a.PatientId equals u.Id
                                   join sc in _context.StatusCategories on a.StatusCategoryId equals sc.Id
                                   where a.Id == id
                                   select new ReappointmentViewModel
                                   {
                                       Id = a.Id,
                                       ReappointmentDate = a.ReappointmentDate,
                                       CreatedDate = a.CreatedDate,
                                       ModifiedDate = a.ModifiedDate,
                                       Note = a.Note,
                                       PatientId = a.PatientId,
                                       PatientFullName = $"{u.LastName} {u.FirstName}",
                                       StatusCategoryId = a.StatusCategoryId,
                                       StatusCategoryName = sc.Name,
                                       FromMedicalExaminationId = a.FromMedicalExaminationId
                                   }).FirstOrDefaultAsync();

            if (viewModel == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModel);
        }

        // POST api/re-appointments
        [HttpPost]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_REAPPOINTMENT, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] ReappointmentRequestModel requestModel)
        {
            var model = _mapper.Map<ReappointmentRequestModel, Reappointment>(requestModel);
            model.CreatedDate = DateTime.Now;

            await _context.Reappointments.AddAsync(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/re-appointments/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_REAPPOINTMENT, CommandsConstant.UPDATE)]
        public async Task<IActionResult> Put(int id, [FromBody] ReappointmentRequestModel requestModel)
        {
            var model = await _context.Reappointments.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            _mapper.Map(requestModel, model);

            model.ModifiedDate = DateTime.Now;

            _context.Reappointments.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/re-appointments/{id}/change-status/{statusCategoryId}
        [HttpPut("{id}/change-status/{statusCategoryId}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_REAPPOINTMENT, CommandsConstant.UPDATE)]
        public async Task<IActionResult> PutStatusCategory(int id, string statusCategoryId)
        {
            var model = await _context.Reappointments.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            model.StatusCategoryId = statusCategoryId;
            model.ModifiedDate = DateTime.Now;

            _context.Reappointments.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // DELETE api/re-appointments/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_REAPPOINTMENT, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Reappointments.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            _context.Reappointments.Remove(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        #region Reappointment via Users
        // GET: api/re-appointments/patients/{patientId}
        [HttpGet("patients/{patientId}")]
        [IdentityPermission(FunctionsConstant.ADMINISTRATION_REAPPOINTMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllByPatientId(string patientId)
        {
            var viewModels = await (from a in _context.Reappointments
                                    join u in _context.Users on a.PatientId equals u.Id
                                    join sc in _context.StatusCategories on a.StatusCategoryId equals sc.Id
                                    orderby a.CreatedDate descending
                                    where a.PatientId == patientId
                                    select new ReappointmentViewModel
                                    {
                                        Id = a.Id,
                                        ReappointmentDate = a.ReappointmentDate,
                                        CreatedDate = a.CreatedDate,
                                        ModifiedDate = a.ModifiedDate,
                                        Note = a.Note,
                                        PatientId = a.PatientId,
                                        PatientFullName = $"{u.LastName} {u.FirstName}",
                                        StatusCategoryId = a.StatusCategoryId,
                                        StatusCategoryName = sc.Name,
                                        FromMedicalExaminationId = a.FromMedicalExaminationId
                                    }).ToListAsync();

            if (viewModels == null || viewModels.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(viewModels);
        }
        #endregion Reappointment via Users
    }
}
