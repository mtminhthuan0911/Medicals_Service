using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("WebsiteSections")]
    public class WebsiteSection
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        public string Content { get; set; }

        public string ParentId { get; set; }

        public int? SortOrder { get; set; }

        public string SeoAlias { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
