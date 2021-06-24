using System;
using System.Linq;
using System.Net;
using ClinicService.IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClinicService.IdentityServer.Filters
{
    public class ValidatorActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"].ToString();
            if (controllerName == "Account")
                return;

            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                        .SelectMany(sm => sm.Value.Errors)
                        .Select(s => s.ErrorMessage).ToArray();

                context.Result = new BadRequestObjectResult(new ErrorMessageModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = string.Join(",", errors)
                });
            }
        }
    }
}
