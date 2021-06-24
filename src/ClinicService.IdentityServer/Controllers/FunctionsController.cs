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
    [Route("api/functions")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FunctionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<FunctionsController> _logger;

        public FunctionsController(ApplicationDbContext context, IMapper mapper, ILogger<FunctionsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/functions
        [HttpGet]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var models = await _context.Functions.OrderBy(o => o.SortOrder).ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<Function>, IEnumerable<FunctionViewModel>>(models));
        }

        // GET: api/functions/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var models = await _context.Functions
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            if (models == null || models.Count == 0)
                return NotFound();

            if (!string.IsNullOrEmpty(q))
                models = models.Where(w => w.Id.Contains(q) || w.Name.Contains(q) || w.Url.Contains(q) || w.ParentId.Contains(q) || w.Icon.Contains(q)).ToList();

            return Ok(_mapper.Map<IEnumerable<Function>, IEnumerable<FunctionViewModel>>(models));
        }

        // GET api/functions/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _context.Functions.FindAsync(id);

            if (model == null)
                return NotFound();

            return Ok(_mapper.Map<Function, FunctionViewModel>(model));
        }

        // POST api/functions
        [HttpPost]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] FunctionRequestModel requestModel)
        {
            var model = _mapper.Map<FunctionRequestModel, Function>(requestModel);

            //if(!string.IsNullOrEmpty(requestModel.SortOrder))
            //    model.SortOrder = int.Parse(requestModel.SortOrder);
            //else model.SortOrder = null;

            await _context.Functions.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);

            return BadRequest();
        }

        // PUT api/functions/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.UPDATE)]
        public async Task<IActionResult> Put(string id, [FromBody] FunctionRequestModel requestModel)
        {
            var model = await _context.Functions.FindAsync(id);

            if (model == null)
                return NotFound();

            _mapper.Map(requestModel, model);

            //if (!string.IsNullOrEmpty(requestModel.SortOrder))
            //    model.SortOrder = int.Parse(requestModel.SortOrder);
            //else model.SortOrder = null;


            _context.Functions.Update(model);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest();
        }

        // DELETE api/functions/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await _context.Functions.FindAsync(id);

            if (model == null)
                return NotFound();

            _context.Functions.Remove(model);

            var commands = await _context.FunctionCommands.Where(w => w.FunctionId == id).ToListAsync();
            if (commands != null && commands.Count > 0)
                _context.FunctionCommands.RemoveRange(commands);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest();
        }

        #region Function via Commands actions
        // GET: api/functions/get-function-commands-view
        [HttpGet("get-function-commands-view")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetFunctionCommandsView()
        {
            var result = await (from f in _context.Functions
                                join fc in _context.FunctionCommands on f.Id equals fc.FunctionId
                                join c in _context.Commands on fc.CommandId equals c.Id into tblCommands
                                from c in tblCommands.DefaultIfEmpty()
                                group new { f, fc, c } by new
                                {
                                    f.Id, f.Name, f.ParentId
                                } into tblResult
                                orderby tblResult.Key.ParentId
                                select new FunctionCommandDisplayViewModel
                                {
                                    Id = tblResult.Key.Id,
                                    Name = tblResult.Key.Name,
                                    ParentId = tblResult.Key.ParentId,
                                    HasViewed = tblResult.Sum(s => s.c.Id.Contains("READ") ? 1 : 0) == 1 ? 1 : 0,
                                    HasCreated = tblResult.Sum(s => s.c.Id.Contains("CREATE") ? 1 : 0) == 1 ? 1 : 0,
                                    HasUpdated = tblResult.Sum(s => s.c.Id.Contains("UPDATE") ? 1 : 0) == 1 ? 1 : 0,
                                    HasDeleted = tblResult.Sum(s => s.c.Id.Contains("DELETE") ? 1 : 0) == 1 ? 1 : 0
                                }).ToListAsync();

            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }

        // GET api/functions/{functionId}/Commands
        [HttpGet("{functionId}/Commands")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.READ)]
        public async Task<IActionResult> GetCommandsByFunctionId(string functionId)
        {
            var query = from a in _context.Commands
                        join cif in _context.FunctionCommands on a.Id equals cif.CommandId into result1
                        from commandInFunction in result1.DefaultIfEmpty()
                        join f in _context.Functions on commandInFunction.FunctionId equals f.Id into result2
                        from function in result2.DefaultIfEmpty()
                        select new
                        {
                            a.Id,
                            a.Name,
                            commandInFunction.FunctionId
                        };

            if (query == null)
                return NotFound();

            query = query.Where(x => x.FunctionId == functionId);

            var items = await query.Select(x => new CommandViewModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return Ok(items);
        }

        // POST api/functions/{functionId}/Commands
        [HttpPost("{functionId}/Commands")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.CREATE)]
        public async Task<IActionResult> PostCommandByFunctionId(string functionId, [FromBody] FunctionCommandRequestModel requestModel)
        {
            var cmdInFunc = await _context.FunctionCommands.FindAsync(functionId, requestModel.CommandId);
            if (cmdInFunc != null)
            {
                return BadRequest(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Lệnh đã được thêm vào chức năng."
                }); ;
            }

            var model = new FunctionCommand()
            {
                CommandId = requestModel.CommandId,
                FunctionId = requestModel.FunctionId
            };

            _context.FunctionCommands.Add(model);

            int result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Created(string.Empty, model);
            }

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }

        // DELETE api/functions/{functionId}/Commands/{commandId}
        [HttpDelete("{functionId}/Commands/{commandId}")]
        [IdentityPermission(FunctionsConstant.SYSTEM_FUNCTION, CommandsConstant.DELETE)]
        public async Task<IActionResult> DeleteCommandByFunctionId(string functionId, string commandId)
        {
            var cmdInFunc = await _context.FunctionCommands.FindAsync(functionId, commandId);
            if (cmdInFunc == null)
            {
                return NotFound(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = string.Format(MessagesConstant.RECORD_NOT_FOUND, "Chức năng")
                });
            }

            _context.FunctionCommands.Remove(cmdInFunc);

            int result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return NoContent();
            }

            return BadRequest(new ErrorMessageModel
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = MessagesConstant.DEFAULT_BAD_REQUEST
            });
        }
        #endregion Function via Commands actions
    }
}
