using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.IdentityServer.Data.Entities
{
    [Table("Prescriptions")]
    public class Prescription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        public string Subname { get; set; }

        [Required]
        [MaxLength(64)]
        public string Quantity { get; set; }

        [Required]
        [MaxLength(64)]
        public string AvailableQuantity { get; set; }

        [Required]
        public int Total { get; set; }

        [Required]
        public int Take { get; set; }

        public bool? IsMorning { get; set; }

        public bool? IsAfternoon { get; set; }

        public bool? IsEvening { get; set; }

        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [Required]
        public string DoctorId { get; set; }

        [Required]
        public int MedicalExaminationId { get; set; }
    }
}
