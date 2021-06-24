using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("Functions")]
    public class Function
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        public string ParentId { get; set; }

        public int? SortOrder { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }
    }
}
