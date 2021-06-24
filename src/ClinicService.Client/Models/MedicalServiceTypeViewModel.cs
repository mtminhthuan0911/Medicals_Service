using System;
namespace ClinicService.Client.Models
{
    public class MedicalServiceTypeViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public int? SortOrder { get; set; }
    }
}
