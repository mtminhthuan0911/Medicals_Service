using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("Permissions")]
    public class Permission
    {
        [Required]
        public string FunctionId { get; set; }

        [Required]
        public string CommandId { get; set; }

        [Required]
        public string RoleId { get; set; }
    }
}
