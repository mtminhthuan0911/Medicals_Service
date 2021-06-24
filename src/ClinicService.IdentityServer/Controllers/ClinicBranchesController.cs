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
    [Route("api/clinic-branches")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClinicBranchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ClinicBranchesController> _logger;

        public ClinicBranchesController(ApplicationDbContext context, IMapper mapper, ILogger<ClinicBranchesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/clinic-branches
        [HttpGet]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CONTENT_CLINIC_BRANCH, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _context.ClinicBranches.ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<IEnumerable<ClinicBranch>, IEnumerable<ClinicBranchViewModel>>(models));
        }

        // GET: api/clinic-branches/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.CONTENT_CLINIC_BRANCH, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _context.ClinicBranches
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
                models = models.Where(w => w.Name.Contains(q) || w.Address.Contains(q) || w.PhoneNumber.Contains(q)).ToList();

            return Ok(new Pagination<ClinicBranchViewModel>
            {
                Items = _mapper.Map<IEnumerable<ClinicBranch>, IEnumerable<ClinicBranchViewModel>>(models),
                TotalRecords = await _context.ClinicBranches.CountAsync()
            });
        }

        // GET api/clinic-branches/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_CLINIC_BRANCH, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(int id)
        {
            var model = await _context.ClinicBranches.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<ClinicBranch, ClinicBranchViewModel>(model));
        }

        // POST api/clinic-branches
        [HttpPost]
        [IdentityPermission(FunctionsConstant.CONTENT_CLINIC_BRANCH, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] ClinicBranchRequestModel requestModel)
        {
            var model = _mapper.Map<ClinicBranchRequestModel, ClinicBranch>(requestModel);

            await _context.ClinicBranches.AddAsync(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/clinic-branches/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_CLINIC_BRANCH, CommandsConstant.UPDATE)]
        public async Task<IActionResult> Put(int id, [FromBody] ClinicBranchRequestModel requestModel)
        {
            var model = await _context.ClinicBranches.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = string.Format(MessagesConstant.RECORD_NOT_FOUND, "Chi nhánh này")
                });

            _mapper.Map(requestModel, model);

            _context.ClinicBranches.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // DELETE api/clinic-branches/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.CONTENT_CLINIC_BRANCH, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.ClinicBranches.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = string.Format(MessagesConstant.RECORD_NOT_FOUND, "Chi nhánh này")
                });

            _context.ClinicBranches.Remove(model);

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
