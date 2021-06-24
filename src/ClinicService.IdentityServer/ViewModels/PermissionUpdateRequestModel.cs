using System;
using System.Collections.Generic;

namespace ClinicService.IdentityServer.ViewModels
{
    public class PermissionUpdateRequestModel
    {
        public List<PermissionViewModel> PermissionViewModels { get; set; }

        public PermissionUpdateRequestModel()
        {
            PermissionViewModels = new List<PermissionViewModel>();
        }
    }
}
