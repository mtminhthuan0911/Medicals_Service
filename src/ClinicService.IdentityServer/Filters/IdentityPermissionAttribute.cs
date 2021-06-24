using System;
using ClinicService.IdentityServer.Constants;
using Microsoft.AspNetCore.Mvc;

namespace ClinicService.IdentityServer.Filters
{
    public class IdentityPermissionAttribute : TypeFilterAttribute
    {
        public IdentityPermissionAttribute(FunctionsConstant functionId, CommandsConstant commandId)
           : base(typeof(IdentityPermissionFilter))
        {
            Arguments = new object[] { functionId, commandId };
        }
    }
}
