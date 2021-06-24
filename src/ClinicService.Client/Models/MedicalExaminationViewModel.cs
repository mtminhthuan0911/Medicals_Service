using System;
using System.Collections.Generic;

namespace ClinicService.Client.Models
{
    public class MedicalExaminationViewModel
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string PatientId { get; set; }

        public string PatientFullName { get; set; }

        public string StatusCategoryId { get; set; }

        public string StatusCategoryName { get; set; }

        public IEnumerable<MedicalExaminationDetailViewModel> Details { get; set; }

        public IEnumerable<PrescriptionViewModel> Prescriptions { get; set; }

        public IEnumerable<MedicalExaminationAttachmentViewModel> Attachments { get; set; }
    }
}
