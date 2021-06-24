using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("MedicalExaminationDetails")]
    public class MedicalExaminationDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Diagnostic { get; set; }

        [Required]
        public string Treatment { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [Required]
        public string DoctorId { get; set; }

        [Required]
        public int MedicalExaminationId { get; set; }
    }
}
