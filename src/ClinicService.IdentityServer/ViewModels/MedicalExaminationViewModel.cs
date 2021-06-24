using System;
using System.Collections.Generic;
using ClinicService.IdentityServer.Data.Entities;

namespace ClinicService.IdentityServer.ViewModels
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

        public int? AppointmentId { get; set; }

        public int? ReappointmentId { get; set; }

        public IEnumerable<MedicalExaminationDetailViewModel> Details { get; set; }

        public IEnumerable<PrescriptionViewModel> Prescriptions { get; set; }

        public IEnumerable<MedicalExaminationAttachment> Attachments { get; set; }
    }
}
