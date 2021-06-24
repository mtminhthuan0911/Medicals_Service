using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.Data;
using ClinicService.IdentityServer.Data.Entities;
using ClinicService.IdentityServer.Filters;
using ClinicService.IdentityServer.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicService.IdentityServer.Controllers
{
    [Route("api/commands")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommandsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CommandsController> _logger;

        public CommandsController(ApplicationDbContext context, IMapper mapper, ILogger<CommandsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/commands
        [HttpGet]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _context.Commands.ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<Command>, IEnumerable<CommandViewModel>>(models));
        }

        // GET: api/commands/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _context.Commands
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound();

            if (!string.IsNullOrEmpty(q))
                models = models.Where(w => w.Id.Contains(q) || w.Name.Contains(q)).ToList();

            return Ok(_mapper.Map<IEnumerable<Command>, IEnumerable<CommandViewModel>>(models));
        }

        // GET api/commands/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _context.Commands.FindAsync(id);

            if (model == null)
                return NotFound();

            return Ok(_mapper.Map<Command, CommandViewModel>(model));
        }

        // POST api/commands
        [HttpPost]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] CommandRequestModel requestModel)
        {
            var model = _mapper.Map<CommandRequestModel, Command>(requestModel);

            await _context.Commands.AddAsync(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);

            return BadRequest();
        }

        // PUT api/commands/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.UPDATE)]
        public async Task<IActionResult> Put(string id, [FromBody] CommandRequestModel requestModel)
        {
            var model = await _context.Commands.FindAsync(id);

            if (model == null)
                return NotFound();

            _mapper.Map(requestModel, model);

            _context.Commands.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest();
        }

        // DELETE api/commands/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await _context.Commands.FindAsync(id);

            if (model == null)
                return NotFound();

            _context.Commands.Remove(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest();
        }
    }
}
