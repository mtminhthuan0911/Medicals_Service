using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("MedicalExaminations")]
    public class MedicalExamination
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [Required]
        public string PatientId { get; set; }

        [Required]
        public string StatusCategoryId { get; set; }

        public int? AppointmentId { get; set; }

        public int? ReappointmentId { get; set; }
    }
}
