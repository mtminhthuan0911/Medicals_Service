using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class MedicalServiceTypeViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public int? SortOrder { get; set; }
    }
}
