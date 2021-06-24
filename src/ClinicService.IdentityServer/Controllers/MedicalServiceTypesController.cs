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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicService.IdentityServer.Controllers
{
    [Route("api/medical-service-types")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MedicalServiceTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicalServiceTypesController> _logger;

        public MedicalServiceTypesController(ApplicationDbContext context, IMapper mapper, ILogger<MedicalServiceTypesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/medical-service-types
        [HttpGet]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CATEGORY_MEDICAL_SERVICE_TYPE, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _context.MedicalServiceTypes.ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<IEnumerable<MedicalServiceType>, IEnumerable<MedicalServiceTypeViewModel>>(models));
        }

        // GET: api/medical-service-types/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.CATEGORY_MEDICAL_SERVICE_TYPE, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _context.MedicalServiceTypes
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
                models = models.Where(w => w.Name.Contains(q)).ToList();

            return Ok(new Pagination<MedicalServiceTypeViewModel>
            {
                Items = _mapper.Map<IEnumerable<MedicalServiceType>, IEnumerable<MedicalServiceTypeViewModel>>(models),
                TotalRecords = await _context.MedicalServiceTypes.CountAsync()
            });
        }

        // GET api/medical-service-types/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.CATEGORY_MEDICAL_SERVICE_TYPE, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _context.MedicalServiceTypes.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<MedicalServiceType, MedicalServiceTypeViewModel>(model));
        }

        // POST api/medical-service-types
        [HttpPost]
        [IdentityPermission(FunctionsConstant.CATEGORY_MEDICAL_SERVICE_TYPE, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] MedicalServiceTypeRequestModel requestModel)
        {
            var model = _mapper.Map<MedicalServiceTypeRequestModel, MedicalServiceType>(requestModel);

            await _context.MedicalServiceTypes.AddAsync(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/medical-service-types/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.CATEGORY_MEDICAL_SERVICE_TYPE, CommandsConstant.UPDATE)]
        public async Task<IActionResult> Put(string id, [FromBody] MedicalServiceTypeRequestModel requestModel)
        {
            var model = await _context.MedicalServiceTypes.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = string.Format(MessagesConstant.RECORD_NOT_FOUND, "Loại dịch vụ này")
                });

            _mapper.Map(requestModel, model);

            _context.MedicalServiceTypes.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // DELETE api/medical-service-types/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.CATEGORY_MEDICAL_SERVICE_TYPE, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await _context.MedicalServiceTypes.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = string.Format(MessagesConstant.RECORD_NOT_FOUND, "Loại dịch vụ này")
                });

            _context.MedicalServiceTypes.Remove(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }
    }
}
