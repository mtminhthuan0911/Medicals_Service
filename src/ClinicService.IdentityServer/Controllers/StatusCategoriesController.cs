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
    [Route("api/status-categories")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StatusCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<StatusCategoriesController> _logger;

        public StatusCategoriesController(ApplicationDbContext context, IMapper mapper, ILogger<StatusCategoriesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/status-categories
        [HttpGet]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CATEGORY_STATUS_CATEGORY, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _context.StatusCategories.ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<IEnumerable<StatusCategory>, IEnumerable<StatusCategoryViewModel>>(models));
        }

        // GET: api/status-categories/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CATEGORY_STATUS_CATEGORY, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _context.StatusCategories
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

            return Ok(new Pagination<StatusCategoryViewModel>
            {
                Items = _mapper.Map<IEnumerable<StatusCategory>, IEnumerable<StatusCategoryViewModel>>(models),
                TotalRecords = await _context.StatusCategories.CountAsync()
            });
        }

        // GET api/status-categories/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        [IdentityPermission(FunctionsConstant.CATEGORY_STATUS_CATEGORY, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _context.StatusCategories.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<StatusCategory, StatusCategoryViewModel>(model));
        }

        // POST api/status-categories
        [HttpPost]
        [IdentityPermission(FunctionsConstant.CATEGORY_STATUS_CATEGORY, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] StatusCategoryRequestModel requestModel)
        {
            var model = _mapper.Map<StatusCategoryRequestModel, StatusCategory>(requestModel);

            await _context.StatusCategories.AddAsync(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // PUT api/status-categories/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.CATEGORY_STATUS_CATEGORY, CommandsConstant.UPDATE)]
        public async Task<IActionResult> Put(string id, [FromBody] StatusCategoryRequestModel requestModel)
        {
            var model = await _context.StatusCategories.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            _mapper.Map(requestModel, model);

            _context.StatusCategories.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // DELETE api/status-categories/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.CATEGORY_STATUS_CATEGORY, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await _context.StatusCategories.FindAsync(id);

            if (model == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            _context.StatusCategories.Remove(model);

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
