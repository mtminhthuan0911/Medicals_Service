using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("MedicalServiceTypes")]
    public class MedicalServiceType
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Icon { get; set; }

        public int? SortOrder { get; set; }
    }
}
