using System;
namespace ClinicService.Client.Models
{
    public class ReappointmentViewModel
    {
        public int Id { get; set; }

        public DateTime ReappointmentDate { get; set; }

        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string PatientId { get; set; }

        public string PatientFullName { get; set; }

        public string StatusCategoryId { get; set; }

        public string StatusCategoryName { get; set; }

        public int FromMedicalExaminationId { get; set; }
    }
}
