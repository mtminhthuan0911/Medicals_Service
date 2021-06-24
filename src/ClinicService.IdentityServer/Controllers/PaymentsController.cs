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
    [Route("api/payments")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(ApplicationDbContext context, IMapper mapper, ILogger<PaymentsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/payments
        [HttpGet]
        [IdentityPermission(FunctionsConstant.PAYMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetAll()
        {
            var viewModels = await (from p in _context.Payments
                                join pm in _context.PaymentMethods on p.PaymentMethodId equals pm.Id
                                join sc in _context.StatusCategories on p.StatusCategoryId equals sc.Id
                                select new PaymentViewModel
                                {
                                    AppointmentId = p.AppointmentId,
                                    CreatedDate = p.CreatedDate,
                                    ModifiedDate = p.ModifiedDate,
                                    PaymentMethodId = p.PaymentMethodId,
                                    PaymentMethodName = pm.Name,
                                    StatusCategoryId = p.StatusCategoryId,
                                    StatusCategoryName = sc.Name
                                }).ToListAsync();

            if (viewModels == null || viewModels.Count == 0)
                return NotFound();

            return Ok(viewModels);
        }

        // GET: api/payments/search?q={q}&page={page}&limit={limit}
        [HttpGet("search")]
        [IdentityPermission(FunctionsConstant.PAYMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetAllPaging(string q = "", int page = 1, int limit = 10)
        {
            var viewModels = await (from p in _context.Payments
                                 join pm in _context.PaymentMethods on p.PaymentMethodId equals pm.Id
                                 join sc in _context.StatusCategories on p.StatusCategoryId equals sc.Id
                                 select new PaymentViewModel
                                 {
                                     AppointmentId = p.AppointmentId,
                                     CreatedDate = p.CreatedDate,
                                     ModifiedDate = p.ModifiedDate,
                                     PaymentMethodId = p.PaymentMethodId,
                                     PaymentMethodName = pm.Name,
                                     StatusCategoryId = p.StatusCategoryId,
                                     StatusCategoryName = sc.Name
                                 })
                                .Skip((page - 1) * limit)
                                .Take(limit)
                                .ToListAsync();

            if (viewModels == null || viewModels.Count == 0)
                return NotFound();

            return Ok(viewModels);
        }

        // GET api/payments/{id}
        [HttpGet("{id}")]
        [IdentityPermission(FunctionsConstant.PAYMENT, CommandsConstant.READ)]
        public async Task<IActionResult> GetById(int id)
        {
            var viewModel = await (from p in _context.Payments
                               join pm in _context.PaymentMethods on p.PaymentMethodId equals pm.Id
                               join sc in _context.StatusCategories on p.StatusCategoryId equals sc.Id
                               where p.AppointmentId == id
                               select new PaymentViewModel
                               {
                                   AppointmentId = p.AppointmentId,
                                   CreatedDate = p.CreatedDate,
                                   ModifiedDate = p.ModifiedDate,
                                   PaymentMethodId = p.PaymentMethodId,
                                   PaymentMethodName = pm.Name,
                                   StatusCategoryId = p.StatusCategoryId,
                                   StatusCategoryName = sc.Name
                               }).FirstOrDefaultAsync();

            if (viewModel == null)
                return NotFound();

            return Ok(viewModel);
        }

        // POST api/payments
        [HttpPost]
        [IdentityPermission(FunctionsConstant.PAYMENT, CommandsConstant.CREATE)]
        public async Task<IActionResult> Post([FromBody] PaymentRequestModel requestModel)
        {
            var model = _mapper.Map<PaymentRequestModel, Payment>(requestModel);
            model.CreatedDate = DateTime.Now;

            await _context.Payments.AddAsync(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = model.AppointmentId }, model);

            return BadRequest();
        }

        // PUT api/payments/{id}
        [HttpPut("{id}")]
        [IdentityPermission(FunctionsConstant.PAYMENT, CommandsConstant.UPDATE)]
        public async Task<IActionResult> Put(int id, [FromBody] PaymentRequestModel requestModel)
        {
            var model = await _context.Payments.FindAsync(id);

            if (model == null)
                return NotFound();

            _mapper.Map(requestModel, model);

            model.ModifiedDate = DateTime.Now;

            _context.Payments.Update(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest();
        }

        // DELETE api/payments/{id}
        [HttpDelete("{id}")]
        [IdentityPermission(FunctionsConstant.PAYMENT, CommandsConstant.DELETE)]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Payments.FindAsync(id);

            if (model == null)
                return NotFound();

            _context.Payments.Remove(model);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest();
        }
    }
}
