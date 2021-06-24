using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("Specialties")]
    public class Specialty
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(512)]
        public string Description { get; set; }

        public string Content { get; set; }

        public string ParentId { get; set; }

        public int? SortOrder { get; set; }

        public string SeoAlias { get; set; }
    }
}
