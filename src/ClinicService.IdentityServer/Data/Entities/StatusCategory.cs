using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("StatusCategories")]
    public class StatusCategory
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        public string Color { get; set; }

        public string ParentId { get; set; }

        public int? SortOrder { get; set; }
    }
}
