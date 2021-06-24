using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("MedicalServiceAttachments")]
    public class MedicalServiceAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public long? FileSize { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public int MedicalServiceId { get; set; }
    }
}
