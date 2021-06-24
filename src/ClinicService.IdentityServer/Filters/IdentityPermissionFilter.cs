using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ClinicService.IdentityServer.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace ClinicService.IdentityServer.Filters
{
    public class IdentityPermissionFilter : IAuthorizationFilter
    {
        private readonly FunctionsConstant _functionId;
        private readonly CommandsConstant _commandId;

        public IdentityPermissionFilter(FunctionsConstant functionId, CommandsConstant commandId)
        {
            _functionId = functionId;
            _commandId = commandId;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                                 .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

            if (hasAllowAnonymous) return;

            var permissionsClaim = context.HttpContext.User.Claims
                .SingleOrDefault(c => c.Type == SystemsConstant.Claims.Permission);
            if (permissionsClaim != null)
            {
                var permissions = JsonConvert.DeserializeObject<IEnumerable<string>>(permissionsClaim.Value);
                if (!permissions.Contains(_functionId + "_" + _commandId))
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            else context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
        }
    }
}
