using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("PaymentMethods")]
    public class PaymentMethod
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        public string Logo { get; set; }

        public int? SortOrder { get; set; }
    }
}
