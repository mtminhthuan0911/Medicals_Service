using System;
namespace ClinicService.Client.Models
{
    public class MedicalExaminationDetailViewModel
    {
        public int Id { get; set; }

        public string Diagnostic { get; set; }

        public string Treatment { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string DoctorId { get; set; }

        public string DoctorFullName { get; set; }

        public int MedicalExaminationId { get; set; }
    }
}
