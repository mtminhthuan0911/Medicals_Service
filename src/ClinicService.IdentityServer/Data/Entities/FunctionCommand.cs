using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("FunctionCommands")]
    public class FunctionCommand
    {
        [Required]
        public string FunctionId { get; set; }

        [Required]
        public string CommandId { get; set; }
    }
}
