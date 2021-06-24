using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? MedicalServiceId { get; set; }

        public string GuessFullName { get; set; }

        public string GuessPhoneNumber { get; set; }

        [Required]
        public string PatientId { get; set; }

        [Required]
        public string StatusCategoryId { get; set; }

        [Required]
        public int ClinicBranchId { get; set; }
    }
}
