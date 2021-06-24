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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicService.IdentityServer.Controllers
{
    [Route("api/roles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RolesController> _logger;

        public RolesController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IMapper mapper, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/roles
        [HttpGet]
        [IdentityPermission(FunctionsConstant.SYSTEM_ROLE, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _roleManager.Roles.ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<IdentityRole>, IEnumerable<RoleViewModel>>(models));
        }

        // GET: api/roles/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.SYSTEM_ROLE, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _roleManager.Roles
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound();

            if (!string.IsNullOrEmpty(q))
                models = models.Where(w => w.Id.Contains(q) || w.Name.Contains(q)).ToList();

            return Ok(new Pagination<RoleViewModel>
            {
                Items = _mapper.Map<IEnumerable<IdentityRole>, IEnumerable<RoleViewModel>>(models),
                TotalRecords = await _roleManager.Roles.CountAsync()
            });
        }

        // GET api/roles/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_ROLE, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _roleManager.FindByIdAsync(id);

            if (model == null)
                return NotFound();

            return Ok(_mapper.Map<IdentityRole, RoleViewModel>(model));
        }

        // POST api/roles
        [HttpPost]
        [IdentityPermission(FunctionsConstant.SYSTEM_ROLE, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] RoleRequestModel requestModel)
        {
            var model = _mapper.Map<RoleRequestModel, IdentityRole>(requestModel);

            var result = await _roleManager.CreateAsync(model);
            if (result.Succeeded)
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);

            return BadRequest();
        }

        // PUT api/roles/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_ROLE, CommandsConstant.UPDATE)]
        public async Task<IActionResult> Put(string id, [FromBody] RoleRequestModel requestModel)
        {
            var model = await _roleManager.FindByIdAsync(id);

            if (model == null)
                return NotFound();

            _mapper.Map(requestModel, model);

            var result = await _roleManager.UpdateAsync(model);
            if (result.Succeeded)
                return NoContent();

            return BadRequest();
        }

        // DELETE api/roles/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_ROLE, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await _roleManager.FindByIdAsync(id);

            if (model == null)
                return NotFound();

            var result = await _roleManager.DeleteAsync(model);
            if (result.Succeeded)
                return NoContent();

            return BadRequest();
        }

        #region Role via Permissions actions
        [HttpGet("{roleId}/permissions")]
        [IdentityPermission(FunctionsConstant.SYSTEM_ROLE, CommandsConstant.READ)]
        public async Task<IActionResult> GetPermissions(string roleId)
        {
            var models = await _context.Permissions.Where(w => w.RoleId == roleId).ToListAsync();

            if (models == null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<Permission>, IEnumerable<PermissionViewModel>>(models));
        }

        [HttpPut("{roleId}/Permissions")]
        [IdentityPermission(FunctionsConstant.SYSTEM_ROLE, CommandsConstant.UPDATE)]
        public async Task<IActionResult> PutPermissionByRoleId(string roleId, [FromBody] PermissionUpdateRequestModel updateRequestModel)
        {
            var newPermissions = new List<Permission>();

            foreach (var p in updateRequestModel.PermissionViewModels)
            {
                newPermissions.Add(new Permission()
                {
                    CommandId = p.CommandId,
                    FunctionId = p.FunctionId,
                    RoleId = p.RoleId
                });
            }

            var existingPermissions = _context.Permissions.Where(x => x.RoleId == roleId);

            _context.Permissions.RemoveRange(existingPermissions);
            _context.Permissions.AddRange(newPermissions);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return NoContent();
            }

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }
        #endregion Role via Permissions actions
    }
}
