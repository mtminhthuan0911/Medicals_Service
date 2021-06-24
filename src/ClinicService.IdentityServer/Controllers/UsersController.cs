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
    [Route("api/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, ILogger<UsersController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _userManager.Users.ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(models));
        }

        // GET: api/users/get-by-role/{roleId}
        [HttpGet("get-by-role/{roleId}")]
        [IdentityPermission(FunctionsConstant.PERSONNEL, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllByRoleId(string roleId)
        {
            var roleName = await _roleManager.FindByIdAsync(roleId);
            if (roleName == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = string.Format(MessagesConstant.RECORD_NOT_FOUND, "Mã vai trò")
                });

            var models = await _userManager.GetUsersInRoleAsync(roleName.Name);
            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            return Ok(_mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(models));
        }

        // GET: api/users/get-by-role/{roleId}/search?q={q}&page={page}&limit={limit}
        [HttpGet("get-by-role/{roleId}/search")]
        [IdentityPermission(FunctionsConstant.PERSONNEL, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllByRoleId(string roleId, string q = "", int page = 1, int limit = 10)
        {
            var roleName = await _roleManager.FindByIdAsync(roleId);
            if (roleName == null)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = string.Format(MessagesConstant.RECORD_NOT_FOUND, "Mã vai trò")
                });

            var models = await _userManager.GetUsersInRoleAsync(roleName.Name);

            var totalRecords = models.Count();

            if (models == null || models.Count == 0)
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = MessagesConstant.DEFAULT_NOT_FOUND
                });

            models = models
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            if (!string.IsNullOrEmpty(q))
            {
                models = models.Where(w => w.UserName.Contains(q) || w.FirstName.Contains(q) || w.LastName.Contains(q) || (!string.IsNullOrEmpty(w.PhoneNumber) ? w.PhoneNumber.Contains(q) : false) || (!string.IsNullOrEmpty(w.Address) ? w.Address.Contains(q) : false)).ToList();
            }

            return Ok(new Pagination<UserViewModel>
            {
                Items = _mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(models),
                TotalRecords = totalRecords
            });
        }

        // GET: api/users/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _userManager.Users
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound();

            if (!string.IsNullOrEmpty(q))
                models = models.Where(w => w.UserName.Contains(q) || w.FirstName.Contains(q) || w.LastName.Contains(q) || w.PhoneNumber.Contains(q) || w.Address.Contains(q)).ToList();

            return Ok(new Pagination<UserViewModel>
            {
                Items = _mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(models),
                TotalRecords = await _userManager.Users.CountAsync()
            });
        }

        // GET api/users/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _userManager.FindByIdAsync(id);

            if (model == null)
                return NotFound();

            return Ok(_mapper.Map<ApplicationUser, UserViewModel>(model));
        }

        // GET api/users/find-by-name/{userName}
        [HttpGet("find-by-name/{userName}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.READ)]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            var model = await _userManager.FindByNameAsync(userName);

            if (model == null)
                return NotFound();

            return Ok(_mapper.Map<ApplicationUser, UserViewModel>(model));
        }

        // POST api/users
        [HttpPost]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] UserRequestModel requestModel)
        {
            var model = _mapper.Map<UserRequestModel, ApplicationUser>(requestModel);

            model.Id = Guid.NewGuid().ToString();

            if (!string.IsNullOrEmpty(requestModel.DateOfBirth))
                model.DateOfBirth = DateTime.Parse(requestModel.DateOfBirth);

            model.CreatedDate = DateTime.Now;

            var result = await _userManager.CreateAsync(model, "Default@123");
            if (result.Succeeded)
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);

            return BadRequest();
        }

        // PUT api/users/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.UPDATE)]
        public async Task<IActionResult> Put(string id, [FromBody] UserRequestModel requestModel)
        {
            var model = await _userManager.FindByIdAsync(id);

            if (model == null)
                return NotFound();

            if (!string.IsNullOrEmpty(requestModel.DateOfBirth))
                model.DateOfBirth = DateTime.Parse(requestModel.DateOfBirth);

            _mapper.Map(requestModel, model);

            model.ModifiedDate = DateTime.Now;

            var result = await _userManager.UpdateAsync(model);
            if (result.Succeeded)
                return NoContent();

            return BadRequest();
        }

        // PUT api/users/reset-password/{id}
        [HttpPut("reset-password/{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.UPDATE)]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var model = await _userManager.FindByIdAsync(id);

            if (model == null)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(model);

            var result = await _userManager.ResetPasswordAsync(model, token, "Default@123");
            if (result.Succeeded)
                return NoContent();

            return BadRequest();
        }

        // DELETE api/users/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await _userManager.FindByIdAsync(id);

            if (model == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(model);
            if (result.Succeeded)
                return NoContent();

            return BadRequest();
        }

        #region User via Roles actions
        // GET api/users/{userId}/Roles
        [HttpGet("{userId}/Roles")]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.READ)]
        public async Task<IActionResult> GetRolesByUserId(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }

        // POST api/users/{userId}/Roles
        [HttpPost("{userId}/Roles")]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.CREATE)]
        public async Task<IActionResult> PostRoleByUserId(string userId, [FromBody] string roleName)
        {
            if (roleName.Length == 0)
            {
                return BadRequest(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = string.Format(MessagesConstant.RECORD_REQUIRED, "Tên quyền")
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = string.Format(MessagesConstant.RECORD_NOT_FOUND, "Tài khoản")
                });
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = string.Format(MessagesConstant.DEFAULT_BAD_REQUEST)
            });
        }

        // DELETE api/users/{userId}/Roles
        [HttpDelete("{userId}/Roles")]
        [IdentityPermission(FunctionsConstant.SYSTEM_USER, CommandsConstant.DELETE)]
        public async Task<IActionResult> DeleteRoleByUserId(string userId, [FromQuery] string roleName)
        {
            if (roleName.Length == 0)
            {
                return BadRequest(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = string.Format(MessagesConstant.RECORD_REQUIRED, "Tên quyền")
                });
            }

            //var adminRoles = _roleManager.Roles.Where(w => w.Id == SystemConstants.Admin.ID).ToList();
            var adminRoles = await _userManager.GetUsersInRoleAsync(roleName);
            var role = await _roleManager.FindByNameAsync(roleName);

            if (adminRoles.Count == 1 && role.Id == SystemsConstant.Users.ID_ADMIN)
            {
                return BadRequest(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = string.Format(MessagesConstant.RECORD_NOT_REMOVE, "Tài khoản")
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = string.Format(MessagesConstant.RECORD_NOT_FOUND, "Tài khoản")
                });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = string.Format(MessagesConstant.DEFAULT_BAD_REQUEST)
            });
        }
        #endregion User via Roles actions

        #region User via Permissions actions
        [HttpGet("{userId}/Menu")]
        public async Task<IActionResult> GetMenuByUserPermission(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            var query = from f in _context.Functions
                        join p in _context.Permissions on f.Id equals p.FunctionId
                        join r in _roleManager.Roles on p.RoleId equals r.Id
                        join a in _context.Commands on p.CommandId equals a.Id
                        where roles.Contains(r.Name) && a.Id == "READ"
                        orderby f.SortOrder, f.Id
                        select new FunctionViewModel
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Url = f.Url,
                            ParentId = f.ParentId,
                            SortOrder = f.SortOrder,
                            Icon = f.Icon
                        };
            var data = await query.Distinct()
                .OrderBy(x => x.ParentId)
                .ThenBy(x => x.SortOrder)
                .ToListAsync();

            return Ok(data);
        }
        #endregion User via Permissions actions
    }
}
