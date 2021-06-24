using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class PermissionRequestModel
    {
        public string FunctionId { get; set; }

        public string CommandId { get; set; }

        public string RoleId { get; set; }
    }
}
